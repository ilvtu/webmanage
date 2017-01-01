namespace IlvtuManage.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TradeModel : DbContext
    {
        public TradeModel()
            : base("name=TradeModel")
        {
        }

        public virtual DbSet<ilvtu_TradeStatus> ilvtu_TradeStatus { get; set; }
        public virtual DbSet<PlanTrade> PlanTrade { get; set; }
        public virtual DbSet<TradeRequest> TradeRequest { get; set; }

        public virtual DbSet<RequesterInfo> RequesterInfo { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
