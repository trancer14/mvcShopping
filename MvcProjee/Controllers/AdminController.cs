using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcProjee.Models;

namespace MvcProjee.Controllers
{
    public class AdminController : Controller
    {
        Database1Entities veri = new Database1Entities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(userlar kisi)
        {
            var varmi = from nesne in veri.userlar where nesne.userad == kisi.userad && nesne.usersifre == kisi.usersifre && nesne.userrol != 1 select nesne;
            if (varmi.Any())
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.hata = "Kullanıcı Adı ve Şifre Hatalı";
                return View();
            }
        }
        public ActionResult TumListe()
        {
            //var kisiler = veri.userlar.ToList();
            //select * from userlar join roller on userlar.userrol=roller.rolid
            var kisiler = from n1 in veri.userlar
                          join n2 in veri.roller on n1.userrol equals n2.rolid
                          select new kullanicilar
                          {
                              userid = n1.userid,
                              userad = n1.userad,
                              usersifre = n1.usersifre,
                              userrol = n2.rolad
                          };
            return View(kisiler);
        }
        public ActionResult KullaniciGuncelle(int id)
        {
            //var kisi = veri.userlar.Find(id);
            var kisi = from n1 in veri.userlar join n2 in veri.roller on n1.userrol equals n2.rolid where n1.userid == id select new kullanicilar {
                userid=n1.userid,
                userad=n1.userad,
                usersifre=n1.usersifre,
                userrol=n2.rolad
            };
            int rolid = (from n1 in veri.userlar where n1.userid == id select n1.userrol).SingleOrDefault();
            ViewBag.rolum = from nesnes in veri.roller where nesnes.rolid == rolid select nesnes.rolid;
            ViewBag.roller = from nesne in veri.roller where nesne.rolid != rolid select nesne;
            return View(kisi.SingleOrDefault());

        }
        [HttpPost]
        public ActionResult KullaniciGuncelle(kullanicilar kisi)
        {
            var varolan = veri.userlar.Find(kisi.userid);
            varolan.userad = kisi.userad;
            varolan.usersifre = kisi.usersifre;
            varolan.userrol = Convert.ToInt32(kisi.userrol);

            veri.SaveChanges();
            return RedirectToAction("TumListe");
        }
        public ActionResult KullaniciSil(int? id)
        {
            if (id != null)
            {
                var silinecek = veri.userlar.Find(id);
                if (silinecek != null)
                {
                    veri.userlar.Remove(silinecek);
                    veri.SaveChanges();
                    Session["hata"]= "Kullanıcı Silindi";
                }
                else
                {
                    Session["hata"] = "Geçersiz Kullanıcı. Silme İşlemi Başarısız";
                }
                return RedirectToAction("TumListe");
            }
            else
            {
                Session["hata"] = "Silme İşlemi Başarısız";
                return RedirectToAction("TumListe");
            }
        }
        public ActionResult Yorumlar()
        {
            var yorumlarim = from n1 in veri.yorumlar
                             join n2 in veri.userlar on n1.yorumuser equals n2.userid
                             join n3 in veri.urunler on n1.yorumurun equals n3.urunid
                             select new yorum
                             {
                                 yorumid = n1.yorumid,
                                 yorummetin = n1.yorummetin,
                                 yorumpuan = n1.yorumpuan,
                                 yorumonay = n1.yorumonay,
                                 yorumtarih = n1.yorumtarih.ToString(),
                                 yorumurunad = n3.urunad,
                                 yorumuserad = n2.userad,
                                 yorumaltid = 0

            
                             };
            var altyorumlar = from n1 in veri.yorumlar
                              join n2 in veri.altyorum on n1.yorumid equals n2.altyorumustid
                              join n3 in veri.userlar on n2.altyorumuser equals n3.userid
                              select new List<string>
                              {
                                  n2.altyorumid.ToString(),
                                  n2.altyorumustid.ToString(),
                                  n2.altyorummetin,
                                  n2.altyorumonay.ToString(),
                                  n2.altyorumpuan.ToString(),
                                  n2.altyorumtarih.ToString(),
                                  n3.userad
                              };

            ViewBag.altyorumlar = altyorumlar;
            return View(yorumlarim);
        }
        public ActionResult YorumOnay(int id)
        {
            var yorum = veri.yorumlar.Find(id);
            yorum.yorumonay = 1;
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
        public ActionResult AltYorumOnay(int id)
        {
            var altyorum = veri.altyorum.Find(id);
            altyorum.altyorumonay = 1;
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
        public ActionResult YorumOnayKaldir(int id)
        {
            var yorum = veri.yorumlar.Find(id);
            yorum.yorumonay = 0;
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
        public ActionResult AltYorumOnayKaldir(int id)
        {
            var altyorum = veri.altyorum.Find(id);
            altyorum.altyorumonay = 0;
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");

        }
        public ActionResult YorumSil(int id)
        {
            var yorum = veri.yorumlar.Find(id);
            veri.yorumlar.Remove(yorum);
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");

        }
        public ActionResult AltYorumSil(int id)
        {
            var altyorum = veri.altyorum.Find(id);
            veri.altyorum.Remove(altyorum);
            veri.SaveChanges();
            return RedirectToAction("Yorumlar");
        }
    }
}