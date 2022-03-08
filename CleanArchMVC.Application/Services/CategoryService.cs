using AutoMapper;
using CleanArchMVC.Application.DTOs;
using CleanArchMVC.Application.Interfaces;
using CleanArchMVC.Domain.Entities;
using CleanArchMVC.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchMVC.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ??
                throw new ArgumentNullException(nameof(categoryRepository));

            _mapper = mapper;
        }


        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categoriesEntity = await _categoryRepository.GetCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categoriesEntity);
        }

        public async Task<CategoryDTO> GetByIdAsync(int? id)
        {
            var categoriesEntity = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryDTO>(categoriesEntity);
        }



        public async Task AddAsync(CategoryDTO categoryDto)
        {
            var categoriesEntity = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.CreateAsync(categoriesEntity);
        }

        public async Task UpdateAsync(CategoryDTO categoryDto)
        {
            var categoriesEntity = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.UpdateAsync(categoriesEntity);
        }

        public async Task RemoveAsync(int? id)
        {
            var categoriesEntity = _categoryRepository.GetByIdAsync(id).Result;
            await _categoryRepository.RemoveAsync(categoriesEntity);
        }


    }
}
