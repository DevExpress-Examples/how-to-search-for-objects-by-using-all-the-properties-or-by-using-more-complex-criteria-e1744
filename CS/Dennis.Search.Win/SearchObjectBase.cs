using DevExpress.ExpressApp.Model;
using System;
using DevExpress.Xpo;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model.NodeWrappers;

namespace Dennis.Search.Win {
    public interface ISearchObject {
        event EventHandler<SearchStartEventArgs> SearchStart;
        event EventHandler<SearchCompleteEventArgs> SearchComplete;
        GroupOperator Criteria { get;set;}
        ICollection SearchResults { get;}
        bool IsSearching { get;}
        void Search();
        void Reset();
    }
    public class SearchStartEventArgs : CancelEventArgs {
        private CriteriaOperator criteriaCore;
        public SearchStartEventArgs(CriteriaOperator criteria) {
            criteriaCore = criteria;
        }
        public CriteriaOperator Criteria {
            get { return criteriaCore; }
            set { criteriaCore = value; }
        }
    }
    public class SearchCompleteEventArgs : EventArgs {
        private ICollection searchResultsCore;
        private Exception errorCore;
        public SearchCompleteEventArgs(ICollection searchResults, Exception error) {
            searchResultsCore = searchResults;
            errorCore = error;
        }
        public Exception Error {
            get { return errorCore; }
        }
        public ICollection SearchResults {
            get { return searchResultsCore; }
        }
    }
    [NonPersistent]
    public abstract class SearchObjectBase<T> : XPBaseObject, ISearchObject where T : IXPSimpleObject {
        private XPCollection<T> searchResultsCore;
        private GroupOperator criteriaCore;
        private bool isSearchingCore;
        protected const string SearchObjectPropertyEditorFullName = "Dennis.Search.Win.Editors";
        protected const string SearchResultsCollectionName = "SearchResults";
        protected const BinaryOperatorType BinaryCriteriaType = BinaryOperatorType.Like;
        protected const GroupOperatorType GroupCriteriaType = GroupOperatorType.And;
        protected readonly CriteriaOperator EmptyCollectionCriteria = CriteriaOperator.Parse("1=0");
        public SearchObjectBase(Session session) : base(session) { }
        [ModelDefault("PropertyEditorType", SearchObjectPropertyEditorFullName)]
        public GroupOperator Criteria {
            get {
                if (ReferenceEquals(criteriaCore, null))
                    criteriaCore = new GroupOperator(GroupCriteriaType, EmptyCollectionCriteria);
                return criteriaCore;
            }
            set { SetPropertyValue("Criteria", ref criteriaCore, value); }
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading && !IsSaving)
                if (propertyName != "Criteria" && propertyName != SearchResultsCollectionName)
                    UpdateCriteria(propertyName, newValue);
        }
        protected virtual void UpdateCriteria(string propertyName, object propertyValue) {
            bool updated = false;
            foreach (CriteriaOperator operand in Criteria.Operands) {
                BinaryOperator binaryOperator = operand as BinaryOperator;
                if (!ReferenceEquals(binaryOperator, null) && !ReferenceEquals(binaryOperator, EmptyCollectionCriteria))
                    if (((OperandProperty)binaryOperator.LeftOperand).PropertyName == propertyName) {
                        ((OperandValue)binaryOperator.RightOperand).Value = propertyValue;
                        updated = true;
                        break;
                    }
            }
            if (!updated)
                if (propertyValue is String) {
                    Criteria.Operands.Add(new BinaryOperator(propertyName, propertyValue, BinaryCriteriaType));
                } else {
                    Criteria.Operands.Add(new BinaryOperator(propertyName, propertyValue));
                }
        }
        [Browsable(false)]
        public bool IsSearching {
            get { return isSearchingCore; }
        }
        public XPCollection<T> SearchResults {
            get {
                if (searchResultsCore == null)
                    searchResultsCore = new XPCollection<T>(Session, EmptyCollectionCriteria, null);
                return searchResultsCore;
            }
        }
        public virtual void Reset() {
            criteriaCore = null;
            searchResultsCore.Criteria = EmptyCollectionCriteria;
        }
        ICollection ISearchObject.SearchResults {
            get { return SearchResults; }
        }
        public virtual void Search() {
            isSearchingCore = true;
            if (Criteria.Operands.Contains(EmptyCollectionCriteria))
                Criteria.Operands.Remove(EmptyCollectionCriteria);
            SearchStartEventArgs args = new SearchStartEventArgs(Criteria);
            OnSearchStart(args);
            if (!args.Cancel) {
                searchResultsCore.SuspendChangedEvents();
                searchResultsCore.Criteria = args.Criteria;
                searchResultsCore.LoadAsync(LoadSearchResultsCallback);
            }
        }
        private void LoadSearchResultsCallback(ICollection[] result, Exception exception) {
            try {
                if (exception != null)
                    throw new InvalidOperationException(string.Format("Cannot load search results for the {0} object type due to an error: {1}", typeof(T), exception.Message));
            } finally {
                isSearchingCore = false;
                OnSearchComplete(new SearchCompleteEventArgs(result != null && result.Length > 0 ? result[0] : null, exception));
                searchResultsCore.ResumeChangedEvents();
            }
        }
        protected virtual void OnSearchStart(SearchStartEventArgs args) {
            if (SearchStart != null)
                SearchStart(this, args);
        }
        protected virtual void OnSearchComplete(SearchCompleteEventArgs args) {
            if (SearchComplete != null)
                SearchComplete(this, args);
        }
        public event EventHandler<SearchStartEventArgs> SearchStart;
        public event EventHandler<SearchCompleteEventArgs> SearchComplete;
    }
}