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

using System.Data.SqlClient;

namespace IlvtuManage.Controllers
{
    [Authorize]
    public class PlanTradeController : Controller
    {
        private TravelModel travelDb = new TravelModel();
        private PlanerModel planerDb = new PlanerModel();
        private TradeModel tradeDb = new TradeModel();
       
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManger;

        public PlanTradeController()
        { }
        public PlanTradeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
                var curPlanerId = User.Identity.GetUserId();

                var user =await UserManager.FindByIdAsync(curPlanerId);

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

                var planer = planerDb.TravelPlaner.Where(x => x.PlanerId == curPlanerId);
                if(planer!=null && planer.ToList().Count>0)
                {

                    ViewData["planer"] = planer.First();

                    var tRs = (from tR in tradeDb.PlanTrade
                               join r in tradeDb.TradeRequest on tR.RequestId equals r.Id
                               where tR.PlanerId == curPlanerId && tR.StatusId == 1
                               select new RequestForPlanerViewModel
                               {
                                   //dayNum = tR.DayNumber,
                                   addTime = (DateTime)tR.AddTime,
                                   requestId = r.Id,
                                   requestInfo  = r.RequestInfo,
                                   requestNo = r.RequestNo,
                                   statusId = (int)tR.StatusId

                               }).OrderBy(x => x.addTime) as IEnumerable<RequestForPlanerViewModel>;
                   
                    ViewData["requestlist"] = tRs;




                    
                }
            }

            return View();
        }

        

        // GET: RequestInfo
        public ActionResult RequestInfo(int requestId)
        {

            var curPlanerId = User.Identity.GetUserId();
            TradeRequest req = tradeDb.TradeRequest.Where(x => x.Id == requestId).First();
            RequesterInfo curRequester = tradeDb.RequesterInfo.Where(y => y.RequesterId == req.Requester).First();
            PlanTrade curtrade = tradeDb.PlanTrade.Where(x => x.RequestId == requestId && x.PlanerId == curPlanerId).First();


            ViewData["curReq"] = req;
            ViewData["curRequester"] = curRequester;
            ViewData["curtradestatus"] = curtrade.StatusId;
            return View();
        }

        // GET: sendPlan
        public async Task<ActionResult> sendPlan(int requestId)
        {
            var curPlanerId = User.Identity.GetUserId();
            int result = await tradeDb.Database.ExecuteSqlCommandAsync("update PlanTrade set StatusId =3 where RequestId=" + requestId + " and PlanerId=" + curPlanerId);

            if (result > 1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("RequestInfo",new { requestId = requestId });
            }
        }


        // GET: ChoosePlan
        public ActionResult ChoosePlan(int requestId)
        {
            var curPlanerId = User.Identity.GetUserId();
            IEnumerable<TravelPlan > planerPlans = travelDb.TravelPlan.Where(x => x.TravelPlanner == curPlanerId && (x.TravelStatus ==6 || x.TravelStatus==7));

            
            ViewData["myPlans"] = planerPlans;
            return View();
        }

        // post: ChoosePlan
        [HttpPost]
        public async Task<ActionResult> ChoosePlan(int requestId,FormCollection curForm)
        {
            var curPlanerId = User.Identity.GetUserId();
            int result = 0;
            if (curForm.GetValues("checkboxPlan") != null)//这是判断name为checkboxPlan的checkbox的值是否为空，若为空返回NULL;
            {
                string chooseplans = curForm.GetValue("checkboxPlan").AttemptedValue;//AttemptedValue返回一个以，分割的字符串
                string[] lstplans = chooseplans.Split(',');
                foreach (string r in lstplans)
                {
                    int chooseplan = Convert.ToInt32(r);

                     result = await tradeDb.Database.ExecuteSqlCommandAsync("update PlanTrade set StatusId =2 ,AddTime="+DateTime.Now.ToShortTimeString()+" , PlanId = "+chooseplan+" where RequestId="+requestId+" and PlanerId="+ curPlanerId);
                    
                }
            }
            if (result > 1)
            {
                return RedirectToAction("RequestInfo");
            }
            else
            {
                IEnumerable<TravelPlan> planerPlans = travelDb.TravelPlan.Where(x => x.TravelPlanner == curPlanerId && (x.TravelStatus == 6 || x.TravelStatus == 7));


                ViewData["myPlans"] = planerPlans;
                return View();
            }
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
