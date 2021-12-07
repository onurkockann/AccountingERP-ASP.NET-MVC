using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class UserController : Controller
    {
        public void RegisterM(Firm kat)
        {
            //Magaza ekleme
            Model m = new Model();
            Firm k = m.Firms.FirstOrDefault(x => x.FirmID == kat.FirmID);
            if (k == null)
            {
                kat.PartnerCount = 1;
                kat.NetProfit = 0;
                m.Firms.Add(kat);

                m.SaveChanges();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();

            if ((string)TempData["Y"] == "1")
                ViewBag.FailMsg = "Kullanıcı adı veya parola hatalı";
            else if ((string)TempData["Y"] == "7")
                ViewBag.DoneMsg = "Şifreniz başarıyla mail adresinize gönderilmiştir.";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(FirmUser user)
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == user.username && x.pw == user.pw);

            if (u != null)//Login ol
            {
                //Kullanıcı junk usermı?
                if (u.Rank != 2 && u.Firm == null)
                {
                    TempData["Y"] = "1";
                    return RedirectToAction("Login", "User");
                }

                HttpCookie AuthCookie;
                AuthCookie = FormsAuthentication.GetAuthCookie(u.username, true);
                AuthCookie.Expires = DateTime.Now.AddHours(5);
                Response.Cookies.Add(AuthCookie);

                return RedirectToAction("Dashboard", "Home");

            }
            else
            {
                TempData["Y"] = "1";
                return RedirectToAction("Login", "User");
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            FormsAuthentication.SignOut();

            if ((string)TempData["Y"] == "3")
                ViewBag.FailMsg = "Bu kullanıcı adına sahip bir kullanıcı var.Lütfen başka bir kullanıcı adı deneyin.";
            else if ((string)TempData["Y"] == "4")
                ViewBag.FailMsg = "Girdiğiniz şifreler birbirine eşit değil.";
            else if ((string)TempData["Y"] == "5")
                ViewBag.FailMsg = "Lütfen mağaza adını boş bırakmayın.";
            else if ((string)TempData["Y"] == "6")
                ViewBag.FailMsg = "Bu maile sahip bir kullanıcı var.Lütfen başka bir mail deneyin.";
            else if ((string)TempData["Y"] == "7")
                ViewBag.FailMsg = "Accounting ERP'e kayıt olup kullanmak için kullanım koşullarımızı kabul etmeniz gerekiyor. Detaylı bilgi için gizlilik sözleşmesi, kullanım koşulları ve çerez politikamızı inceleyin.";

            Model m = new Model();

            FirmUser Usr = new FirmUser();
            return View(Usr);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterUser(FirmUser kat, String FirmName, String pwt, String KVKK)
        {
            if (FirmName == null || FirmName == "")
            {
                TempData["Y"] = "5";
                return RedirectToAction("Register");
            }
            if (kat.pw != pwt)
            {
                TempData["Y"] = "4";
                return RedirectToAction("Register");
            }
            if (KVKK == null)
            {
                TempData["Y"] = "7";
                return RedirectToAction("Register");
            }

            Model m = new Model();
            FirmUser k = m.FirmUsers.FirstOrDefault(x => x.username == kat.username || x.mail == kat.mail);
            if (k == null)
            {
                Firm newS = new Firm();
                newS.FirmName = FirmName;
                RegisterM(newS);

                kat.Rank = 1;
                kat.NetState = 0;
                kat.DateJoin = DateTime.Now;
                kat.FirmID = newS.FirmID;
                m.FirmUsers.Add(kat);
                FormsAuthentication.SetAuthCookie(kat.username, true);

                Article art = new Article();
                art.Content = "Accounting ERP'e hoşgeldiniz! Sorularınız için ''Yardım'' bölümünü, daha fazla bilgi için lütfen ''Mağaza'' bölümünü inceleyın.";
                art.EventDate = DateTime.Now;
                art.FirmID = kat.FirmID;
                m.Articles.Add(art);

                m.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (k.username == kat.username)
                    TempData["Y"] = "3";
                else
                    TempData["Y"] = "6";

                return RedirectToAction("Register");
            }
        }
    }
}