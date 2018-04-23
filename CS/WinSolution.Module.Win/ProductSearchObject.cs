using System;
using DevExpress.Xpo;
using Dennis.Search.Win;
using DevExpress.Persistent.Base;

namespace WinSolution.Module.Win {
    [NonPersistent]
    [System.ComponentModel.DisplayName("Complex search by products")]
    [ImageName("Attention")]
    public class ProductSearchObject : SearchObjectBase<Product> {
        public ProductSearchObject(Session session) : base(session) { }
        public override void Reset() {
            Price = null;
            Name = null;
            //Dennis: it's very important to reset searched properties before the base method call.
            base.Reset();
        }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        private decimal? _Price;
        //Dennis: it's also recommended to use nullable properties here.
        public decimal? Price {
            get { return _Price; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }
    }
}