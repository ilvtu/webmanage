namespace IlvtuManage.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TradeRequest")]
    public partial class TradeRequest
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string Requester { get; set; }
        [StringLength(100)]
        public string RequestNo { get; set; }
        [Column(TypeName = "ntext")]
        public string RequestInfo { get; set; }

        public int? RequestCost { get; set; }

        [StringLength(200)]
        public string RequestCity { get; set; }

        public int? TravelType { get; set; }

        public int? TravelPeopleNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TravelDate { get; set; }
    }

    [Table("RequesterInfo")]
    public partial class RequesterInfo
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string RequesterId { get; set; }

        public string RequesterName { get; set; }

        [StringLength(50)]
        public string ImageUrl { get; set; }

        public string Phone { get; set; }

        public int? RequesterLevel { get; set; }
    }


    [Table("PlanTrade")]
    public partial class PlanTrade
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string PlanerId { get; set; }

        public int? RequestId { get; set; }

        public int? PlanId { get; set; }

        public int? StatusId { get; set; }

        public DateTime? AddTime { get; set; }
    }

    public partial class ilvtu_TradeStatus
    {
        public int Id { get; set; }

        public int? TradeStatusId { get; set; }

        [StringLength(50)]
        public string StatusInfo { get; set; }
    }
}
