using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data
{
    public interface IGenericRepository<TEntity>
     where TEntity : class, IEntity
    {
        Task<int> Total();

        Task<IEnumerable<TEntity>> GetAll();

        Task<IEnumerable<TEntity>> GetPaginated(int page, int size);

        Task<TEntity> GetById(int id);
        Task<TEntity> GetByIndex(int index);

        Task<int> Create(TEntity entity);

        Task Update(int id, TEntity entity);

        Task Delete(int id);
    }
}
