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
            //var tt = await _productContext.Database.ExecuteSqlRawAsync("select * from Products where id = 2");            
            //using (var command = _productContext.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = "select * from Products where id = 2";
            //    _productContext.Database.OpenConnection();
            //    using (var result = command.ExecuteReader())
            //    {
            //        var tt = result;
            //    }
            //}

            //return await _productContext.Products.AsNoTracking().Include(c => c.Category)
            //    .SingleOrDefaultAsync(p => p.Id == id);


            Product product = null;

            using (var dbTrans = _productContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                product = await _productContext.Products.AsNoTracking().Include(c => c.Category).SingleOrDefaultAsync(p => p.Id == id);

                dbTrans.Commit();
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            IEnumerable<Product> products = null;
            using (var dbTrans = _productContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                products = await _productContext.Products.AsNoTracking().ToListAsync();

                dbTrans.Commit();

                if (products == null)
                    throw new Exception("Lista não encontrada");
            }

            return products;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            using (var dbTrans = _productContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _productContext.Add(product);
                    await _productContext.SaveChangesAsync();

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                }
            }
            
            return product;
        }

        public async Task<Product> RemoveAsync(Product product)
        {
            using (var dbTrans = _productContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _productContext.Remove(product);
                    await _productContext.SaveChangesAsync();

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                }
            }

            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            using (var dbTrans = _productContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _productContext.Update(product);
                    await _productContext.SaveChangesAsync();

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                }
            }

            return product;
        }
    }
}
