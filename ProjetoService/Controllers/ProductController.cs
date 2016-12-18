using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetoModels;
using System.Data.Entity;

namespace ProjetoService.Controllers
{
    /// <summary>
    /// Contains methods for interacting with product data.
    /// </summary>
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private ProjetoContext _db = new ProjetoContext();

        /// <summary>
        /// Gets all products in the database.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<Building>> Get() => await _db.Products.ToArrayAsync();

        /// <summary>
        /// Gets the product with the given id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Building> Get(Guid id) => await _db.Products.FirstOrDefaultAsync(x => x.Id == id);


        /// <summary>
        /// Gets all products with a data field matching the start of the given string.
        /// </summary>
        [HttpGet("search")]
        public async Task<IEnumerable<Building>> Search(string value) =>
            await _db.Products.Where(x =>
                x.Name.StartsWith(value) ||
                x.Color.StartsWith(value) ||
                x.DaysToManufacture.ToString().StartsWith(value) ||
                x.StandardCost.ToString().StartsWith(value) ||
                x.ListPrice.ToString().StartsWith(value) ||
                x.Weight.ToString().StartsWith(value) ||
                x.Description.StartsWith(value))
            .ToArrayAsync();
    }
}
