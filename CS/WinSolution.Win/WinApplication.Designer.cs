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

namespace WinSolution.Win {
    partial class WinSolutionWindowsFormsApplication {
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
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule();
            this.module3 = new WinSolution.Module.WinSolutionModule();
            this.module4 = new WinSolution.Module.Win.WinSolutionWindowsFormsModule();
            this.module5 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.module7 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.searchModule1 = new Dennis.Search.Win.SearchModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module1
            // 
            this.module1.AdditionalExportedTypes.Add(typeof(DevExpress.Xpo.XPObjectType));
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // WinSolutionWindowsFormsApplication
            // 
            this.ApplicationName = "WinSolution";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.module7);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.searchModule1);
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.WinSolutionWindowsFormsApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private WinSolution.Module.WinSolutionModule module3;
        private WinSolution.Module.Win.WinSolutionWindowsFormsModule module4;
        private DevExpress.ExpressApp.Validation.ValidationModule module5;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule module7;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private Dennis.Search.Win.SearchModule searchModule1;
    }
}
