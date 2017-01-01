namespace IlvtuManage.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
}
