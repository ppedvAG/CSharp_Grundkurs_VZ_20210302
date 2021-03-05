using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab16_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab16_API.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {

        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>().ToTable("TodoItem"); 
        }
    }
}
