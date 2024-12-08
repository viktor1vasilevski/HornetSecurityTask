using Core;
using EntityModels.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Data.Repositories;

public class SqlGenericRepository<TEntity, TContext> : IGenericRepository<TEntity> where TEntity : class where TContext : IDbContext
{
    internal IDbContext context;
    internal DbSet<TEntity> dbSet;
    public SqlGenericRepository(IDbContext context)
    {
        this.context = context;
        dbSet = context.Set<TEntity>();
    }

    public void Delete(TEntity entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            dbSet.Attach(entity);
        }
        dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
        }
        dbSet.RemoveRange(entities);
    }

    public bool Exists(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = dbSet;
        return query.Any(filter ?? (_ => true));
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = dbSet;
        return await query.AnyAsync(filter ?? (_ => true));
    }

    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (include != null)
        {
            query = include(query);
        }
        return [.. query];
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (include != null)
        {
            query = include(query);
        }
        return await query.ToListAsync();
    }

    public IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (include != null)
        {
            query = include(query);
        }
        return query;
    }

    public TEntity GetByID(object id)
    {
        IQueryable<TEntity> query = dbSet;
        return dbSet.Find(id);
    }

    public TEntity Insert(TEntity entity)
    {
        dbSet.Add(entity);
        return entity;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
        return entity;
    }

    public void InsertRange(IEnumerable<TEntity> entities)
    {
        dbSet.AddRange(entities);
    }

    public TEntity Update(TEntity entity)
    {
        var entry = context.Entry(entity);
        context.Entry(entity).State = EntityState.Detached;
        context.Entry(entity).State = EntityState.Modified;
        return entity;
    }
}
