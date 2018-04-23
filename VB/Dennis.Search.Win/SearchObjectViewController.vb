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
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.SystemModule

Namespace Dennis.Search.Win
	Public Class SearchObjectViewController
		Inherits ViewController
		Private Const ActiveKeySearchObject As String = "ISearchObject"
		Private scaSearchObjectActionCore As SingleChoiceAction
		Public Const SearchResultsCollectionName As String = "SearchResults"
		Public Sub New()
			scaSearchObjectActionCore = New SingleChoiceAction(Me, "SearchObject", PredefinedCategory.Search)
			AddHandler scaSearchObjectActionCore.Execute, AddressOf saSearchObjectActionCore_Execute
			scaSearchObjectActionCore.Caption = My.Resources.SearchObjectActionText
			scaSearchObjectActionCore.ImageName = "Action_Search"
            scaSearchObjectActionCore.ItemType = SingleChoiceActionItemType.ItemIsOperation
		End Sub
		Public ReadOnly Property SearchObjectAction() As SingleChoiceAction
			Get
				Return scaSearchObjectActionCore
			End Get
		End Property
		Private Sub saSearchObjectActionCore_Execute(ByVal sender As Object, ByVal args As SingleChoiceActionExecuteEventArgs)
			SearchObject(args)
		End Sub
		Protected Overridable Sub SearchObject(ByVal args As SingleChoiceActionExecuteEventArgs)
			Dim os As IObjectSpace = Application.CreateObjectSpace()
			Dim obj As Object = os.CreateObject(CType(args.SelectedChoiceActionItem.Data, Type))
			Dim dv As DetailView = Application.CreateDetailView(os, obj)
			args.ShowViewParameters.CreatedView = dv
		End Sub
		Protected Overrides Sub OnActivated()
			MyBase.OnActivated()
			InitSearchObjectActionItems()
			If GetType(ISearchObject).IsAssignableFrom(View.ObjectTypeInfo.Type) Then
				Dim dv As DetailView = TryCast(View, DetailView)
				If dv IsNot Nothing Then
					For Each editor As ListPropertyEditor In dv.GetItems(Of ListPropertyEditor)()
						If editor.PropertyName = SearchResultsCollectionName Then
							editor.Frame.GetController(Of LinkUnlinkController)().LinkAction.Active(ActiveKeySearchObject) = False
							editor.Frame.GetController(Of LinkUnlinkController)().UnlinkAction.Active(ActiveKeySearchObject) = False
							editor.Frame.GetController(Of DeleteObjectsViewController)().DeleteAction.Active(ActiveKeySearchObject) = False
							editor.Frame.GetController(Of NewObjectViewController)().NewObjectAction.Active(ActiveKeySearchObject) = False
						End If
					Next editor
					Frame.GetController(Of RefreshController)().RefreshAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of RecordsNavigationController)().PreviousObjectAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of RecordsNavigationController)().NextObjectAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of DetailViewController)().SaveAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of DetailViewController)().SaveAndCloseAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of DetailViewController)().CancelAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of DeleteObjectsViewController)().DeleteAction.Active(ActiveKeySearchObject) = False
					Frame.GetController(Of NewObjectViewController)().NewObjectAction.Active(ActiveKeySearchObject) = False
				End If
				SearchObjectAction.Active(ActiveKeySearchObject) = False
			End If
		End Sub
		Private Sub InitSearchObjectActionItems()
			SearchObjectAction.BeginUpdate()
			SearchObjectAction.Items.Clear()
			Dim genericType As Type = GetType(SearchObjectBase(Of )).MakeGenericType(View.ObjectTypeInfo.Type)
			Dim imageName As String = View.Model.ImageName
			For Each typeInfo As ITypeInfo In XafTypesInfo.Instance.PersistentTypes
				If genericType.IsAssignableFrom(typeInfo.Type) AndAlso (Not typeInfo.IsAbstract) Then
					Dim item As New ChoiceActionItem(CaptionHelper.GetClassCaption(typeInfo.FullName), typeInfo.Type)
					item.ImageName = imageName
					SearchObjectAction.Items.Add(item)
				End If
			Next typeInfo
			SearchObjectAction.EndUpdate()
		End Sub
	End Class
End Namespace