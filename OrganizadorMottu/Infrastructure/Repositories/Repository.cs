using Microsoft.EntityFrameworkCore;
using OrganizadorMottu.Infrastructure.Context;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace OrganizadorMottu.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        // Permite via procedure.
        public async Task ExecutarProcedureAsync(string procedureName, Dictionary<string, object> parametros)
        {
            var connectionString = _context.Database.GetConnectionString();
            using var conn = new OracleConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var param in parametros)
                cmd.Parameters.Add(param.Key, param.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // Permite via procedure.
        public async Task<TResult?> ExecutarFunctionAsync<TResult>(string functionName)
        {
            var connectionString = _context.Database.GetConnectionString();
            using var conn = new OracleConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new OracleCommand(functionName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("RETURN_VALUE", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync();

            var result = cmd.Parameters["RETURN_VALUE"].Value;
            return (TResult?)Convert.ChangeType(result, typeof(TResult));
        }
    }
}
