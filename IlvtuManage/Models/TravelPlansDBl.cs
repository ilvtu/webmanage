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

    [Table("Record")]
    public partial class Record
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public double? RecordX { get; set; }

        public double? RecordY { get; set; }

        [StringLength(200)]
        public string RecordInfo { get; set; }

        public int? Cost { get; set; }

        public int? RecordType { get; set; }

        [StringLength(50)]
        public string TravelTime { get; set; }
    }

    [Table("RecordImage")]
    public partial class RecordImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(10)]
        public string ImageUrl { get; set; }

        public int? RecordId { get; set; }

        public DateTime? AddTime { get; set; }

        public int? Status { get; set; }
    }

    [Table("TravelPlaner")]
    public partial class TravelPlaner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
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

        public int? PlanerStatus { get; set; }

        public int PlanerLevel { get; set; }
    }

}
