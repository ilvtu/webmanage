using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace IlvtuManage.Models
{
    public class EditRecordInDayViewModel
    {
        public int recordId { get; set; }
        public int orderId { get; set; }
        public double? recordX { get; set; }
        public double? recordY { get; set; }

        public int? cost { get; set; }
        public string recordInfo { get; set; }
        public int? recordType { get; set; }
        public string travelTime { get; set; }
        public string imgUrl { get; set; }
    }

}