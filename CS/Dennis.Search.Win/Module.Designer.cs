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

namespace Dennis.Search.Win {
    partial class SearchModule {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            // 
            // SearchModule
            // 
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));

        }

        #endregion
    }
}
