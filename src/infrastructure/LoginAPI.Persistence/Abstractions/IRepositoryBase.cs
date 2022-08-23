using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LoginAPI.Persistence.Abstractions
{
    public interface IRepositoryBase<T>
    {
        public Task<List<T>> GetAll();
        public Task<List<T>> GetByCondition(Expression<Func<T, bool>> expression);
        public void Create(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}