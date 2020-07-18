using Cashback.Data;
using Cashback.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cashback.Repository
{
    public class UserRepository : IUserRepository
    {
        protected MongoDbContext Context { get; }

        public UserRepository(MongoDbContext context)
        {
            Context = context;
        }

        public async Task DeleteAsync(User user)
        {
            await Context.DeleteAsync<User>(user.Id);
        }    

        public async Task<User> GetById(string id)
        {
            return await Context.GetByIdAsync<User>(id);
        }        

        public async Task<User> GetByEmail(string email)
        {
             var filter = Builders<User>.Filter.Eq(x => x.Email, email);                        
            return await Context.GetAsync(filter);
        }        

        public async Task<ICollection<User>> GetItemsAsync()
        {
            return await Context.GetItemsAsync(Builders<User>.Filter.Empty, 
                "Id", "Name", "Role", "Cpf", "Email", "CreatedAt");
        }

        public async Task<User> InsertAsync(User user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            user.CreatedAt = DateTimeOffset.UtcNow;

            return await Context.InsertAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await Context.UpdateAsync(user.Id, user);
        }
    }
}