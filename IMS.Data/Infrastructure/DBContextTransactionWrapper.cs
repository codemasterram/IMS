using System.Data.Entity;

namespace IMS.Data.Infrastructure
{
    public class DbContextTransactionWrapper
    {
        private readonly DbContextTransaction _tx;

        public DbContextTransactionWrapper(DbContextTransaction tx)
        {
            _tx = tx;
        }

        public void Commit()
        {
            _tx.Commit();
        }

        public void Rollback()
        {
            _tx.Rollback();
        }

        public void Dispose()
        {
            _tx.Dispose();
        }
    }
}
