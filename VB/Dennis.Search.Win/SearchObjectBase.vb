' Developer Express Code Central Example:
' How to search for objects by using all the properties or by using more complex criteria
' 
' This example provides a possible workaround for the suggestion. The
' Dennis.Search.Win module, shown in the example, provides API to create a
' non-persistent object that can be used to search by properties of a business
' class. Such a non-persistent search-object should implement the ISearchObject
' contract. By default, it's supported by the abstract and generic
' SearchObjectBase class in the module. Search-objects are shown in a Detail View
' with the help of the SearchObjectViewController. To compose a search criteria, a
' specific SearchObjectPropertyEditor is used (it contains the Search and Clear
' buttons). An example use of the implemented module is illustrated in the
' WinSolution.Module.Win module. There is a ProductSearchObject class that is
' inherited from the base SearchObjectBase class. If necessary, you can create
' several search objects for one business class. In UI, all search objects will be
' listed in a drop-down list in the toolbar of a View of your business class.
' Download and run the attached example to see how this works in action. (Take
' special note of the SearchObjectBase class implementation, because it
' asynchronously loads search results of a specific type from the data store into
' the Search Results nested List View.) See Also:
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E1744


Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports System.Collections
Imports System.ComponentModel
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp.NodeWrappers

Namespace Dennis.Search.Win
	Public Interface ISearchObject
		Event SearchStart As EventHandler(Of SearchStartEventArgs)
		Event SearchComplete As EventHandler(Of SearchCompleteEventArgs)
		Property Criteria() As GroupOperator
		ReadOnly Property SearchResults() As ICollection
		ReadOnly Property IsSearching() As Boolean
		Sub Search()
		Sub Reset()
	End Interface
	Public Class SearchStartEventArgs
		Inherits CancelEventArgs
		Private criteriaCore As CriteriaOperator
		Public Sub New(ByVal criteria As CriteriaOperator)
			criteriaCore = criteria
		End Sub
		Public Property Criteria() As CriteriaOperator
			Get
				Return criteriaCore
			End Get
			Set(ByVal value As CriteriaOperator)
				criteriaCore = value
			End Set
		End Property
	End Class
	Public Class SearchCompleteEventArgs
		Inherits EventArgs
		Private searchResultsCore As ICollection
		Private errorCore As Exception
		Public Sub New(ByVal searchResults As ICollection, ByVal [error] As Exception)
			searchResultsCore = searchResults
			errorCore = [error]
		End Sub
		Public ReadOnly Property [Error]() As Exception
			Get
				Return errorCore
			End Get
		End Property
		Public ReadOnly Property SearchResults() As ICollection
			Get
				Return searchResultsCore
			End Get
		End Property
	End Class
	<NonPersistent> _
	Public MustInherit Class SearchObjectBase(Of T As IXPSimpleObject)
		Inherits XPBaseObject
		Implements ISearchObject
		Private searchResultsCore As XPCollection(Of T)
		Private criteriaCore As GroupOperator
		Private isSearchingCore As Boolean
		Protected Const SearchObjectPropertyEditorFullName As String = "Dennis.Search.Win.Editors"
		Protected Const SearchResultsCollectionName As String = "SearchResults"
		Protected Const BinaryCriteriaType As BinaryOperatorType = BinaryOperatorType.Like
		Protected Const GroupCriteriaType As GroupOperatorType = GroupOperatorType.And
		Protected ReadOnly EmptyCollectionCriteria As CriteriaOperator = CriteriaOperator.Parse("1=0")
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		<Custom("PropertyEditorType", SearchObjectPropertyEditorFullName)> _
		Public Property Criteria() As GroupOperator Implements ISearchObject.Criteria
			Get
				If ReferenceEquals(criteriaCore, Nothing) Then
					criteriaCore = New GroupOperator(GroupCriteriaType, EmptyCollectionCriteria)
				End If
				Return criteriaCore
			End Get
			Set(ByVal value As GroupOperator)
				SetPropertyValue("Criteria", criteriaCore, value)
			End Set
		End Property
		Protected Overrides Sub OnChanged(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			MyBase.OnChanged(propertyName, oldValue, newValue)
			If (Not IsLoading) AndAlso (Not IsSaving) Then
				If propertyName <> "Criteria" AndAlso propertyName <> SearchResultsCollectionName Then
					UpdateCriteria(propertyName, newValue)
				End If
			End If
		End Sub
		Protected Overridable Sub UpdateCriteria(ByVal propertyName As String, ByVal propertyValue As Object)
			Dim updated As Boolean = False
			For Each operand As CriteriaOperator In Criteria.Operands
				Dim binaryOperator As BinaryOperator = TryCast(operand, BinaryOperator)
				If (Not ReferenceEquals(binaryOperator, Nothing)) AndAlso (Not ReferenceEquals(binaryOperator, EmptyCollectionCriteria)) Then
					If (CType(binaryOperator.LeftOperand, OperandProperty)).PropertyName = propertyName Then
						CType(binaryOperator.RightOperand, OperandValue).Value = propertyValue
						updated = True
						Exit For
					End If
				End If
			Next operand
			If (Not updated) Then
				If TypeOf propertyValue Is String Then
					Criteria.Operands.Add(New BinaryOperator(propertyName, propertyValue, BinaryCriteriaType))
				Else
					Criteria.Operands.Add(New BinaryOperator(propertyName, propertyValue))
				End If
			End If
		End Sub
		<Browsable(False)> _
		Public ReadOnly Property IsSearching() As Boolean Implements ISearchObject.IsSearching
			Get
				Return isSearchingCore
			End Get
		End Property
		Public ReadOnly Property SearchResults() As XPCollection(Of T)
			Get
				If searchResultsCore Is Nothing Then
					searchResultsCore = New XPCollection(Of T)(Session, EmptyCollectionCriteria, Nothing)
				End If
				Return searchResultsCore
			End Get
		End Property
		Public Overridable Sub Reset() Implements ISearchObject.Reset
			criteriaCore = Nothing
			searchResultsCore.Criteria = EmptyCollectionCriteria
		End Sub
		Private ReadOnly Property ISearchObject_SearchResults() As ICollection Implements ISearchObject.SearchResults
			Get
				Return SearchResults
			End Get
		End Property
		Public Overridable Sub Search() Implements ISearchObject.Search
			isSearchingCore = True
			If Criteria.Operands.Contains(EmptyCollectionCriteria) Then
				Criteria.Operands.Remove(EmptyCollectionCriteria)
			End If
			Dim args As New SearchStartEventArgs(Criteria)
			OnSearchStart(args)
			If (Not args.Cancel) Then
				searchResultsCore.SuspendChangedEvents()
				searchResultsCore.Criteria = args.Criteria
				searchResultsCore.LoadAsync(AddressOf LoadSearchResultsCallback)
			End If
		End Sub
		Private Sub LoadSearchResultsCallback(ByVal result() As ICollection, ByVal exception As Exception)
			Try
				If exception IsNot Nothing Then
					Throw New InvalidOperationException(String.Format("Cannot load search results for the {0} object type due to an error: {1}", GetType(T), exception.Message))
				End If
			Finally
				isSearchingCore = False
				OnSearchComplete(New SearchCompleteEventArgs(If(result IsNot Nothing AndAlso result.Length > 0, result(0), Nothing), exception))
				searchResultsCore.ResumeChangedEvents()
			End Try
		End Sub
		Protected Overridable Sub OnSearchStart(ByVal args As SearchStartEventArgs)
			RaiseEvent SearchStart(Me, args)
		End Sub
		Protected Overridable Sub OnSearchComplete(ByVal args As SearchCompleteEventArgs)
			RaiseEvent SearchComplete(Me, args)
		End Sub
		Public Event SearchStart As EventHandler(Of SearchStartEventArgs) Implements ISearchObject.SearchStart
		Public Event SearchComplete As EventHandler(Of SearchCompleteEventArgs) Implements ISearchObject.SearchComplete
	End Class
End Namespace