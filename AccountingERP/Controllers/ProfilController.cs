using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class ProfilController : Controller
    {
        public void setViewBags()
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;
                ViewBag.uID = u.FirmUserID;
                ViewBag.uYetki = u.Rank;
            }
        }

        public void setPaydasCount()
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);

            List<FirmUser> mgzs = m.FirmUsers.Where(x=> x.FirmID == u.FirmID).ToList();

            int pyC = 0;
            foreach (FirmUser item in mgzs)
            {
                if (item.Rank == 1)
                {
                    pyC++;
                }
            }

            Firm getMg = m.Firms.FirstOrDefault(x => x.FirmID == u.FirmID);
            getMg.PartnerCount = pyC;
            m.SaveChanges();
        }

        // GET: Profil
        public ActionResult Index()
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);

            if ((string)TempData["Y"] == "1")
                ViewBag.DoneMsg = "Profil bilgileri başarıyla güncellendi.";
            else if ((string)TempData["Y"] == "2")
                ViewBag.FailMsg = "Bu kullanıcı adına sahip bir kullanıcı var.Lütfen başka bir kullanıcı adı deneyin.";
            else if ((string)TempData["PwErr"] != null)
                ViewBag.FailMsg = TempData["PwErr"];
            else if ((string)TempData["PwDone"] != null)
                ViewBag.DoneMsg = TempData["PwDone"];
            else if ((string)TempData["PwOErr"] != null)
                ViewBag.FailMsg = TempData["PwOErr"];
            else if ((string)TempData["DG"] != null)
                ViewBag.DoneMsg = TempData["DG"];
            else if ((string)TempData["DGErr"] != null)
                ViewBag.FailMsg = TempData["DGErr"];


            return View(u);
        }

        [HttpGet]
        public ActionResult GuncelleUser(int id)
        {
            Model m = new Model();
            FirmUser k = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == id);
            setViewBags();

            int tempMCID = ViewBag.uID;
            if (k != null)
            {
                if (k.FirmUserID != tempMCID)
                {
                    TempData["BG"] = "1";
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                TempData["BG"] = "1";
                return RedirectToAction("Dashboard", "Home");
            }

            if ((string)TempData["Y"] == "2")
                ViewBag.FailMsg = "Bu kullanıcı adına sahip bir kullanıcı var.Lütfen başka bir kullanıcı adı deneyin.";
            else if ((string)TempData["MErr"] != null)
                ViewBag.FailMsg = TempData["MErr"];


            return View(k);
        }

        [HttpPost]
        public ActionResult GuncelleUser(FirmUser kat)
        {
            Model m = new Model();
            FirmUser k = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == kat.FirmUserID);


            FirmUser k2 = m.FirmUsers.FirstOrDefault(x => x.username == kat.username && x.FirmUserID != kat.FirmUserID);
            if (k2 == null) // username uygun.
            {
                FirmUser k3 = m.FirmUsers.FirstOrDefault(x => x.mail == kat.mail && x.FirmUserID != kat.FirmUserID);

                if (k3 != null)
                {
                    TempData["MErr"] = "Bu maile sahip bir kullanıcı var.Lütfen başka bir mail deneyin.";
                    return RedirectToAction("GuncelleUser", new { id = kat.FirmUserID });
                }

                k.username = kat.username;
                k.mail = kat.mail;
                k.Phone = kat.Phone;
                k.name = kat.name;
                k.surname = kat.surname;
                k.Rank = kat.Rank;
                m.SaveChanges();
                FormsAuthentication.SetAuthCookie(kat.username, true);
                TempData["Y"] = "1";
                return RedirectToAction("Index", "Profil");
            }
            else
            {
                FirmUser k3 = m.FirmUsers.FirstOrDefault(x => x.mail == kat.mail && x.FirmUserID != kat.FirmUserID);

                if (k3 != null)
                {
                    TempData["MErr"] = "Bu maile sahip bir kullanıcı var.Lütfen başka bir mail deneyin.";
                    return RedirectToAction("GuncelleUser", new { id = kat.FirmUserID });
                }

                TempData["Y"] = "2";
                return RedirectToAction("GuncelleUser", new { id = kat.FirmUserID });
            }
        }

        [HttpPost]
        public ActionResult ChgPw(String pwold, String pwnew1, String pwnew2)
        {
            setViewBags();
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);

            if (u.pw != pwold)
            {
                TempData["PwOErr"] = "Eski şifrenizi yalnış girdiniz.";
                return RedirectToAction("Index");
            }

            if (pwnew1 != pwnew2)
            {
                TempData["PwErr"] = "Yeni şifreler birbirine eşit değil.";
                return RedirectToAction("Index");
            }



            u.pw = pwnew1;
            m.SaveChanges();
            TempData["PwDone"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Downgrade(int KullaniciID, String pw)
        {
            setViewBags();
            Model m = new Model();
            FirmUser getUs = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == KullaniciID);
            if (getUs.pw != pw)
            {
                TempData["DGErr"] = "Şifrenizi yalnış girdiniz. İşleme devam etmek için şifrenizi doğru girin ve tekrar deneyin.";
                return RedirectToAction("Index");
            }

            getUs.Rank = 0;
            getUs.NetState = 0;

            FirmUser getM = m.FirmUsers.FirstOrDefault(x => x.FirmID == getUs.Firm.FirmID);

            //ortak çıktıktan sonra bütün ortakların durumlarının sıfırlanması.
            List<FirmUser> allPayds = m.FirmUsers.Where(x=> x.FirmID == getM.FirmID).ToList();
            foreach (FirmUser itemKl in allPayds)
            {
                itemKl.NetState = 0;
            }

            //Elden para aktarımında bulunmuşsa bu işlemi geçersiz kılma;
            List<MTIPartner> chcEpAl = m.MTIPartners.Where(x=> x.SenderID == getUs.FirmUserID).ToList();
            List<MTIPartner> chcEpVer = m.MTIPartners.Where(x => x.ReceiverID == getUs.FirmUserID).ToList();

            if (chcEpAl.Count != 0)
            {
                foreach (MTIPartner itemepAl in chcEpAl)
                {
                    itemepAl.EventState = 0;
                }
            }

            if (chcEpVer.Count != 0)
            {
                foreach (MTIPartner itemepVer in chcEpVer)
                {
                    itemepVer.EventState = 0;
                }
            }

            m.SaveChanges();
            setPaydasCount();

            TempData["DG"] = "İşlem tamamlandı. Artık mağazada ortak değil, personelsiniz.";
            return RedirectToAction("Index");
        }
    }
}