using CleanArchMVC.Domain.Entities;
using CleanArchMVC.Domain.Interfaces;
using CleanArchMVC.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchMVC.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ApplicationDbContext _productContext;

        public ProductRepository(ApplicationDbContext context)
        {
            _productContext = context;
        }


        public async Task<Product> GetByIdAsync(int? id)
        {
            //var tt = await _productContext.Database.ExecuteSqlRawAsync("select * from Products where id =2");

            
            //using (var command = _productContext.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = "select * from Products where id = 2";
            //    _productContext.Database.OpenConnection();
            //    using (var result = command.ExecuteReader())
            //    {
            //        var tt = result;
            //    }

            //}
            


                return await _productContext.Products.Include(c => c.Category)
                    .SingleOrDefaultAsync(p => p.Id == id);
        }

        //public async Task<IEnumerable<Product>> GetProductsAsync()
        //{
        //    return await _productContext.Products.ToListAsync();
        //}

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            IEnumerable<Product> produtos = null;
            using (var context = _productContext)
            {
                using (var dbTrans = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    produtos = await _productContext.Products.ToListAsync();

                    dbTrans.Commit();

                    if (produtos == null)
                        //throw new BrokenRuleException("Produto não encontrado");
                        throw new Exception("Lista não encontrada");
                }
            }

            return produtos;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _productContext.Add(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> RemoveAsync(Product product)
        {
            _productContext.Remove(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _productContext.Update(product);
            await _productContext.SaveChangesAsync();
            return product;
        }
    }
}
