using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly TaskManagementContext _context;
    // private readonly ICacheService _cacheService;

    // Pass ICacheService cacheService params to the constructor if caching needed
    public GenericRepository(TaskManagementContext context)
    {
        _context = context;
        // _cacheService = cacheService;
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        // string cacheKey = $"{typeof(TEntity).Name}:{id}";
        // return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        // {
        return await _context.Set<TEntity>().FindAsync(id);
        // }, TimeSpan.FromMinutes(10));
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        // string cacheKey = $"{typeof(TEntity).Name}:All";
        // return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        // {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        // }, TimeSpan.FromMinutes(10));
    }


    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public IQueryable<TEntity> Query()
    {
        return _context.Set<TEntity>().AsQueryable();
    }
}