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
			scaSearchObjectActionCore.ItemHierarchyType = ChoiceActionItemHierarchyType.List
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
			Dim os As ObjectSpace = Application.CreateObjectSpace()
			Dim obj As Object = os.CreateObject(CType(args.SelectedChoiceActionItem.Data, Type))
			Dim dv As DetailView = Application.CreateDetailView(os, obj)
			args.ShowViewParameters.CreatedView = dv
		End Sub
		Protected Overrides Overloads Sub OnActivated()
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
			Dim imageName As String = View.Info.GetAttributeValue("ImageName")
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