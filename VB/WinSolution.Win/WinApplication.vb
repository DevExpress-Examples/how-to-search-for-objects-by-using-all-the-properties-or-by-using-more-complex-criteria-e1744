Imports DevExpress.ExpressApp.Xpo
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
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.ExpressApp.Win
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp

Namespace WinSolution.Win
	Partial Public Class WinSolutionWindowsFormsApplication
		Inherits WinApplication
        Protected Overrides Sub CreateDefaultObjectSpaceProvider(ByVal args As CreateCustomObjectSpaceProviderEventArgs)
            args.ObjectSpaceProvider = New XPObjectSpaceProvider(args.ConnectionString, args.Connection)
        End Sub
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub WinSolutionWindowsFormsApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles MyBase.DatabaseVersionMismatch
#If EASYTEST Then
			e.Updater.Update()
			e.Handled = True
#Else
			If System.Diagnostics.Debugger.IsAttached Then
				e.Updater.Update()
				e.Handled = True
			Else
				Throw New InvalidOperationException("The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application." & Constants.vbCrLf & "The automatic update is disabled, because the application was started without debugging." & Constants.vbCrLf & "You should start the application under Visual Studio, or modify the " & "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " & "or manually create a database using the 'DBUpdater' tool.")
			End If
#End If
		End Sub
	End Class
End Namespace
