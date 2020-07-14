using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cashback.Data
{
    public class MongoDbContext//<TEntity> : IRepository<TEntity, string> where TEntity : IEntityBase
    {
        #region [ Fields ]

        private readonly IMongoDatabase _database;

        #endregion [ Fields ]

        #region [ Constructors ]
       
        public MongoDbContext(string connectionString, string database)
        {
            if (string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionString não foi informada!");
            }

            ConventionPack.UseConventionMongo();
            _database = new MongoClient(connectionString).GetDatabase(database);

            if (_database == null)
            {
                throw new Exception("Não foi possível conectar no MongoDB!");
            }
        }

        #endregion [ Constructors ]

        #region [ Properties ]

        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(CollectionName<T>());
        }

        #endregion [ Properties ]

        #region [ Public Methods ]

        public async Task<T> InsertAsync<T>(T entity)
        {
            await GetCollection<T>().InsertOneAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync<T>(string id, T entity)
        {
            var _id = ObjectId.Parse(id);
            return await GetCollection<T>().FindOneAndReplaceAsync(Builders<T>.Filter.Eq("_id", _id), entity);
        }

        public async Task<DeleteResult> DeleteAsync<T>(string id)
        {
            var _id = ObjectId.Parse(id);
            return await GetCollection<T>().DeleteOneAsync(Builders<T>.Filter.Eq("_id", _id));
        }

        public async Task<T> DeleteAsync<T>(FilterDefinition<T> filter)
        {
            return await GetCollection<T>().FindOneAndDeleteAsync<T>(filter);
        }

        public async Task<T> GetByIdAsync<T>(string id)
        {
            var _id = ObjectId.Parse(id);
            return await GetCollection<T>()
                .Find(Builders<T>.Filter.Eq("_id", _id))
                .FirstOrDefaultAsync();
        }        
        public async Task<T> GetAsync<T>(FilterDefinition<T> filter, params string[] columns)
        {
            return await GetCollection<T>()
                .Find(filter)
                .Project<T>(GetProjection<T>(columns))
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<T>> GetItemsAsync<T>(FilterDefinition<T> where, params string[] columns)
        {
            return await GetCollection<T>()
                .Find(where)
                .Project<T>(GetProjection<T>(columns))
                .ToListAsync();
        }

        #endregion [ Public Methods ]

        #region [ Private Methods ]

        private string CollectionName<T>()
        {
            var collection = typeof(T).Name;
            return ToCamelCase(collection);
        }

        private string ToCamelCase(string value)
        {
            return $"{char.ToLowerInvariant(value[0])}{value.Substring(1)}";
        }

        private ProjectionDefinition<T> GetProjection<T>(params string[] columns)
        {
            var projections = new List<ProjectionDefinition<T>>();
            if (columns != null && columns.Length > 0)
            {
                projections.AddRange(columns.Select(entity => Builders<T>.Projection.Include(entity)));
            }

            return Builders<T>.Projection.Combine(projections.ToArray());
        }

        #endregion [ Private Methods ]
    }
}