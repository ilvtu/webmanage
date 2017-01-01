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
        public int Id { get; set; }

        [StringLength(200)]
        public string TravelName { get; set; }

        [Column(TypeName = "ntext")]
        public string TravelInfo { get; set; }

        public int? TravelCost { get; set; }
        public int? TravelDays { get; set; }
        public DateTime? CreateTime { get; set; }

        public DateTime? AlterTime { get; set; }
        [StringLength(100)]
        public string TravelPlanner { get; set; }

        public int? TravelType { get; set; }

        public int? TravelStatus { get; set; }
        [StringLength(200)]
        public string TravelCity { get; set; }
        [StringLength(200)]
        public string ShowImageUrl { get; set; }
    }

    [Table("RecordImage")]
    public partial class RecordImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; }

        public int? RecordId { get; set; }

        public DateTime? AddTime { get; set; }

        public int? Status { get; set; }
    }

    [Table("Record")]
    public partial class Record
    {
        public int Id { get; set; }

        public  double? RecordX { get; set; }

        public double? RecordY { get; set; }

        [StringLength(200)]
        public string RecordInfo { get; set; }

        public int? Cost { get; set; }

        public int? RecordType { get; set; }

        [StringLength(50)]
        public string TravelTime { get; set; }
    }

    [Table("TravelRecords")]
    public partial class TravelRecords
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int PlanId { get; set; }
        public int RecordId { get; set; }
        public int OrderId { get; set; }
        public int DayNumber { get; set; }
    }
    public partial class ilvtu_TravelType
    {
        public int Id { get; set; }

        public int? TypeID { get; set; }

        [StringLength(100)]
        public string TypeInfo { get; set; }
    }

    public partial class ilvtu_TravelStatus
    {
        public int Id { get; set; }

        public int? StatusId { get; set; }

        [StringLength(50)]
        public string StatusInfo { get; set; }
    }

    public partial class ilvtu_RecordType
    {
        public int Id { get; set; }

        public int? TypeId { get; set; }

        [StringLength(100)]
        public string TypeInfo { get; set; }
    }

    public partial class ilvtu_RecordImgStatus
    {
        public int Id { get; set; }

        public int? StatusId { get; set; }

        [StringLength(50)]
        public string StatusInfo { get; set; }
    }
}
