using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;
using System.Globalization;

namespace AccountingERP.Controllers
{
    public class GelirController : Controller
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

        public void setNetDurum()
        {
            setViewBags();
            int magazaid = Convert.ToInt32(ViewBag.magazaID);

            Model m = new Model();
            List<Income> getGelir = m.Incomes.Where(x=> x.FirmID == magazaid).ToList();
            List<Expense> getGider = m.Expenses.Where(x => x.FirmID == magazaid).ToList();

            double topGelir = 0;
            double topGider = 0;

            foreach (Income itemG in getGelir)
            {
                topGelir += (double)itemG.Amount;
            }

            foreach (Expense itemGi in getGider)
            {
                topGider += (double)itemGi.Amount;
            }

            Firm getMuhasebe = m.Firms.FirstOrDefault(x => x.FirmID == magazaid);
            getMuhasebe.NetProfit = topGelir - topGider;
            getMuhasebe.NetProfit = Math.Round((double)getMuhasebe.NetProfit, 3);

            m.SaveChanges();

        }

        // GET: Gelir
        public ActionResult Index(int sayfa = 1)
        {
            setViewBags();

            if ((string)TempData["Y"] == "1")
                ViewBag.DoneMsg = "Yeni gelir eklendi.";
            else if ((string)TempData["Y"] == "2")
                ViewBag.DoneMsg = "Seçilen gelir güncellendi.";

            Model m = new Model();
            int fID = ViewBag.magazaID;
            List<Income> gelir = m.Incomes.Where(x=> x.FirmID == fID).ToList();

            gelir.Reverse();
            return View(gelir);
        }

        [HttpGet]
        public ActionResult Ekle()
        {
            setViewBags();

            Model m = new Model();
            //Paydaşları çekme
            List<FirmUser> pd = m.FirmUsers.ToList();
            List<SelectListItem> itemsP = new List<SelectListItem>();

            foreach (FirmUser itemP in pd)
            {
                if (itemP.FirmID == ViewBag.magazaID && itemP.Rank == 1)
                {
                    itemsP.Add(new SelectListItem { Text = itemP.username, Value = itemP.FirmUserID.ToString() });
                }
            }
            if (itemsP.Count > 1)
            {
                itemsP.Add(new SelectListItem { Text = "Ortak Dağılım", Value = "Ortak" });
                ViewBag.Paydaslar = itemsP;
            }
            else
            {
                ViewBag.Paydaslar = itemsP;
            }


            Income k = new Income();
            return View(k);
        }

