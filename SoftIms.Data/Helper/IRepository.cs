using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SoftIms.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table();
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        T GetById(int id);
        int Create(T entity);
        void Attach(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        /// <summary>
        /// if not successed to delete returns the error message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<string> TryDelete(int id, out string message);
    }
}
