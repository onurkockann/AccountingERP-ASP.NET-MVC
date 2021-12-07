using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountingERP.Models;

namespace AccountingERP.Controllers
{
    public class NetDurumController : Controller
    {
        public void setViewBags()
        {
            //Viewbaglerin ayarlanması
            Model vb = new Model();
            FirmUser u = vb.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;
                ViewBag.uID = u.FirmUserID;
            }
        }

        public void setPaydas()
        {
            Model m = new Model();
            //Paydaşları çekme
            int fID = ViewBag.magazaID;
            List<FirmUser> pd = m.FirmUsers.Where(x=> x.FirmID == fID).ToList();
            List<SelectListItem> itemsP = new List<SelectListItem>();
            foreach (FirmUser itemP in pd)
            {
                if (itemP.Rank == 1)
                {
                    itemsP.Add(new SelectListItem { Text = itemP.username, Value = itemP.FirmUserID.ToString() });
                }
            }

            if (ViewBag.uYetki == -1)
                itemsP.Add(new SelectListItem { Text = "Demo", Value = "Demo" });


            ViewBag.Paydaslar = itemsP;
            ViewBag.PaydaslarSec = itemsP;

        }

        // GET: NetDurum
        public ActionResult Index()
        {
            if ((string)TempData["Y"] == "1")
                ViewBag.FailMsg = "Aynı kullanıcı arasında para aktarımı gerçekleştiremezsiniz";
            else if ((string)TempData["YAL"] == "1")
                ViewBag.DoneMsg = "Elden para aktarım işlemi başarıyla gerçekleştirildi ve paydaşlar arası genel durum tablosuna yansıtıldı.";

            //Viewbaglerin ayarlanması
            Model m = new Model();
            FirmUser u = m.FirmUsers.FirstOrDefault(x => x.username == HttpContext.User.Identity.Name);
            if (u != null)
            {
                ViewBag.magaza = u.Firm.FirmName;
                ViewBag.magazaID = u.Firm.FirmID;
            }

            //toplam gelirler

            List<Income> gelir = m.Incomes.Where(x=> x.FirmID == u.Firm.FirmID).ToList();
            double toplamGelir = 0;

            foreach (Income item in gelir)
            {
                toplamGelir += (double)item.Amount;
            }

            ViewBag.topGelir = toplamGelir.ToString();


            //toplam giderler
            List<Expense> gider = m.Expenses.Where(x => x.FirmID == u.Firm.FirmID).ToList();
            double toplamGider = 0;

            foreach (Expense item in gider)
            {
                toplamGider += (double)item.Amount;
            }

            ViewBag.topGider = toplamGider.ToString();


            //Net durumun ayarlanması
            ViewBag.MNetDurum = (double)(toplamGelir - toplamGider);
            ViewBag.MNetDurum = Math.Round((double)ViewBag.MNetDurum, 3);

            //Paydaş sayısı
            ViewBag.PCount = u.Firm.PartnerCount;

            setPaydas();
            List<FirmUser> us = m.FirmUsers.Where(x => x.FirmID == u.FirmID).ToList();
            return View(us);
        }

        [HttpPost]
        public ActionResult EkleSG(String MiktarSG, String Paydaslar, String PaydaslarSec)
        {
            if (Paydaslar == PaydaslarSec)
            {
                TempData["Y"] = "1";
                return RedirectToAction("Index");
            }

            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";

            Model m = new Model();
            setViewBags();
            int mgzID = ViewBag.magazaID;

            MTIPartner epara = new MTIPartner();
            epara.Amount = Convert.ToDouble(MiktarSG, ci);
            epara.EventDate = DateTime.Now;
            epara.FirmID = mgzID;
            epara.SenderID = Convert.ToInt32(Paydaslar);
            epara.ReceiverID = Convert.ToInt32(PaydaslarSec);
            epara.EventState = 1;

            m.MTIPartners.Add(epara);

            FirmUser verenU = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == epara.SenderID);
            FirmUser alanU = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == epara.ReceiverID);

            verenU.NetState += epara.Amount;
            alanU.NetState -= epara.Amount;

            verenU.NetState = Math.Round((double)verenU.NetState, 3);
            alanU.NetState = Math.Round((double)alanU.NetState, 3);

            //Post at
            Firm mgz = m.Firms.FirstOrDefault(x => x.FirmID == mgzID);

            Article art = new Article();
            art.Content = epara.EventDate.ToString() + " Tarihinde " + epara.Amount.ToString() + " TL " +
                " Miktarındaki elden para aktarım işlemi gerçekleştirilmiştir. Bu para aktarım işlemi " + m.FirmUsers.FirstOrDefault(x=> x.FirmUserID == epara.SenderID).username.ToString() +
                " kullanıcı adına sahip paydaştan " +
                m.FirmUsers.FirstOrDefault(x => x.FirmUserID == epara.ReceiverID).username.ToString() + 
                " kullanıcı adına sahip paydaşa aktarılarak gerçekleştirildi. Ortaklar arası son durumu incelemek için lütfen Muhasebe bölümüne bakın.";
            art.EventDate = DateTime.Now;
            art.FirmID = epara.FirmID;

            m.Articles.Add(art);

            m.SaveChanges();
            TempData["YAL"] = "1";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SilSG(int id)
        {
            Model m = new Model();
            MTIPartner k = m.MTIPartners.FirstOrDefault(x => x.MTIPartnersID == id);

            try
            {
                FirmUser verenU = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.SenderID);
                FirmUser alanU = m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.ReceiverID);

                if (k.EventState != 0)
                {
                    verenU.NetState -= k.Amount;
                    alanU.NetState += k.Amount;

                    verenU.NetState = Math.Round((double)verenU.NetState, 3);
                    alanU.NetState = Math.Round((double)alanU.NetState, 3);

                    Article art = new Article();
                    art.Content = k.EventDate.ToString() + " Tarihinde gerçekleştirilen " + k.Amount.ToString() + " TL " +
                        " Miktarındaki elden para aktarım işlemi silinmiştir. Bu para aktarım işlemi " +
                        m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.SenderID).username.ToString() + 
                        " kullanıcı adına sahip paydaştan " +
                        m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.ReceiverID).username.ToString() + 
                        " kullanıcı adına sahip paydaşa aktarılarak gerçekleştirilmişti. Ortaklar arası son durumu incelemek için lütfen Muhasebe bölümüne bakın.";
                    art.EventDate = DateTime.Now;
                    art.FirmID = k.FirmID;

                    m.Articles.Add(art);
                }
                else
                {
                    Article art = new Article();
                    art.Content = k.EventDate.ToString() + " Tarihinde gerçekleştirilen " + k.Amount.ToString() + " TL "+
                        " Miktarındaki elden para aktarım işlemi silinmiştir. Bu para aktarım işlemi " +
                        m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.SenderID).username.ToString() + " kullanıcı adına sahip paydaştan " +
                        m.FirmUsers.FirstOrDefault(x => x.FirmUserID == k.ReceiverID).username.ToString() + " kullanıcı adına sahip paydaşa aktarılarak gerçekleştirilmişti. Önemli: Bu elden para aktarım işlemi, geçmiş ortaklık arasında gerçekleştiği için mevcut ortaklar arasındaki net tabloya yansıtılmamıştır.";
                    art.EventDate = DateTime.Now;
                    art.FirmID = k.FirmID;

                    m.Articles.Add(art);
                }



                m.MTIPartners.Remove(k);
                m.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Index");
        }
    }
}