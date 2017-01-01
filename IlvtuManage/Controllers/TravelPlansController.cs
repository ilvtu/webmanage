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
    public class TravelPlansController : Controller
    {
        private TravelModel travelDb = new TravelModel();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        
        public TravelPlansController()
        { }
        public TravelPlansController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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
      
        // GET: Index
        public ActionResult Index()
        {

            string userName = "";
            if (User.Identity.IsAuthenticated)
            {
                userName = User.Identity.Name;
            }
            ViewData["userName"] = userName;
            ViewData["travelPlansToPublish"] = travelDb.TravelPlan.Where(x => x.TravelStatus == 1 ||x.TravelStatus==5).ToList();
            ViewData["travelPlansToTrade"] = travelDb.TravelPlan.Where(x => x.TravelStatus == 6 || x.TravelStatus ==7).ToList();
            return View();
        }


        // GET: CreatePlan
        public ActionResult CreatePlan()
        {
            

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_TravelType type in travelDb.ilvtu_TravelType)
            {

                items.Add(new SelectListItem { Text = type.TypeInfo, Value = type.TypeID.ToString() });
            }
           

            ViewData["travelType"] = items;

            List<SelectListItem> items2 = new List<SelectListItem>();
          
            items2.Add(new SelectListItem { Text = "公开发布用", Value = "1" });
            items2.Add(new SelectListItem { Text = "交易用", Value = "2" });

            ViewData["tradeType"] = items2;


            return View();
        }

        //
        // POST: CreatePlan
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePlan([Bind(Include ="TravelName,TravelInfo, TravelCost,TravelDays,TravelStatus")]TravelPlan travelPlan)
        {
            
            
            if (Request.Form["travelType"].Equals(""))
            {
                ModelState.AddModelError("typeError", "请选择一项旅游类型");
            }

            if (Request.Form["tradeType"].Equals(""))
            {
                ModelState.AddModelError("tradeError", "请选择一项线路用途类型");
            }

            if (ModelState.IsValid)
            {
                switch (Convert.ToInt32(Request.Form["tradeType"]))
                {
                    case 1:
                        travelPlan.TravelStatus = 1;
                        break;
                    case 2:
                        travelPlan.TravelStatus = 6;
                        break;
                    default:
                        travelPlan.TravelStatus = 1;
                        break;
                }

                travelPlan.CreateTime = DateTime.Now;
                travelPlan.TravelStatus = 1;
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                travelPlan.TravelPlanner = user.Id;
               
                travelPlan.TravelType = int.Parse(Request.Form["travelType"]);

                travelDb.TravelPlan.Add(travelPlan);

                await travelDb.SaveChangesAsync();
                
                return RedirectToAction("Plan", new {planId = travelPlan.Id });
            }

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_TravelType type in travelDb.ilvtu_TravelType)
            {

                items.Add(new SelectListItem { Text = type.TypeInfo, Value = type.TypeID.ToString() });
            }
            ViewData["travelType"] = items;
            return View();
        }


        // GET: Plan
        public ActionResult Plan(int planId)
        {           
            TravelPlan curPlan = travelDb.TravelPlan.Where(x => x.Id == planId).First();
            ViewData["curPlan"] = curPlan;


            return View();
        }

      

        // GET: EditRecordInDay
        public ActionResult EditRecordInDay(int dayNum,int planId)
        {
            ViewData["dayNum"] = dayNum;
            ViewData["planId"] = planId;


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_RecordType type in travelDb.ilvtu_RecordType)
            {

                items.Add(new SelectListItem { Text = type.TypeInfo, Value = type.TypeId.ToString() });
            }


            ViewData["recordType"] = items;


            //获得所有当日的旅游项目
            //int[] tRecords = travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum).Select(y => y.RecordId).ToArray();


            var tRs = (from tR in travelDb.TravelRecords
                       join r in travelDb.Record on tR.RecordId equals r.Id

                       join ri in travelDb.RecordImage on r.Id equals ri.RecordId
                       into uri

                       where tR.PlanId == planId && tR.DayNumber == dayNum
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

            ViewData["records"] = tRs ;

            ViewData["recordNum"] = tRs.ToList().Count;
            return View();
        }

        //
        // POST: EditRecordInDay
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRecordInDay(int dayNum, int planId,[Bind(Include = "RecordInfo,Cost,RecordType,TravelTime,RecordX,RecordY")]Record record)
        {
           

            if (Request.Form["recordType"].Equals(""))
            {
                ModelState.AddModelError("typeError", "请选择一个项目类型");
            }

            if (ModelState.IsValid)
            {
              


                record.RecordType = int.Parse(Request.Form["recordType"]);

                travelDb.Record.Add(record);

                await travelDb.SaveChangesAsync();
                int recordId = record.Id;

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        var curPlanerId = User.Identity.GetUserId();
                        string key = curPlanerId + DateTime.Now.ToString().Trim() + ".png";

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
                        string imgsql = "insert into RecordImage(AddTime,Status,RecordId,ImageUrl) values('" + DateTime.Now.ToString() + "',1," + recordId + ",'" + newImageUrl + "')";


                        await travelDb.Database.ExecuteSqlCommandAsync(imgsql);
                    }
                }
               

                int newRecordNum = travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum).Select(y => y.RecordId).ToList().Count + 1;


                await travelDb.Database.ExecuteSqlCommandAsync("insert into TravelRecords(PlanId,RecordId,OrderId,DayNumber) values(" + planId + "," + recordId + "," + newRecordNum + "," + dayNum + ")");
                

            }
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_RecordType type in travelDb.ilvtu_RecordType)
            {

                items.Add(new SelectListItem { Text = type.TypeInfo, Value = type.TypeId.ToString() });
            }


            ViewData["recordType"] = items;

            //获得所有当日的旅游项目
            //int[] tRecords = travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum).Select(y => y.RecordId).ToArray();


            var tRs = (from tR in travelDb.TravelRecords
                       join r in travelDb.Record on tR.RecordId equals r.Id

                       join ri in travelDb.RecordImage on r.Id equals ri.RecordId
                       into uri

                       where tR.PlanId == planId && tR.DayNumber == dayNum
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



            ViewData["records"] = tRs;
            ViewData["recordNum"] = tRs.ToList().Count;


             ViewData["dayNum"] = dayNum;
            ViewData["planId"] = planId;
            return View();
            
        
        }

        //get:RecGis
        //删除当日的旅游单项
        public ActionResult RecGis(int planId ,int dayNum , int recordId)
        {

            ViewData["dayNum"] = dayNum;
            ViewData["planId"] = planId;
            var curRecord = travelDb.Record.Where(x => x.Id == recordId).First();
            ViewData["curRecord"] = curRecord;

            return View();
        }

        //Post:RecGis
        //删除当日的旅游单项
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RecGis(int planId, int dayNum ,int recordId,Record record)
        {
            float rx =float.Parse( Request.Form["gis"].ToString().Split(',')[0]);
            float ry = float.Parse(Request.Form["gis"].ToString().Split(',')[1]);

           int result = await travelDb.Database.ExecuteSqlCommandAsync("update Record set RecordX= "+rx+",RecordY="+ ry+"  where Id="+recordId);

            if (result > 0)
            {
                return RedirectToAction("EditRecordInDay", new {  dayNum = dayNum, planId = planId });
            }
            else
            {
                return View();
            }
        }

        //post:DeleteRecordInDay
        //删除当日的旅游单项
        public async Task<ActionResult> DeleteRecordInDay(int planId,int dayNum,int recordId)
        {
            if (ModelState.IsValid)
            {
                //删除后修改其后所有orderId
                int delrecordOrderId = travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum && x.RecordId == recordId).First().OrderId;
                int result = await travelDb.Database.ExecuteSqlCommandAsync("delete from TravelRecords where PlanId ="+planId+" and DayNumber ="+ dayNum + " and RecordId = " + recordId);
                if (result > 0)
                {
                    var alterTRList=  travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum && x.OrderId > delrecordOrderId).ToList();
                    foreach (var tmpTR in alterTRList)
                    {
                        int newOrderId = tmpTR.OrderId-1;
                        await travelDb.Database.ExecuteSqlCommandAsync("update TravelRecords set OrderId="+newOrderId + " where PlanId = " + tmpTR.PlanId+ " and DayNumber =" + tmpTR.DayNumber + " and RecordId = " + tmpTR.RecordId);
                    }
                }
               
            }
            return RedirectToAction("EditRecordInDay",new {planId = planId,dayNum = dayNum });
        }


        //post:setlvtuimg
        //删除当日的旅游单项
        public async Task<ActionResult> setlvtuimg(int planId, int recordid)
        {
            if (ModelState.IsValid)
            {
                string url = travelDb.RecordImage.Where(x => x.RecordId == recordid).First().ImageUrl;
                string sqlstr = "update TravelPlan set ShowImageUrl='" + url + "'where Id=" + planId;
               int result= await travelDb.Database.ExecuteSqlCommandAsync(sqlstr);
            }
            return RedirectToAction("Plan", new { planId = planId});
        }
        //post:alterOrder
        //删除当日的旅游单项
        public async Task<ActionResult> alterOrder(int planId, int dayNum, int recordId,bool upOrder)
        {
            if (ModelState.IsValid)
            {
                //删除后修改其后所有orderId
                int nowOrderId = travelDb.TravelRecords.Where(x => x.PlanId == planId && x.DayNumber == dayNum && x.RecordId == recordId).First().OrderId;
                int newOrderId = 0;
                if (upOrder)    //上移
                {
                    newOrderId = nowOrderId - 1;
                }
                else             //下移
                {
                    newOrderId = nowOrderId + 1;
                }
                int result1= await travelDb.Database.ExecuteSqlCommandAsync("update TravelRecords set OrderId=" + nowOrderId + " where PlanId = " + planId + " and DayNumber =" + dayNum + " and OrderId = " + newOrderId);
                int result2 = await travelDb.Database.ExecuteSqlCommandAsync("update TravelRecords set OrderId=" + newOrderId + " where PlanId = " + planId + " and DayNumber =" + dayNum + " and RecordId = " + recordId);
                             

            }
            return RedirectToAction("EditRecordInDay", new { planId = planId, dayNum = dayNum });
        }
        // GET: ModifyPlan
        public ActionResult ModifyPlan(int planId,TravelPlan curPlan)
        {
            curPlan = travelDb.TravelPlan.Where(x => x.Id == planId).First();
           
            List<SelectListItem> items = new List<SelectListItem>();
            //SelectListItem tmpItem = new SelectListItem();
            foreach (ilvtu_TravelType type in travelDb.ilvtu_TravelType)
            {
                SelectListItem curItem = new SelectListItem { Text = type.TypeInfo, Value = type.TypeID.ToString() };
                if (curPlan.TravelType == type.TypeID)
                {
                    curItem.Selected= true;
                    //tmpItem = curItem;
                }
                items.Add(curItem);
                
            }
         
            //SelectList typelist = new SelectList(items,"Value","Text", 4);

            ViewData["travelType"] = items;
            return View(curPlan);
        }

        //
        // POST: ModifyPlan
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifyPlan([Bind(Include = "Id,TravelName,TravelInfo, TravelCost,TravelDays")]TravelPlan travelPlan)
        {           
            if (ModelState.IsValid)
            {

                travelPlan.AlterTime = DateTime.Now;
                travelPlan.TravelStatus = 1;
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                travelPlan.TravelPlanner = user.Id;

                travelPlan.TravelType = int.Parse(Request.Form["travelType"]);

               
                travelDb.TravelPlan.Add(travelPlan);

                travelDb.Entry(travelPlan).State = EntityState.Modified;
                await travelDb.SaveChangesAsync();

                return RedirectToAction("Plan", new { planId = travelPlan.Id });
            }


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ilvtu_TravelType type in travelDb.ilvtu_TravelType)
            {
                SelectListItem curItem = new SelectListItem { Text = type.TypeInfo, Value = type.TypeID.ToString() };
                if (travelPlan.TravelType == type.TypeID)
                {
                    curItem.Selected = true;
                    //tmpItem = curItem;
                }
                items.Add(curItem);

            }

            ViewData["travelType"] = items;
            return View();
        }
    

        //
        // POST: Publish
        public async Task<ActionResult> Publish(string planId)
        {

            if (ModelState.IsValid)
            {
                await travelDb.Database.ExecuteSqlCommandAsync("update TravelPlan set TravelStatus =4 where Id = " + planId);
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // POST: Publish
        public async Task<ActionResult> Trade(string planId)
        {

            if (ModelState.IsValid)
            {
                await travelDb.Database.ExecuteSqlCommandAsync("update TravelPlan set TravelStatus =6 where Id = " + planId);
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // DeletePlan

        public async Task<ActionResult> DeletePlan(string planId)
        {
            if (ModelState.IsValid)
            {
                await travelDb.Database.ExecuteSqlCommandAsync("update TravelPlan set TravelStatus = 3 where Id = " + planId);
                return RedirectToAction("Index");
            }

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                travelDb.Dispose();
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

        //// GET: TravelPlans/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TravelPlan travelPlan = db.TravelPlan.Find(id);
        //    if (travelPlan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(travelPlan);
        //}

        //// GET: TravelPlans/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: TravelPlans/Create
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,TravelName,TravelInfo,TravelCost,CreateTime,TravelPlanner,TravelType,TravelStatus")] TravelPlan travelPlan)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TravelPlan.Add(travelPlan);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(travelPlan);
        //}

        //// GET: TravelPlans/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TravelPlan travelPlan = db.TravelPlan.Find(id);
        //    if (travelPlan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(travelPlan);
        //}

        //// POST: TravelPlans/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TravelName,TravelInfo,TravelCost,CreateTime,TravelPlanner,TravelType,TravelStatus")] TravelPlan travelPlan)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(travelPlan).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(travelPlan);
        //}

        //// GET: TravelPlans/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TravelPlan travelPlan = db.TravelPlan.Find(id);
        //    if (travelPlan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(travelPlan);
        //}

        //// POST: TravelPlans/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TravelPlan travelPlan = db.TravelPlan.Find(id);
        //    db.TravelPlan.Remove(travelPlan);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


    }
}
