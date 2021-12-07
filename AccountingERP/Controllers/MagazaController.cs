using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class MagazaController : Controller
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
                ViewBag.kisiYetki = u.Rank;
            }
        }

        // GET: Magaza
        public ActionResult Index()
        {

            setViewBags();
            Model m = new Model();

            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);


            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;

                ViewBag.Adres = u.Firm.Adres;
                ViewBag.Telefon = u.Firm.Phone;
                ViewBag.WebSayfasi = u.Firm.WebSite;
                ViewBag.Ortaklar = u.Firm.PartnerCount;
            }

            if ((string)TempData["Y"] == "1")
                ViewBag.DoneMsg = "Mağaza bilgileri düzenlendi.";
            else if ((string)TempData["Y"] == "2")
                ViewBag.DoneMsg = "Kullanıcı başarıyla mağazaya eklendi.";
            else if ((string)TempData["Y"] == "4")
                ViewBag.DoneMsg = "Kullanıcı bilgileri başarıyla güncellendi.";
            else if ((string)TempData["YK"] == "1")
                ViewBag.FailMsg = "Mağazanıza ait bir pazar yeri algılanamadı. Lütfen mağaza bilgilerini düzenleme kısmından pazar yerinizi belirtin.";

            List<FirmUser> users = m.FirmUsers.Where(x=> x.FirmID == u.FirmID).ToList();

            return View(users);
        }


        [HttpGet]
        public ActionResult Ekle()
        {
            setViewBags();
            Model m = new Model();

            Firm mg = new Firm();
            return View(mg);
        }

        [HttpPost]
        public ActionResult Ekle(Firm kat)
        {
            Model m = new Model();
            Firm k = m.Firms.FirstOrDefault(x => x.FirmID == kat.FirmID);

            k.FirmName = kat.FirmName;
            k.Adres = kat.Adres;
            k.Phone = kat.Phone;
            k.WebSite = kat.WebSite;

            m.SaveChanges();

            TempData["Y"] = "1";


            return RedirectToAction("Index", "Magaza");
        }

        public ActionResult Guncelle(int id)
        {
            setViewBags();

            Model m = new Model();

            //Magaza bilgisi çekilecek. İlk olarak kullanıcı çekiliyor.
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);

            Firm kat = m.Firms.FirstOrDefault(x => x.FirmID == id);
            //istek dışı erişimin engellenmesı;
            int tempMCID = ViewBag.magazaID;
            if (kat != null)
            {
                if (kat.FirmID != tempMCID)
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



            if (kat == null)
                return RedirectToAction("Dashboard", "Home");
            else
            {
                return View("Ekle", kat);
            }

        }
        [HttpGet]
        public ActionResult AddUser()
        {
            setViewBags();
            Model m = new Model();
            int magazaId = ViewBag.magazaID;

            Firm getOrtC = m.Firms.FirstOrDefault(x => x.FirmID == magazaId);
            ViewBag.ortaklar = getOrtC.PartnerCount;

            if ((string)TempData["Y"] == "3")
                ViewBag.FailMsg = "Bu kullanıcı adına sahip bir kullanıcı var.Lütfen başka bir kullanıcı adı deneyin.";
            else if ((string)TempData["Y"] == "5")
                ViewBag.FailMsg = "Bu maile sahip bir kullanıcı var.Lütfen başka bir mail deneyin.";
            else if ((string)TempData["RqErr"] != null)
                ViewBag.FailMsg = TempData["RqErr"];
            else
                ViewBag.WarnMsg = "Mağazanıza ortak olarak kaydettiğiniz kişiyi bir daha silemez veya bilgilerini değiştiremezsiniz!";

            FirmUser k = new FirmUser();//Kullanıcı ekleme
            k.FirmID = magazaId;
            return View(k);
        }

        [HttpPost]
        public ActionResult AddUser(FirmUser kat, String yetkiF)
        {
            setViewBags();

            Model m = new Model();
            FirmUser k = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == kat.FirmUserID);
            if (k == null)//Kullanıcı yok,Ekleme yapılacak
            {
                if (kat.username == null || kat.username == "" || kat.pw == "" || kat.pw == null || kat.mail == "" || kat.mail == null)
                {
                    TempData["RqErr"] = "Lütfen zorunlu olan bütün alanları doldurun.";
                    return RedirectToAction("AddUser");
                }
                else if (kat.username.Length < 4 || kat.pw.Length < 4)
                {
                    TempData["RqErr"] = "Lütfen kullanıcı adı ve şifreyi 4 karakterden büyük seçin.";
                    return RedirectToAction("AddUser");
                }

                FirmUser k2 = m.FirmUsers.FirstOrDefault(x => x.username == kat.username || x.mail == kat.mail);
                if (k2 == null) // username uygun.
                {
                    int mId = (int)kat.FirmID;
                    string yetkiType = yetkiF;
                    int yetki;
                    if (yetkiType == "Personel")
                        yetki = 0;
                    else
                        yetki = 1;

                    kat.NetState = 0;
                    kat.DateJoin = DateTime.Now;
                    kat.Rank = yetki;
                    m.FirmUsers.Add(kat);


                    if (yetki == 1)
                    {
                        Firm getM = m.Firms.FirstOrDefault(x => x.FirmID == mId);
                        getM.PartnerCount += 1;

                        //yeni ortak eklendikten sonra bütün ortakların durumlarının sıfırlanması.
                        List<FirmUser> allPayds = m.FirmUsers.Where(x=> x.FirmID == mId).ToList();
                        foreach (FirmUser itemKl in allPayds)
                        {
                            itemKl.NetState = 0;
                        }
                    }

                    m.SaveChanges();

                    TempData["Y"] = "2";
                    return RedirectToAction("Index", "Magaza");
                }
                else //username uygun değil.
                {
                    if (k2.username == kat.username)
                        TempData["Y"] = "3";
                    else
                        TempData["Y"] = "5";
                    return RedirectToAction("AddUser");
                }
            }
            else //kullanıcı var,güncelleme yapılacak.
            {
                if (kat.username == null || kat.username == "" || kat.pw == "" || kat.pw == null || kat.mail == "" || kat.mail == null)
                {
                    TempData["RqErr"] = "Lütfen zorunlu olan bütün alanları doldurun.";
                    return RedirectToAction("GuncelleUser", new { id = kat.FirmUserID });
                }
                else if (kat.username.Length < 4 || kat.pw.Length < 4)
                {
                    TempData["RqErr"] = "Lütfen kullanıcı adı ve şifreyi 4 karakterden büyük seçin.";
                    return RedirectToAction("AddUser");
                }

                FirmUser k2 = m.FirmUsers.FirstOrDefault(x => (x.username == kat.username || x.mail == kat.mail) && x.FirmUserID != kat.FirmUserID);
                if (k2 == null) // username uygun.
                {
                    int mId = (int)kat.FirmID;
                    int oldY = k.Rank;
                    k.username = kat.username;
                    k.pw = kat.pw;
                    k.mail = kat.mail;
                    string yetkiType = yetkiF;
                    int yetki;
                    if (yetkiType == "Personel")
                        yetki = 0;
                    else
                        yetki = 1;
                    k.Rank = yetki;
                    TempData["Y"] = "4";

                    if (oldY == 0 && k.Rank == 1)
                    {
                        Firm getM = m.Firms.FirstOrDefault(x => x.FirmID == mId);
                        getM.PartnerCount += 1;

                        //yeni ortak eklendikten sonra bütün ortakların durumlarının sıfırlanması.
                        List<FirmUser> allPayds = m.FirmUsers.Where(x=> x.FirmID == mId).ToList();
                        foreach (FirmUser itemKl in allPayds)
                        {
                            itemKl.NetState = 0;
                        }
                    }
                }
                else
                {
                    if (k2.username == kat.username)
                        TempData["Y"] = "3";
                    else
                        TempData["Y"] = "5";
                    return RedirectToAction("GuncelleUser", new { id = kat.FirmUserID });
                }

            }
            m.SaveChanges();
            return RedirectToAction("Index", "Magaza");
        }

        public ActionResult GuncelleUser(int id)
        {
            Model m = new Model();

            FirmUser kat = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == id);
            setViewBags();
            //istek dışı erişimin engellenmesı;
            if (ViewBag.kisiYetki == -1)//Demo hesap kontrolü
                return RedirectToAction("Index");
            int tempMCID = ViewBag.magazaID;
            if (kat != null)
            {
                if (kat.FirmID != tempMCID || kat.Rank != 0 || ViewBag.kisiYetki != 1)
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

            if ((string)TempData["Y"] == "3")
                ViewBag.FailMsg = "Bu kullanıcı adına sahip bir kullanıcı var.Lütfen başka bir kullanıcı adı deneyin.";
            else if ((string)TempData["Y"] == "5")
                ViewBag.FailMsg = "Bu maile sahip bir kullanıcı var.Lütfen başka bir mail deneyin.";
            else
                ViewBag.WarnMsg = "Firmanıza ortak olarak kaydettiğiniz kişiyi bir daha silemez veya bilgilerini değiştiremezsiniz!";

            ViewBag.ortaklar = kat.Firm.PartnerCount;


            if (kat == null)
                return RedirectToAction("Index", "Magaza");
            else
            {
                return View("AddUser", kat);
            }
        }

        [HttpPost]
        public ActionResult SilUser(int id)
        {
            Model m = new Model();
            FirmUser k = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == id);

            try
            {
                int mId = (int)k.FirmID;
                int yetki = (int)k.Rank;

                List<Income> chcGel = m.Incomes.SqlQuery("select * from Income where FirmUserID=" + id).ToList();
                List<Expense> chcGid = m.Expenses.SqlQuery("select * from Expense where FirmUser=" + id).ToList();
                List<Article> chcArt = m.Articles.SqlQuery("select * from Article where FirmUserID=" + id).ToList();

                if (chcGel.Count != 0 || chcGid.Count != 0 || chcArt.Count != 0)
                {
                    k.FirmID = null;
                }
                else
                    m.FirmUsers.Remove(k);



                if (yetki == 1)
                {
                    Firm getM = m.Firms.FirstOrDefault(x => x.FirmID == mId);
                    getM.PartnerCount -= 1;
                }

                m.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }
    }
}