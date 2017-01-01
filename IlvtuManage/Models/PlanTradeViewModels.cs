using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace IlvtuManage.Models
{
    public class RequestForPlanerViewModel
    {
        public int requestId { get; set; }

        public string requestInfo { get; set; }

        public string requestNo { get; set; }

        public DateTime addTime { get; set; }
        public int statusId { get; set; }
    }

}