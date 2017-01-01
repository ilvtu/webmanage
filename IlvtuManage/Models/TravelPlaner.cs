namespace IlvtuManage.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TravelPlaner")]
    public partial class TravelPlaner
    {
        public int Id { get; set; }

        
        [StringLength(128)]
        public string PlanerId { get; set; }

        [StringLength(100)]
        public string PlanerName { get; set; }

        public int? PlanerType { get; set; }

        public int? Sex { get; set; }

        [StringLength(100)]
        public string PlanerPhoto { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string QQ { get; set; }

        [StringLength(50)]
        public string Weixin { get; set; }

        [StringLength(50)]
        public string OtherContact { get; set; }

        [Column(TypeName = "ntext")]
        public string PlanerInfo { get; set; }

        [StringLength(50)]
        public string BlogUrl { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public int? PlanerStatus { get; set; }

        public int PlanerLevel { get; set; }
    }

    public partial class ilvtu_PlanerType
    {
        public int Id { get; set; }

        public int? TypeId { get; set; }

        [StringLength(100)]
        public string TypeName { get; set; }
    }
}
