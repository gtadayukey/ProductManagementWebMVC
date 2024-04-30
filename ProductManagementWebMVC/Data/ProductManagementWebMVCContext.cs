using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductManagementWebMVC.Models;

namespace ProductManagementWebMVC.Data
{
    public class ProductManagementWebMVCContext : DbContext
    {
        public ProductManagementWebMVCContext (DbContextOptions<ProductManagementWebMVCContext> options)
            : base(options)
        {
        }

        public DbSet<ProductManagementWebMVC.Models.Product> Product { get; set; } = default!;
    }
}
