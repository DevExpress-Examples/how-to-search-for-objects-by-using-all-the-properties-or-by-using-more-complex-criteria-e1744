using DevExpress.ExpressApp.Xpo;
// Developer Express Code Central Example:
// How to search for objects by using all the properties or by using more complex criteria
// 
// This example provides a possible workaround for the suggestion. The
// Dennis.Search.Win module, shown in the example, provides API to create a
// non-persistent object that can be used to search by properties of a business
// class. Such a non-persistent search-object should implement the ISearchObject
// contract. By default, it's supported by the abstract and generic
// SearchObjectBase class in the module. Search-objects are shown in a Detail View
// with the help of the SearchObjectViewController. To compose a search criteria, a
// specific SearchObjectPropertyEditor is used (it contains the Search and Clear
// buttons). An example use of the implemented module is illustrated in the
// WinSolution.Module.Win module. There is a ProductSearchObject class that is
// inherited from the base SearchObjectBase class. If necessary, you can create
// several search objects for one business class. In UI, all search objects will be
// listed in a drop-down list in the toolbar of a View of your business class.
// Download and run the attached example to see how this works in action. (Take
// special note of the SearchObjectBase class implementation, because it
// asynchronously loads search results of a specific type from the data store into
// the Search Results nested List View.) See Also:
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1744

using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;

namespace WinSolution.Win {
    public partial class WinSolutionWindowsFormsApplication : WinApplication {
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
        public WinSolutionWindowsFormsApplication() {
            InitializeComponent();
        }

        private void WinSolutionWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "The automatic update is disabled, because the application was started without debugging.\r\n" +
                    "You should start the application under Visual Studio, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.");
            }
#endif
        }
    }
}
