namespace IlvtuManage.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
}
