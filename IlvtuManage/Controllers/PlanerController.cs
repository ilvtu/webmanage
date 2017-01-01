using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IlvtuManage.Models;
using Qiniu.Storage;
using Qiniu.Util;
using Qiniu.Http;
namespace IlvtuManage.Controllers
{
    [Authorize]
    public class PlanerController : Controller
    {
        private TravelModel travelDb = new TravelModel();
        private PlanerModel planerDb = new PlanerModel();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManger;

        public PlanerController()
        { }
        public PlanerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GET: Planer
        public async Task<ActionResult> Index()
        {           
            if (User.Identity.IsAuthenticated)
            {
                var curPlanId = User.Identity.GetUserId();

                var user =await UserManager.FindByIdAsync(curPlanId);
                

                string[] roleIds =user.Roles.Select(x => x.RoleId).ToArray();
                IEnumerable<ApplicationRole> roles = RoleManager.Roles.Where(x => roleIds.Any(y => y == x.Id));
           
                ViewData["isAdmin"] = false;
                ViewData["isplaner"] = false;
                foreach (ApplicationRole curRole in roles)
                {
                    if (curRole.Name.Contains("Admin"))
                    {
                        ViewData["isAdmin"] = true;
                      
                    }
                    if (curRole.Name == "planer")
                    {

                        ViewData["isplaner"] = true;
                    }
                    
                }

                var planer = planerDb.TravelPlaner.Where(x => x.PlanerId == curPlanId);
                if(planer!=null && planer.ToList().Count>0)
                {

                    ViewData["planer"] = planer.First();
                }
                

            }

            return View();
        }



        // GET: Planer/PlanerRequest
        public async Task<ActionResult> PlanerRequest()
        {
            var curPlanId = User.Identity.GetUserId();
            TravelPlaner newplaner = new TravelPlaner();
            newplaner.PlanerId = curPlanId;
            newplaner.PlanerLevel = 1;
            newplaner.PlanerStatus = 1;
            planerDb.TravelPlaner.Add(newplaner);
            await planerDb.SaveChangesAsync();

            return RedirectToAction("EditPlanerInfo", new { id= newplaner.Id});
        }


    
      

        // GET: Planer/Edit/5
        public ActionResult EditPlanerInfo(int id,TravelPlaner curPlaner)
        {
            curPlaner = planerDb.TravelPlaner.Where(x => x.Id == id).First();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_PlanerType type in planerDb.ilvtu_PlanerType)
            {
                SelectListItem curItem = new SelectListItem { Text = type.TypeName, Value = type.TypeId.ToString() };
                if (curPlaner.PlanerType == type.TypeId)
                {
                    curItem.Selected = true;
                }
                items.Add(curItem);
            }


            ViewData["planerType"] = items;

            List<SelectListItem> items2 = new List<SelectListItem>();
            switch (curPlaner.Sex)
            {
                case 1:
                    items2.Add(new SelectListItem { Text = "男", Value = "1",Selected = true });
                    items2.Add(new SelectListItem { Text = "女", Value = "2" });
                    items2.Add(new SelectListItem { Text = "未知", Value = "3" });
                    break;
                case 2:
                    items2.Add(new SelectListItem { Text = "男", Value = "1" });
                    items2.Add(new SelectListItem { Text = "女", Value = "2", Selected = true });
                    items2.Add(new SelectListItem { Text = "未知", Value = "3" });
                    break;
                case 3:
                    items2.Add(new SelectListItem { Text = "男", Value = "1" });
                    items2.Add(new SelectListItem { Text = "女", Value = "2" });
                    items2.Add(new SelectListItem { Text = "未知", Value = "3", Selected = true });
                    break;
                default: break;
            }
            items2.Add(new SelectListItem { Text = "男", Value = "1" });
            items2.Add(new SelectListItem { Text = "女", Value = "2" });
            items2.Add(new SelectListItem { Text = "未知", Value = "3" });

            ViewData["sexType"] = items2;
           
            return View(curPlaner);
        }

        // POST: Planer/Edit/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPlanerInfo([Bind(Include = "Id,PlanerName,Phone,QQ,Weixin,OtherContact,PlanerInfo,BlogUrl,Email")]TravelPlaner curPlaner)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    curPlaner.PlanerType = int.Parse(Request.Form["planerType"]);
                    curPlaner.Sex = int.Parse(Request.Form["sexType"]);
                    curPlaner.PlanerId = User.Identity.GetUserId();
                    planerDb.TravelPlaner.Add(curPlaner);

