using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagementWebMVC.Controllers;
using ProductManagementWebMVC.Data;
using ProductManagementWebMVC.Models;

namespace ProductManagementTests
{
    public class IntegrateTests
    {
        [Fact]
        public async Task TestDatabaseConnection()
        {
            var builder = WebApplication.CreateBuilder([]);
            builder.Configuration.AddJsonFile("appsettings.json");
            var services = builder.Services;

            services.AddDbContext<ProductManagementWebMVCContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("ProductManagementWebMVCContext") ?? throw new InvalidOperationException("Connection string 'ProductManagementWebMVCContext' not found.")));

            await using var context = services.BuildServiceProvider().GetService<ProductManagementWebMVCContext>();
            try
            {
                await context.Database.OpenConnectionAsync();
                Assert.True(true, "Database succesfully connected.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Database failed connection. Error: {ex.Message}");
            }
        }

        [Fact]
        public async Task TestCreateProduct()
        {
            var options = new DbContextOptionsBuilder<ProductManagementWebMVCContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ProductManagementWebMVCContext(options);
            var controller = new ProductsController(context);

            var product = new Product
            {
                Name = "Test Product 1",
                Price = 7.99,
                Stock = 5
            };

            await controller.Create(product);

            var savedProduct = await context.Product.FirstOrDefaultAsync(p => p.Name == product.Name);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.Name, savedProduct.Name);
            Assert.Equal(product.Price, savedProduct.Price);
            Assert.Equal(product.Stock, savedProduct.Stock);
        }

        [Fact]
        public async Task TestDeleteProduct()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ProductManagementWebMVCContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var product = new Product
                {
                    Name = "Test Product 2",
                    Price = 12.99,
                    Stock = 2
                };
                context.Product.Add(product);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var controller = new ProductsController(context);

                var productId = await context.Product
                    .Where(p => p.Name == "Test Product 2")
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                Assert.NotEqual(0, productId);

                await controller.DeleteConfirmed(productId);
            }

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var savedProduct = await context.Product
                    .FirstOrDefaultAsync(p => p.Name == "Test Product 2");

                Assert.Null(savedProduct);
            }
        }

        [Fact]
        public async Task TestUpdateProduct()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ProductManagementWebMVCContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var product = new Product
                {
                    Name = "Test Product 3",
                    Price = 10.99,
                    Stock = 5
                };
                context.Product.Add(product);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var controller = new ProductsController(context);

                var productId = await context.Product
                    .Where(p => p.Name == "Test Product 3")
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                Assert.NotEqual(0, productId);

                var updatedProduct = new Product
                {
                    Id = productId,
                    Name = "Updated Test Product 3",
                    Price = 15.99,
                    Stock = 3
                };

                await controller.Edit(productId, updatedProduct);
            }

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var updatedProduct = await context.Product.FindAsync(1);

                Assert.NotNull(updatedProduct);
                Assert.Equal("Updated Test Product 3", updatedProduct.Name);
                Assert.Equal(15.99, updatedProduct.Price);
                Assert.Equal(3, updatedProduct.Stock);
            }
        }

        [Fact]
        public async Task TestReadProduct()
        {
            var databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ProductManagementWebMVCContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var product = new Product
                {
                    Name = "Test Product 4",
                    Price = 11.99,
                    Stock = 8
                };
                context.Product.Add(product);
                await context.SaveChangesAsync();
            }

            using (var context = new ProductManagementWebMVCContext(options))
            {
                var controller = new ProductsController(context);

                var productId = await context.Product
                    .Where(p => p.Name == "Test Product 4")
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                Assert.NotEqual(0, productId);

                var result = await controller.Details(productId);

                var viewResult = Assert.IsType<ViewResult>(result);

                var model = viewResult.ViewData.Model as Product;

                Assert.NotNull(model);
                Assert.Equal("Test Product 4", model.Name);
                Assert.Equal(11.99, model.Price);
                Assert.Equal(8, model.Stock);
            }
        }
    }
}