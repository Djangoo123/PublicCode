
using CompagnyTools.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CompagnyTools.GenericRepository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly EFCoreContext context;
        private readonly DbSet<TEntity> dbSet;

        public GenericRepository(EFCoreContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrEmpty(includeProperty))
                    {
                        query = query.Include(includeProperty);
                    }
                }
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, Boolean>> where)
        {
            return await dbSet.FirstOrDefaultAsync<TEntity>(where);
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Func<TEntity, bool> where)
        {
            return await Task.Run(() => dbSet.Where(where));
        }

        public virtual void Add(TEntity entityToAdd)
        {
            try
            {
                context.Entry(entityToAdd).State = EntityState.Added;
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }   

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                context.Entry(entityToUpdate).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void Update(TEntity entityToUpdate, TEntity newEntity)
        {
            context.Entry(entityToUpdate).CurrentValues.SetValues(newEntity);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            try
            {
                context.Entry(entityToDelete).State = EntityState.Deleted;
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }

        public virtual void AddList(IList<TEntity> listEntityToAdd)
        {
            try
            {
                foreach (var entityToAdd in listEntityToAdd)
                {
                    context.Entry(entityToAdd).State = EntityState.Added;
                }
                if (listEntityToAdd.Count != 0)
                    context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void UpdateList(IList<TEntity> listEntityToUpdate)
        {
            try
            {
                foreach (var entityToUpdate in listEntityToUpdate)
                {
                    context.Entry(entityToUpdate).State = EntityState.Modified;
                }
                if (listEntityToUpdate.Count != 0)
                    context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void DeleteList(IList<TEntity> listEntityToDelete)
        {
            try
            {
                foreach (var entityToDelete in listEntityToDelete)
                {
                    context.Entry(entityToDelete).State = EntityState.Deleted;
                }
                if (listEntityToDelete.Count != 0)
                    context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual void AddNew(TEntity entityToAdd)
        {

            dbSet.Add(entityToAdd);

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// fonction pour recuperer le context (p. ex. utilisation dans extension method)
        /// </summary>
        /// <returns></returns>
        public virtual EFCoreContext GetContext()
        {
            return context;
        }

        /// <summary>
        /// fonction pour recuperer le DbSet (p. ex. utilisation dans extension method)
        /// </summary>
        /// <returns></returns>
        public virtual DbSet<TEntity> GetDbSet()
        {
            return dbSet;
        }
    }
}
