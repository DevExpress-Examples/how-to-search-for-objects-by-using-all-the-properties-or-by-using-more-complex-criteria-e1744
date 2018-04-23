using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Dennis.Search.Win {
    public class SearchObjectViewController : ViewController {
        private const string ActiveKeySearchObject = "ISearchObject";
        private SingleChoiceAction scaSearchObjectActionCore;
        public const string SearchResultsCollectionName = "SearchResults";
        public SearchObjectViewController() {
            scaSearchObjectActionCore = new SingleChoiceAction(this, "SearchObject", PredefinedCategory.Search);
            scaSearchObjectActionCore.Execute += saSearchObjectActionCore_Execute;
            scaSearchObjectActionCore.Caption = Properties.Resources.SearchObjectActionText;
            scaSearchObjectActionCore.ImageName = "Action_Search";
            scaSearchObjectActionCore.ItemHierarchyType = ChoiceActionItemHierarchyType.List;
            scaSearchObjectActionCore.ItemType = SingleChoiceActionItemType.ItemIsOperation;
        }
        public SingleChoiceAction SearchObjectAction {
            get { return scaSearchObjectActionCore; }
        }
        private void saSearchObjectActionCore_Execute(object sender, SingleChoiceActionExecuteEventArgs args) {
            SearchObject(args);
        }
        protected virtual void SearchObject(SingleChoiceActionExecuteEventArgs args) {
            ObjectSpace os = Application.CreateObjectSpace();
            object obj = os.CreateObject((Type)args.SelectedChoiceActionItem.Data);
            DetailView dv = Application.CreateDetailView(os, obj);
            args.ShowViewParameters.CreatedView = dv;
        }
        protected override void OnActivated() {
            base.OnActivated();
            InitSearchObjectActionItems();
            if (typeof(ISearchObject).IsAssignableFrom(View.ObjectTypeInfo.Type)) {
                DetailView dv = View as DetailView;
                if (dv != null) {
                    foreach (ListPropertyEditor editor in dv.GetItems<ListPropertyEditor>()) {
                        if (editor.PropertyName == SearchResultsCollectionName) {
                            editor.Frame.GetController<LinkUnlinkController>().LinkAction.Active[ActiveKeySearchObject] = false;
                            editor.Frame.GetController<LinkUnlinkController>().UnlinkAction.Active[ActiveKeySearchObject] = false;
                            editor.Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active[ActiveKeySearchObject] = false;
                            editor.Frame.GetController<NewObjectViewController>().NewObjectAction.Active[ActiveKeySearchObject] = false;
                        }
                    }
                    Frame.GetController<RefreshController>().RefreshAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<RecordsNavigationController>().PreviousObjectAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<RecordsNavigationController>().NextObjectAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<DetailViewController>().SaveAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<DetailViewController>().SaveAndCloseAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<DetailViewController>().CancelAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active[ActiveKeySearchObject] = false;
                    Frame.GetController<NewObjectViewController>().NewObjectAction.Active[ActiveKeySearchObject] = false;
                }
                SearchObjectAction.Active[ActiveKeySearchObject] = false;
            }
        }
        private void InitSearchObjectActionItems() {
            SearchObjectAction.BeginUpdate();
            SearchObjectAction.Items.Clear();
            Type genericType = typeof(SearchObjectBase<>).MakeGenericType(View.ObjectTypeInfo.Type);
            string imageName = View.Info.GetAttributeValue("ImageName");
            foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                if (genericType.IsAssignableFrom(typeInfo.Type) && !typeInfo.IsAbstract) {
                    ChoiceActionItem item = new ChoiceActionItem(CaptionHelper.GetClassCaption(typeInfo.FullName), typeInfo.Type);
                    item.ImageName = imageName;
                    SearchObjectAction.Items.Add(item);
                }
            }
            SearchObjectAction.EndUpdate();
        }
    }
}