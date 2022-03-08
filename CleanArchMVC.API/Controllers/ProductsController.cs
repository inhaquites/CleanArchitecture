using CleanArchMVC.Application.DTOs;
using CleanArchMVC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchMVC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IDistributedCache _distributedCache;

        public ProductsController(IProductService productService , IDistributedCache distributedCache)
        {
            _productService = productService;
            _distributedCache = distributedCache;
        }



        [HttpGet("redis")]        
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
        {
            var cacheKey = "productsList";
            string serializedProductsList;
            var productsList = new List<ProductDTO>();

            var redisProductsList = await _distributedCache.GetAsync(cacheKey);

            if(redisProductsList!=null)
            {
                serializedProductsList = Encoding.UTF8.GetString(redisProductsList);
                productsList = JsonConvert.DeserializeObject<List<ProductDTO>>(serializedProductsList);
            }
            else
            {
                productsList = (await _productService.GetProductsAsync()).ToList();
                serializedProductsList = JsonConvert.SerializeObject(productsList);
                redisProductsList = Encoding.UTF8.GetBytes(serializedProductsList);

                var options = new DistributedCacheEntryOptions()
                                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await _distributedCache.SetAsync(cacheKey, redisProductsList, options);
            }

            return Ok(productsList);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = await _productService.GetProductsAsync();

            if (products == null)
                return NotFound("Products not found");

            return Ok(products);
        }


        [HttpGet("{id:int}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDTO product)
        {
            if (product == null)
                return BadRequest("Invalid Data");

            await _productService.AddAsync(product);

            return new CreatedAtRouteResult("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            if (product == null)
                return BadRequest();

            await _productService.UpdateAsync(product);

            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound("Product not found");

            await _productService.RemoveAsync(id);

            return Ok(product);
        }

    }
}