                    planerDb.Entry(curPlaner).State = EntityState.Modified;
                    await planerDb.SaveChangesAsync();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentLength > 0)
                        {
                            var curPlanerId = User.Identity.GetUserId();
                            string key = curPlanerId + DateTime.Now.ToShortTimeString().Trim() + ".png";

                            //file.SaveAs("c:\\1.jpg");
                            UploadManager target = new UploadManager();
                            Mac mac = new Mac("pl3kz6DFwjTgrot1C6_Xca3dem0h4Jk1aSA4tFB0", "3_D9E03vPbcmP7y5IypXngQeN9_knq7wjfQwEr6j");
                            //string filePath = "c:\\1.jpg";
                            PutPolicy putPolicy = new PutPolicy();
                            putPolicy.Scope = "ilvtuweb";
                            putPolicy.SetExpires(3600);
                            putPolicy.DeleteAfterDays = 1;
                            string token = Auth.createUploadToken(putPolicy, mac);

                            UploadOptions uploadOptions = null;

                            UpCompletionHandler upCompletionHandler = new UpCompletionHandler(delegate (string fileKey, ResponseInfo respInfo, string response)
                            {
                                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, respInfo.StatusCode);
                            });
                            target.uploadStream(file.InputStream, key, token, uploadOptions, upCompletionHandler);

                            string newImageUrl = "http://ohtaq52av.bkt.clouddn.com/" + key;
                            string imgsql = "update TravelPlaner set PlanerPhoto='"+newImageUrl+"' where PlanerId='"+curPlanerId+"'" ;


                            await planerDb.Database.ExecuteSqlCommandAsync(imgsql);
                        }
                    }

                   

                    return RedirectToAction("Index");
                }

                // TODO: Add update logic here
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (ilvtu_PlanerType type in planerDb.ilvtu_PlanerType)
                {

                    items.Add(new SelectListItem { Text = type.TypeName, Value = type.TypeId.ToString() });
                }


                ViewData["planerType"] = items;


                List<SelectListItem> items2 = new List<SelectListItem>();

                items2.Add(new SelectListItem { Text = "男", Value = "1" });
                items2.Add(new SelectListItem { Text = "女", Value = "2" });
                items2.Add(new SelectListItem { Text = "未知", Value = "3" });

                ViewData["sexType"] = items2;


                return View();


            }
            catch
            {
                return View();
            }
        }

        // GET: Planer/PlanerPlan
        public ActionResult PlanerPlan()
        {
            
            ViewData["travelPlansToPublish"] = travelDb.TravelPlan.Where(x => x.TravelStatus == 4).ToList();
            ViewData["travelPlansToTrade"] = travelDb.TravelPlan.Where(x => x.TravelStatus == 6 || x.TravelStatus==7).ToList();

            return View();
        }


        public async Task<ActionResult> UnderCarriagePlan(string planId)
        {
            if (ModelState.IsValid)
            {
                await travelDb.Database.ExecuteSqlCommandAsync("update TravelPlan set TravelStatus =5 where Id = " + planId);
                return RedirectToAction("PlanerPlan");
            }

            return View();
        }

        // GET: Planer/PlanInfo
        public ActionResult PlanInfo(int planId)
        {
            ViewData["planId"] = planId;


            //获得所有旅游项目

            ViewData["curPlan"] =travelDb.TravelPlan.Where(x => x.Id == planId).First();
            int[] tRecords = travelDb.TravelRecords.Where(x => x.PlanId == planId).Select(y => y.DayNumber).ToArray();
            ViewData["dayNums"] = tRecords.ToList().Count;
            int dayNums = tRecords.ToList().Count;

            for (int i = 0; i < dayNums; i++)
            { int curDay = i + 1;
                var tRs = (from tR in travelDb.TravelRecords
                           join r in travelDb.Record on tR.RecordId equals r.Id

                           join ri in travelDb.RecordImage on r.Id equals ri.RecordId
                           into uri

                           where tR.PlanId == planId && tR.DayNumber == curDay
                           from tmpimg in uri.DefaultIfEmpty()

                           select new EditRecordInDayViewModel
                           {
                               imgUrl = tmpimg.ImageUrl,
                               recordId = r.Id,
                               orderId = tR.OrderId,
                               recordX = (double)r.RecordX,
                               recordY = (double)r.RecordY,
                               cost = r.Cost,
                               recordInfo = r.RecordInfo,
                               travelTime = r.TravelTime,
                               recordType = r.RecordType

                           }).OrderBy(x => x.orderId) as IEnumerable<EditRecordInDayViewModel>;
                string s = "recordsInDay" + curDay;
                ViewData[s] = tRs;
            }

            

            ///ViewData["recordNum"] = tRs.ToList().Count;
            return View();
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                planerDb.Dispose();
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
