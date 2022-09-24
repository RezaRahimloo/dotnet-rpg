using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "Fire ball", Damage = 30},
                new Skill { Id = 2, Name = "Grasp heart", Damage = 10000},
                new Skill { Id = 3, Name = "Black Hole", Damage = 400},
                new Skill { Id = 4, Name = "Blink", Damage = 0}
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
    }
}