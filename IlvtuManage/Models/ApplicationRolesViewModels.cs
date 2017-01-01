using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace IlvtuManage.Models
{
    public class EditUserRolesViewModel
    {

        public ApplicationUser User { get; set; }
        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<ApplicationRole> NonRoles { get; set; }
        
    }

}