using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace licenseDemoNetCore
{

    public class UserContext : DbContext
    {
        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=172.17.0.3;Database=utsuser;Username=postgres;Password=sql");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(t => new { t.id, t.username });
        }
    }

    public class User
    {
        public long id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string password { get; set; }
        public string refreshtoken { get; set; }
        public DateTime? refreshtokenexpirationdate { get; set; }
    }

}