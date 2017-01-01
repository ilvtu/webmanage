namespace IlvtuManage.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TravelModel : DbContext
    {
        public TravelModel()
            : base("name=TravelModel")
        {
        }

        public virtual DbSet<ilvtu_RecordImgStatus> ilvtu_RecordImgStatus { get; set; }
        public virtual DbSet<ilvtu_RecordType> ilvtu_RecordType { get; set; }
        public virtual DbSet<ilvtu_TravelStatus> ilvtu_TravelStatus { get; set; }
        public virtual DbSet<ilvtu_TravelType> ilvtu_TravelType { get; set; }
        public virtual DbSet<Record> Record { get; set; }
        public virtual DbSet<RecordImage> RecordImage { get; set; }
        public virtual DbSet<TravelPlan> TravelPlan { get; set; }

        public virtual DbSet<TravelRecords> TravelRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecordImage>()
                .Property(e => e.ImageUrl)
                .IsFixedLength();
        }
        
    }
}
