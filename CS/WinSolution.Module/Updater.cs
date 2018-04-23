using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Updating;

namespace WinSolution.Module {
    public class Updater : ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitData();
        }
        private void InitData() {
            using (UnitOfWork uow = new UnitOfWork(Session.DataLayer)) {
                for (int i = 0; i < 100000; i++) {
                    Product product = new Product(uow);
                    product.Name = string.Format("Product{0}", i);
                    product.Description = string.Format("Description{0}", i);
                    product.Price = i % 10;
                }
                uow.CommitChanges();
            }
        }
    }
}