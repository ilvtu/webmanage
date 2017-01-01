using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace IlvtuManage.Models
{
    public class PlanInfoViewModel
    {
        public int recordId { get; set; }
        public int orderId { get; set; }
        public float? recordX { get; set; }
        public float? recordY { get; set; }

        public int? cost { get; set; }
        public string recordInfo { get; set; }
        public int? recordType { get; set; }
        public string travelTime { get; set; }
        public int dayNum { get; set; }
    }

}