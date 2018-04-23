Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports Dennis.Search.Win
Imports DevExpress.Persistent.Base

Namespace WinSolution.Module.Win
	<NonPersistent, System.ComponentModel.DisplayName("Complex search by products"), ImageName("Attention")> _
	Public Class ProductSearchObject
		Inherits SearchObjectBase(Of Product)
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Overrides Sub Reset()
			Price = Nothing
			Name = Nothing
			'Dennis: it's very important to reset searched properties before the base method call.
			MyBase.Reset()
		End Sub
		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Name", _Name, value)
			End Set
		End Property
		Private _Price As Nullable(Of Decimal)
		'Dennis: it's also recommended to use nullable properties here.
		Public Property Price() As Nullable(Of Decimal)
			Get
				Return _Price
			End Get
			Set(ByVal value As Nullable(Of Decimal))
				SetPropertyValue("Price", _Price, value)
			End Set
		End Property
	End Class
End Namespace