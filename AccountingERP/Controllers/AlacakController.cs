using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class AlacakController : Controller
    {
        public void setViewBags()
        {
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;
            }

        }

        public void setNetDurum(int magazaid)
        {
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
        // GET: Alacak
        public ActionResult Index()
        {
            setViewBags();

            if ((string)TempData["Y"] == "1")
                ViewBag.DoneMsg = "Alacak Notu Eklendi.";
            else if ((string)TempData["Y"] == "2")
                ViewBag.DoneMsg = "Verecek Notu Eklendi.";

            return View();
        }

        [HttpPost]
        public ActionResult EkleAl(String Aciklama, String Miktar)
        {
            setViewBags();
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            Model m = new Model();
            Payee alacakN = new Payee();

            alacakN.Description = Aciklama;
            alacakN.Amount = Convert.ToDouble(Miktar, ci);
            alacakN.FirmID = ViewBag.magazaID;

            m.Payees.Add(alacakN);
            m.SaveChanges();

            TempData["Y"] = "1";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SilAl(int id)
        {
            Model m = new Model();
            Payee k = m.Payees.FirstOrDefault(x => x.PayeesID == id);

            try
            {
                m.Payees.Remove(k);
                m.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DoneAl(int id)
        {
            Model m = new Model();
            Payee k = m.Payees.FirstOrDefault(x => x.PayeesID == id);

            try
            {
                Income gelirt = new Income();
                gelirt.Amount = (double)k.Amount;
                gelirt.Description = "Alacak alındı=" + k.Description;
                gelirt.EventDate = DateTime.Now;
                Firm getmsb = m.Firms.FirstOrDefault(x => x.FirmID == k.FirmID);
                gelirt.Firm = getmsb;

                m.Incomes.Add(gelirt);
                m.Payees.Remove(k);
                m.SaveChanges();
                setNetDurum(getmsb.FirmID);
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EkleVer(String Aciklama, String Miktar)
        {
            setViewBags();
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            Model m = new Model();
            Debt verecekN = new Debt();

            verecekN.Description = Aciklama;
            verecekN.Amount = Convert.ToDouble(Miktar, ci);
            verecekN.FirmID = ViewBag.magazaID;

            m.Debts.Add(verecekN);
            m.SaveChanges();

            TempData["Y"] = "2";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SilVer(int id)
        {
            Model m = new Model();
            Debt k = m.Debts.FirstOrDefault(x => x.DebtsID == id);

            try
            {
                m.Debts.Remove(k);
                m.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DoneVer(int id)
        {
            Model m = new Model();
            Debt k = m.Debts.FirstOrDefault(x => x.DebtsID == id);

            try
            {
                Expense gidert = new Expense();
                gidert.Amount = (double)k.Amount;
                gidert.Description = "Borç ödendi=" + k.Description;
                gidert.EventDate = DateTime.Now;
                Firm getmsb = m.Firms.FirstOrDefault(x => x.FirmID == k.FirmID);
                gidert.Firm = getmsb;
                gidert.ExpenseTypesID = 4;

                m.Expenses.Add(gidert);
                m.Debts.Remove(k);
                m.SaveChanges();
                setNetDurum(getmsb.FirmID);
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }
    }
}