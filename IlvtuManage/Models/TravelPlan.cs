namespace IlvtuManage.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TravelPlan")]
    public partial class TravelPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(200)]
        public string TravelName { get; set; }

        [Column(TypeName = "ntext")]
        public string TravelInfo { get; set; }

        public int? TravelCost { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(100)]
        public string TravelPlanner { get; set; }

        public int? TravelType { get; set; }

        public int? TravelStatus { get; set; }
    }
}
