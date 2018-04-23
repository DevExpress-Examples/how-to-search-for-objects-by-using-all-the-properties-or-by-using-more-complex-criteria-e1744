Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports DevExpress.ExpressApp.Updating

Namespace WinSolution.Module
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal session As Session, ByVal currentDBVersion As Version)
			MyBase.New(session, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()
			InitData()
		End Sub
		Private Sub InitData()
			Using uow As New UnitOfWork(Session.DataLayer)
				For i As Integer = 0 To 99999
					Dim product As New Product(uow)
					product.Name = String.Format("Product{0}", i)
					product.Description = String.Format("Description{0}", i)
					product.Price = i Mod 10
				Next i
				uow.CommitChanges()
			End Using
		End Sub
	End Class
End Namespace