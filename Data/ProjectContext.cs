using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ProjectContext:DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> opts) : base(opts) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<User>().HasIndex(u => new { u.CountryCode, u.PhoneNumber }).IsUnique();
            b.Entity<User>().HasIndex(u => u.Email).IsUnique(false);
        }
    }
}
