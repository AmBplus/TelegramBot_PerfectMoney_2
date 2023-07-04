using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PefectMoney.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Data
{
    public interface ITelContext : IDisposable, IAsyncDisposable , IDbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<BankCard> BankCards { get; set; }

    }
    public interface IDbContext
    {
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new());
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
    

        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
        public DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;
        public EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        public EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        public EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
        public EntityEntry Add(object entity);
        public ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
        public EntityEntry Attach(object entity);
        public EntityEntry Update(object entity);
        public EntityEntry Remove(object entity);
        public void AddRange(params object[] entities);
        public Task AddRangeAsync(params object[] entities);
        public void AttachRange(params object[] entities);
        public void UpdateRange(params object[] entities);
        public void RemoveRange(params object[] entities);
        public void AddRange(IEnumerable<object> entities);

        public Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default(CancellationToken));
        public void AttachRange(IEnumerable<object> entities);

        public void UpdateRange(IEnumerable<object> entities);
        public void RemoveRange(IEnumerable<object> entities);
        public TEntity? Find<TEntity>(params object?[]? keyValues) where TEntity : class;
        public object? Find(Type entityType, params object?[]? keyValues);

        public ValueTask<object?> FindAsync(Type entityType, params object?[]? keyValues);
        public ValueTask<object?> FindAsync(Type entityType, object?[]? keyValues, CancellationToken cancellationToken);
        public ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class;
        public ValueTask<TEntity?> FindAsync<TEntity>(object?[]? keyValues, CancellationToken cancellationToken) where TEntity : class;
        public IQueryable<TResult> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression);
    }
}
