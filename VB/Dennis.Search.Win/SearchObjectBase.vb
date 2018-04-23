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
		<Custom(PropertyInfoNodeWrapper.PropertyEditorTypeAttribute, SearchObjectPropertyEditorFullName)> _
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
		Protected Overrides Overloads Sub OnChanged(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
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
				Criteria.Operands.Add(New BinaryOperator(propertyName, propertyValue, BinaryCriteriaType))
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
				If result IsNot Nothing AndAlso result.Length > 0 Then
					OnSearchComplete(New SearchCompleteEventArgs(result(0), exception))
				Else
					OnSearchComplete(New SearchCompleteEventArgs(Nothing, exception))
				End If
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