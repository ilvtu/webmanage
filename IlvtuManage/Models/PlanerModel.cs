namespace IlvtuManage.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PlanerModel : DbContext
    {
        public PlanerModel()
            : base("name=PlanerModel")
        {
        }

        public virtual DbSet<TravelPlaner> TravelPlaner { get; set; }
        public virtual DbSet<ilvtu_PlanerType> ilvtu_PlanerType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
