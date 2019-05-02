namespace BinarApp.API.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using BinarApp.Core.POCO;

    public partial class BinarContext : DbContext
    {
        public BinarContext()
            : base("name=BinarContext")
        {
        }

        public DbSet<Fixation> Fixations { get; set; }

        public DbSet<Intruder> Intruders { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<FixationDetail> FixationDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
