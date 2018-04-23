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
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.ExpressApp
Imports DevExpress.XtraEditors
Imports DevExpress.Data.Filtering
Imports DevExpress.Utils.Controls
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Win.Editors
Imports DevExpress.ExpressApp.Model

Namespace Dennis.Search.Win.Editors
	Public Class SearchObjectControl
		Inherits XtraUserControl
		Implements IXtraResizableControl
		Private btnSearchCore As SimpleButton
		Private btnClearCore As SimpleButton
		Private editorCore As PropertyEditor
		Private formCore As Form
		Public Sub New(ByVal editor As PropertyEditor)
			editorCore = editor
			Size = New Size(0, 25)
			BorderStyle = BorderStyle.None
			AddHandler HandleCreated, Function(sender, args) AnonymousMethod1(sender, args)
			AddHandler SearchObject.SearchStart, AddressOf SearchObject_SearchStart
			AddHandler SearchObject.SearchComplete, AddressOf SearchObject_SearchComplete

			btnSearchCore = New SimpleButton()
			btnSearchCore.Location = New Point(0, 0)
			btnSearchCore.Size = New Size(75, 22)
			btnSearchCore.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left
			btnSearchCore.Text = My.Resources.SearchButtonText
			AddHandler btnSearchCore.Click, AddressOf btnSearchCore_Click
			Controls.Add(btnSearchCore)

			btnClearCore = New SimpleButton()
			btnClearCore.Location = New Point(82, 0)
			btnClearCore.Size = New Size(75, 22)
			btnClearCore.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left
			btnClearCore.Text = My.Resources.ClearButtonText
			AddHandler btnClearCore.Click, AddressOf btnClearCore_Click
			Controls.Add(btnClearCore)
		End Sub
		
		Private Function AnonymousMethod1(ByVal sender As Object, ByVal args As EventArgs) As Boolean
			AddHandler Form.Activated, AddressOf Form_Activated
			Return True
		End Function
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Form IsNot Nothing Then
					RemoveHandler Form.Activated, AddressOf Form_Activated
				End If
				If SearchObject IsNot Nothing Then
					RemoveHandler SearchObject.SearchStart, AddressOf SearchObject_SearchStart
					RemoveHandler SearchObject.SearchComplete, AddressOf SearchObject_SearchComplete
				End If
				RemoveHandler btnSearchCore.Click, AddressOf btnSearchCore_Click
				RemoveHandler btnClearCore.Click, AddressOf btnClearCore_Click
			End If
			MyBase.Dispose(disposing)
		End Sub
		Private Sub Form_Activated(ByVal sender As Object, ByVal e As EventArgs)
			Form.AcceptButton = SearchButton
		End Sub
		Private Sub btnSearchCore_Click(ByVal sender As Object, ByVal e As EventArgs)
			SearchObject.Search()
		End Sub
		Private Sub btnClearCore_Click(ByVal sender As Object, ByVal e As EventArgs)
			SearchObject.Reset()
		End Sub
		Private Sub SearchObject_SearchStart(ByVal sender As Object, ByVal e As SearchStartEventArgs)
			UpdateState()
		End Sub
		Private Sub SearchObject_SearchComplete(ByVal sender As Object, ByVal e As SearchCompleteEventArgs)
			UpdateState()
		End Sub
		Public Sub UpdateState()
			Enabled = Not SearchObject.IsSearching
		End Sub
		Public ReadOnly Property SearchButton() As SimpleButton
			Get
				Return btnSearchCore
			End Get
		End Property
		Public ReadOnly Property ClearButton() As SimpleButton
			Get
				Return btnClearCore
			End Get
		End Property
		Protected ReadOnly Property Editor() As PropertyEditor
			Get
				Return editorCore
			End Get
		End Property
		Protected ReadOnly Property SearchObject() As ISearchObject
			Get
				Return If(Editor IsNot Nothing, TryCast(Editor.CurrentObject, ISearchObject), Nothing)
			End Get
		End Property
		Protected ReadOnly Property Form() As Form
			Get
				If formCore Is Nothing Then
					formCore = FindForm()
				End If
				Return formCore
			End Get
		End Property
		#Region "IXtraResizableControl Members"
		Protected Overridable Sub RaiseChanged()
			RaiseEvent Changed(Me, EventArgs.Empty)
		End Sub
        Public Event Changed As EventHandler Implements IXtraResizableControl.Changed
        Public ReadOnly Property IsCaptionVisible() As Boolean Implements IXtraResizableControl.IsCaptionVisible
            Get
                Return False
            End Get
        End Property
        Public ReadOnly Property MaxSize() As Size Implements IXtraResizableControl.MaxSize
            Get
                Return New Size(0, SearchButton.Height)
            End Get
        End Property
        Public ReadOnly Property MinSize() As Size Implements IXtraResizableControl.MinSize
            Get
                Return New Size(0, SearchButton.Height)
            End Get
        End Property
		#End Region
	End Class
	<PropertyEditor(GetType(GroupOperator))> _
	Public Class SearchObjectPropertyEditor
		Inherits WinPropertyEditor
		Public Sub New(ByVal objectType As Type, ByVal info As IModelMemberViewItem)
			MyBase.New(objectType, info)
		End Sub
		Protected Overrides Function CreateControlCore() As Object
			Return New SearchObjectControl(Me)
		End Function
		Public Shadows ReadOnly Property Control() As SearchObjectControl
			Get
				Return CType(MyBase.Control, SearchObjectControl)
			End Get
		End Property
        Public Overrides ReadOnly Property IsCaptionVisible() As Boolean
            Get
                Return False
            End Get
        End Property
	End Class
End Namespace