using CleanArchMVC.Domain.Entities;
using CleanArchMVC.Domain.Interfaces;
using CleanArchMVC.Infra.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchMVC.Infra.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext _categoryContext;
        public CategoryRepository(ApplicationDbContext context)
        {
            _categoryContext = context;
        }
        public async Task<Category> CreateAsync(Category category)
        {
            using (var dbTrans = _categoryContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _categoryContext.Add(category);
                    await _categoryContext.SaveChangesAsync();
                    dbTrans.Commit();
                }
                catch
                {
                    dbTrans.Rollback();                    
                }
                return category;
            }
        }

        public async Task<Category> GetByIdAsync(int? id)
        {
            return await _categoryContext.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _categoryContext.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            using (var dbTrans = _categoryContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _categoryContext.Update(category);
                    await _categoryContext.SaveChangesAsync();
                    dbTrans.Commit();
                }
                catch
                {
                    dbTrans.Rollback();
                }
                return category;
            }
        }

        public async Task<Category> RemoveAsync(Category category)
        {
            using (var dbTrans = _categoryContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    _categoryContext.Remove(category);
                    await _categoryContext.SaveChangesAsync();
                    dbTrans.Commit();
                }
                catch
                {
                    dbTrans.Rollback();
                }
                return category;
            }
        }
    }
}
