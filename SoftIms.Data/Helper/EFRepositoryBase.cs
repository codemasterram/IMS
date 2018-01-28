using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data
{
    public class EFRepositoryBase<T> : IRepository<T> where T : class
    {
        private DbContext _context;
        public EFRepositoryBase(DbContext context)
        {
            _context = context;
        }

        public IQueryable<T> Table()
        {
            return _context.Set<T>().AsQueryable();
        }

        public IQueryable<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Table().Where(predicate);
        }

        private string GetExceptionMessages(Exception ex)
        {
            if (ex == null) return string.Empty;
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg = GetExceptionMessages(ex.InnerException);
            }
            return msg;
        }



        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public List<string> TryDelete(int id, out string errorMessage)
        {
            errorMessage = null;
            var errors = new List<string>();
            try
            {
                var entity = GetById(id);
                if (entity != null)
                    Delete(entity);
            }
            catch (Exception ex)
            {
                if (GetExceptionMessages(ex).Contains("REFERENCE"))
                {
                    errorMessage = "This item is used in another module. Unable to delete this item.";
                }

                errors.Add(ex.Message);
            }
            return errors;
        }


        public int Create(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
        }

        public void CreateRange(IEnumerable<T> entity)
        {
            _context.Set<T>().AddRange(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Entry<T>(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _context.Set<T>().RemoveRange(entity);
        }

        public void Delete(int id)
        {
            T entity = GetById(id);
            Delete(entity);
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Add(entity);
        }

       
    }
}
