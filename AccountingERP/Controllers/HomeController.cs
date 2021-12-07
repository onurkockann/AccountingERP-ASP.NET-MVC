using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public void setViewBags()
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;

                ViewBag.netDurum = (int)u.Firm.NetProfit;


                List<FirmUser> uye = m.FirmUsers.ToList();
                ViewBag.uyeler = uye.Where(x => x.FirmID == ViewBag.magazaID).Count();
            }
        }

        //[Authorize]
        [HttpGet]
        public ActionResult Dashboard(int sayfa = 1)
        {
            //Template için ayarlanması gereken viewbag mesajları
            setViewBags();

            Model m = new Model();
            int firmID = ViewBag.magazaID;

            List<Article> art = m.Articles.Where(x=> x.FirmID == firmID).ToList();
            art.Reverse();

            if ((string)TempData["BG"] == "1")
                ViewBag.FailMsg = "Erişim engellendi! Lütfen gitmek istediğiniz bölüme ilgili alanları kullanarak erişin.";

            return View(art);
        }

        [HttpPost]
        public ActionResult Dashboard(Article art)
        {
            //Template için ayarlanması gereken viewbag mesajları
            setViewBags();

            if (art.Content == null || art.Content == "" || art.Content == " ")
            {
                ViewBag.FailMsg = "Lütfen gönderi içeriğini boş bırakmayın.";
                return Dashboard();
            }


            //Magaza bilgisi icin mevcut kullanıcı çekiliyor
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);

            //post atma işlemi
            Model mA = new Model();
            Article artk = mA.Articles.FirstOrDefault(x => x.ArticleID == art.ArticleID);
            if (artk == null)
            {
                art.FirmUserID = u.FirmUserID;
                art.FirmID = u.Firm.FirmID;
                art.EventDate = DateTime.Now;
                ViewBag.DoneMsg = "Gönderi başarıyla paylaşıldı.";
                m.Articles.Add(art);
            }

            m.SaveChanges();

            //Tekrar dashboarda dönüş
            return Dashboard();
        }

        [HttpPost]
        public ActionResult PostSil(int id)
        {
            Model m = new Model();
            Article k = m.Articles.FirstOrDefault(x => x.ArticleID == id);

            try
            {
                m.Articles.Remove(k);
                m.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Dashboard");
        }
    }
}