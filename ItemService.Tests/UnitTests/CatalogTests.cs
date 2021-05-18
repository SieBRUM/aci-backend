using Microsoft.EntityFrameworkCore;
using ProductService.Controllers;
using ProductService.DBContexts;
using ProductService.Models.DTO;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Flurl.Http.Testing;

namespace ProductService.Tests.UnitTests
{
    public class CatalogTests
    {
        private readonly ProductController _controller;
        private readonly ProductServiceDatabaseContext _context;
        public CatalogTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDatabaseContext>().UseInMemoryDatabase(databaseName: "InMemoryProductDb").Options;

            _context = new ProductServiceDatabaseContext(options);
            _controller = new ProductController(_context);
            SeedProductInMemoryDatabaseWithData();
        }

        [Fact]
        private async Task GetCatalogEntries_ShouldReturnCatalogpageEmpty()
        {
            var imgblobs = new List<ImageBlobModel>();

            string serializedObject = JsonConvert.SerializeObject(imgblobs);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.GetCatalogEntries(0, 0);

                Assert.IsType<CatalogPage>(result);
                Assert.Empty(result.CatalogItems);
            }
        }

        [Fact]
        private async Task GetCatalogEntries_ShouldReturnCatalogpage()
        {
            var imgblobs = new List<ImageBlobModel>();

            string serializedObject = JsonConvert.SerializeObject(imgblobs);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.GetCatalogEntries(0, 5);
                Assert.IsType<CatalogPage>(result);
                Assert.Equal(5, result.CatalogItems.Count);
            }
        }
        [Fact]
        private async Task GetCatalogEntries_ShouldReturnCatalogpageNumber3()
        {
            var imgblobs = new List<ImageBlobModel>();

            string serializedObject = JsonConvert.SerializeObject(imgblobs);
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(serializedObject);
                var result = await _controller.GetCatalogEntries(3, 2);
                Assert.IsType<CatalogPage>(result);
                Assert.Single(result.CatalogItems);
            }
        }

        private void SeedProductInMemoryDatabaseWithData()
        {
            var category = new Category { Name = "Apha" };
            var category2 = new Category { Name = "Beta" };
            var category3 = new Category { Name = "Chat" };
            var category4 = new Category { Name = "Toine" };
            var category5 = new Category { Name = "Berny" };
            _context.Categories.Add(category);

            var data = new List<Product>
                {
                    new Product { Name = "BBB", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category },
                    new Product { Name = "ZZZ", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category2 },
                    new Product { Name = "AAA", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category3 },
                    new Product { Name = "CCC", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category4 },
                    new Product { Name = "DDD", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category5 },
                    new Product { Name = "FFF", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category2 },
                    new Product { Name = "GGG", Description = "", ProductState = ProductState.AVAILABLE, RequiresApproval = true, Category = category },
                };
            _context.Products.AddRange(data);
            _context.SaveChanges();
        }
    }
}
