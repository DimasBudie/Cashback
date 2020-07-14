using Cashback.Data;
using Cashback.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cashback.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        protected MongoDbContext Context { get; }

        public PurchaseRepository(MongoDbContext context)
        {
            Context = context;
        }

        public async Task DeleteAsync(Purchase purchase)
        {
            await Context.DeleteAsync<Purchase>(purchase.Id);
        }    

        public async Task<Purchase> GetById(string id)
        {
            return await Context.GetByIdAsync<Purchase>(id);
        }

        public async Task<ICollection<Purchase>> GetByCpf(string Cpf)
        {
            return await Context.GetItemsAsync(Builders<Purchase>.Filter.Eq(c => c.Cpf, Cpf));
        }

        public async Task<ICollection<Purchase>> GetItemsAsync()
        {
            return await Context.GetItemsAsync(Builders<Purchase>.Filter.Empty, 
                "Id", "CreatedAt", "Code", "Cpf", "Email", "Value", "Status");
        }

        public async Task<Purchase> InsertAsync(Purchase purchase)
        {
            purchase.Id = ObjectId.GenerateNewId().ToString();
            purchase.CreatedAt = DateTimeOffset.UtcNow;

            return await Context.InsertAsync(purchase);
        }

        public async Task<Purchase> UpdateAsync(Purchase purchase)
        {
            return await Context.UpdateAsync(purchase.Id, purchase);
        }
    }
}