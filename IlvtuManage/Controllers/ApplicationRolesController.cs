using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IlvtuManage.Models;

namespace IlvtuManage.Controllers
{
    [Authorize(Roles ="Administrator")]
     public class ApplicationRolesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManger;

        public ApplicationRolesController()
        {
        }

        public ApplicationRolesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManger ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManger = value;
            }
        }

        
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: ApplicationRoles
        public ActionResult EditAdmin()
        {
            return View(db.Users.Where(x=>x.Email.Contains("@ilvtu.net")).ToList());
        }

        // GET: ApplicationRoles
        //EditUserRole
        public async Task< ActionResult> EditUserRoles(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

             var user = await UserManager.FindByIdAsync(userId);
           
            string[] roleIds = user.Roles.Select(x => x.RoleId).ToArray();
            
            

            IEnumerable<ApplicationRole> roles = RoleManager.Roles.Where(x=>roleIds.Any(y=>y==x.Id));

            IEnumerable<ApplicationRole> nonRoles = RoleManager.Roles.Except(roles);
            
            return View(new EditUserRolesViewModel()
            {
                User = user,
                Roles = roles,
                NonRoles = nonRoles   
            });

        }

        //
        // POST: /ApplicationRole/addUserRole
      
        public async Task<ActionResult> addUserRole(string userId, string roleName)
        {
            IdentityResult result;
            if(ModelState.IsValid)
            {
                
                result = await UserManager.AddToRoleAsync(userId, roleName);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
            }
            return RedirectToAction("EditUserRoles", new { userId = userId });
        }

        //
        // POST: /ApplicationRole/removeUserRole
       
        public async Task<ActionResult> removeUserRole(string userId, string roleName)
        {

            IdentityResult result;
            if (ModelState.IsValid)
            {

                result = await UserManager.RemoveFromRoleAsync(userId, roleName);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
            }

            return RedirectToAction("EditUserRoles", new { userId = userId });
        }

        // GET: ApplicationRoles
        public ActionResult Index()
        {
            return View(db.IdentityRoles.ToList());
        }

        // GET: ApplicationRoles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = db.IdentityRoles.Find(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // GET: ApplicationRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationRoles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] ApplicationRole applicationRole)
        {
            if (ModelState.IsValid)
            {                
                
                db.IdentityRoles.Add(applicationRole);
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            return View(applicationRole);
        }

        // GET: ApplicationRoles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = db.IdentityRoles.Find(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // POST: ApplicationRoles/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] ApplicationRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationRole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationRole);
        }

        // GET: ApplicationRoles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = db.IdentityRoles.Find(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // POST: ApplicationRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationRole applicationRole = db.IdentityRoles.Find(id);
            db.IdentityRoles.Remove(applicationRole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                if (_roleManger != null)
                {
                    _roleManger.Dispose();
                    _roleManger = null;
                }
            }

            base.Dispose(disposing);
        }

        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
