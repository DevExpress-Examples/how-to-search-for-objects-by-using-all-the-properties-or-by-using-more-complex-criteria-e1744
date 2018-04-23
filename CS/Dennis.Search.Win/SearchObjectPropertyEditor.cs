using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Controls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;

namespace Dennis.Search.Win.Editors {
    public class SearchObjectControl : XtraUserControl, IXtraResizableControl {
        private SimpleButton btnSearchCore;
        private SimpleButton btnClearCore;
        private PropertyEditor editorCore;
        private Form formCore;
        public SearchObjectControl(PropertyEditor editor) {
            editorCore = editor;
            Size = new Size(0, 25);
            BorderStyle = BorderStyle.None;
            HandleCreated += delegate(object sender, EventArgs args) {
                Form.Activated += Form_Activated;
            };
            SearchObject.SearchStart += SearchObject_SearchStart;
            SearchObject.SearchComplete += SearchObject_SearchComplete;

            btnSearchCore = new SimpleButton();
            btnSearchCore.Location = new Point(0, 0);
            btnSearchCore.Size = new Size(75, 22);
            btnSearchCore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnSearchCore.Text = Properties.Resources.SearchButtonText;
            btnSearchCore.Click += btnSearchCore_Click;
            Controls.Add(btnSearchCore);

            btnClearCore = new SimpleButton();
            btnClearCore.Location = new Point(82, 0);
            btnClearCore.Size = new Size(75, 22);
            btnClearCore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnClearCore.Text = Properties.Resources.ClearButtonText;
            btnClearCore.Click += btnClearCore_Click;
            Controls.Add(btnClearCore);
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (Form != null)
                    Form.Activated -= Form_Activated;
                if (SearchObject != null) {
                    SearchObject.SearchStart -= SearchObject_SearchStart;
                    SearchObject.SearchComplete -= SearchObject_SearchComplete;
                }
                btnSearchCore.Click -= btnSearchCore_Click;
                btnClearCore.Click -= btnClearCore_Click;
            }
            base.Dispose(disposing);
        }
        private void Form_Activated(object sender, EventArgs e) {
            Form.AcceptButton = SearchButton;
        }
        private void btnSearchCore_Click(object sender, EventArgs e) {
            SearchObject.Search();
        }
        private void btnClearCore_Click(object sender, EventArgs e) {
            SearchObject.Reset();
        }
        private void SearchObject_SearchStart(object sender, SearchStartEventArgs e) {
            UpdateState();
        }
        private void SearchObject_SearchComplete(object sender, SearchCompleteEventArgs e) {
            UpdateState();
        }
        public void UpdateState() { Enabled = !SearchObject.IsSearching; }
        public SimpleButton SearchButton { get { return btnSearchCore; } }
        public SimpleButton ClearButton { get { return btnClearCore; } }
        protected PropertyEditor Editor { get { return editorCore; } }
        protected ISearchObject SearchObject { get { return Editor != null ? Editor.CurrentObject as ISearchObject : null; } }
        protected Form Form {
            get {
                if (formCore == null)
                    formCore = FindForm();
                return formCore;
            }
        }
        #region IXtraResizableControl Members
        protected virtual void RaiseChanged() {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }
        public event EventHandler Changed;
        public bool IsCaptionVisible { get { return false; } }
        public Size MaxSize { get { return new Size(0, SearchButton.Height); } }
        public Size MinSize { get { return new Size(0, SearchButton.Height); } }
        #endregion
    }
    [PropertyEditor(typeof(GroupOperator))]
    public class SearchObjectPropertyEditor : WinPropertyEditor {
        public SearchObjectPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
        protected override object CreateControlCore() { return new SearchObjectControl(this); }
        public new SearchObjectControl Control { get { return (SearchObjectControl)base.Control; } }
        public override bool IsCaptionVisible { get { return false; } }
    }
}