        [HttpPost]
        public ActionResult Ekle(Income kat, String MiktarF, String Paydaslar)
        {
            setViewBags();
            Model m = new Model();
            Income k = m.Incomes.FirstOrDefault(x => x.IncomeID == kat.IncomeID);

            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            Firm getMsb = m.Firms.FirstOrDefault(x => x.FirmID == u.Firm.FirmID);

            int pID = Convert.ToInt32(Paydaslar);

            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            if (k == null)//Ekle
            {
                kat.Amount = Convert.ToDouble(MiktarF, ci);
                kat.EventDate = DateTime.Now;
                kat.FirmID = u.FirmID;
                kat.Description = kat.Description;
                kat.FirmUserID = pID;


                Article art = new Article();
                art.FirmID = u.FirmID;
                art.EventDate = DateTime.Now;
                art.Content = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss") + " Tarihinde " + kat.Amount.ToString() + " TL miktarında bir gelir elde edildi.\n";

                if (Paydaslar != "Ortak" && m.Firms.FirstOrDefault(x => x.FirmID == u.FirmID).PartnerCount > 1)//content mail atılacak
                {
                    int kulID = Convert.ToInt32(Paydaslar);
                    kat.FirmUserID = kulID;

                    double topMiktar = (double)kat.Amount;
                    double ortakCount = (double)u.Firm.PartnerCount;
                    double pay = topMiktar / ortakCount; //Herkesin ödemesi gereken miktar.

                    FirmUser getAkt = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == kulID);
                    art.Content += "Geliri elde eden kişi=" + getAkt.username.ToString() + "\nEski durum=>" + getAkt.username.ToString() + ":" + getAkt.NetState.ToString() + "TL";
                    getAkt.NetState -= pay * ((getMsb.PartnerCount) - 1);
                    getAkt.NetState = Math.Round((double)getAkt.NetState, 3);
                    art.Content += "\nYeni durum=>" + getAkt.username.ToString() + ":" + getAkt.NetState.ToString() + "TL";

                    List<FirmUser> others = m.FirmUsers.Where(x=> x.FirmID == u.FirmID).ToList();

                    foreach (FirmUser ot in others)
                    {
                        if (ot.Rank == 1 && ot != getAkt)
                        {
                            art.Content += "\nEski durum=>" + ot.username.ToString() + ":" + Math.Round((double)ot.NetState, 3).ToString() + "TL";
                            ot.NetState += pay;
                            ot.NetState = Math.Round((double)ot.NetState, 3);
                            art.Content += "\nYeni durum=>" + ot.username.ToString() + ":" + ot.NetState.ToString() + "TL";
                        }

                    }
                }


                m.Articles.Add(art);
                m.Incomes.Add(kat);

                TempData["Y"] = "1";
            }
            else//Güncelle
            {
                k.Description = kat.Description;
                TempData["Y"] = "2";
            }
            m.SaveChanges();
            setNetDurum();
            return RedirectToAction("Index");
        }

        public ActionResult Guncelle(int id)
        {
            setViewBags();

            Model m = new Model();
            Income kat = m.Incomes.FirstOrDefault(x => x.IncomeID == id);
            int tempMCID = ViewBag.magazaID;
            if (kat != null)
            {
                if (kat.Firm.FirmID != tempMCID)
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

            return View("Ekle", kat);
        }

        [HttpPost]
        public ActionResult Sil(int id)
        {
            Model m = new Model();
            Income k = m.Incomes.FirstOrDefault(x => x.IncomeID == id);

            Firm getMsb = m.Firms.FirstOrDefault(x => x.FirmID == k.Firm.FirmID);

            try
            {
                if (k.FirmUser != null && k.Firm.PartnerCount > 1 && k.FirmUser.Rank == 1)
                {
                    double topMiktar = (double)k.Amount;
                    double ortakCount = (double)k.Firm.PartnerCount;
                    double pay = topMiktar / ortakCount; //Herkesin cebine yatması gereken miktar.

                    FirmUser getAkt = k.FirmUser;
                    getAkt.NetState += pay * ((getMsb.PartnerCount) - 1);
                    getAkt.NetState = Math.Round((double)getAkt.NetState, 3);

                    List<FirmUser> others = m.FirmUsers.ToList();

                    foreach (FirmUser ot in others)
                    {
                        if (ot.FirmID == k.Firm.FirmID && ot.Rank == 1 && ot != getAkt)
                        {
                            ot.NetState -= pay;
                            ot.NetState = Math.Round((double)ot.NetState, 3);
                        }

                    }
                }

                //Post atma işlemi
                Article art = new Article();
                art.EventDate = DateTime.Now;
                art.FirmID = getMsb.FirmID;
                if (k.FirmUser != null)
                {
                    art.Content = k.EventDate.ToString() + " Tarihinde elde edilen " + k.Amount.ToString() + 
                        " TL miktarındaki gelir kaldırıldı.\nSilinen bu geliri " + k.FirmUser.username.ToString() + " kullanıcı adına sahip paydaş elde etmişti.";
                }
                else
                {
                    art.Content = k.EventDate.ToString() + " Tarihinde elde edilen " + k.Amount.ToString() + 
                        " TL miktarındaki gelir kaldırıldı";
                }

                m.Articles.Add(art);

                m.Incomes.Remove(k);
                getMsb.NetProfit -= k.Amount;

                m.SaveChanges();
                setNetDurum();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }
    }
}