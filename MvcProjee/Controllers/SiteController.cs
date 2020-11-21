using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcProjee.Models;

namespace MvcProjee.Controllers
{
    public class SiteController : Controller
    {
        Database1Entities veri = new Database1Entities();

        public void UrunAdet()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            var uruncesidi = from nesne in veri.sepette where nesne.suserid == userid select nesne;
            int cesit = uruncesidi.Count();
            Session["cesit"] = cesit;
        }
        public void GenelToplam()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            var sepetimdekiler = (from nesne in veri.sepette join nesne2 in veri.urunler on nesne.surunid equals nesne2.urunid where nesne.suserid == userid select nesne.sadet * nesne2.urunfiyat);
            int toplam =(int)sepetimdekiler.Sum();
            Session["toplam"] = toplam;
        }
        // GET: Site
        public ActionResult Index()
        {
            UrunAdet();
            return View();
        }

        public ActionResult KisiKaydet()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KisiKaydet(userlar kisi)
        {
            var varmi = from nesne in veri.userlar where nesne.userad == kisi.userad select nesne;
            if (varmi.Any())
            {
                ViewBag.hata = "Kullanıcı adı Kayıtlı";
                return View();
            }
            else
            {
                veri.userlar.Add(kisi);
                veri.SaveChanges();
                return Redirect("/Login/Index");
            }
        }
        public ActionResult Urunler(int id=0)
        {
            //var urunler = veri.urunler.ToList();
            int atlanacakSatir = 0;
            if (id!=0)
            {
                atlanacakSatir = (id-1)*5;
            }
            ViewBag.UrunSayisi = Math.Ceiling(Convert.ToDecimal(veri.urunler.Count())/5);
            ViewBag.anlikSayfa = id;
            if (id == 0)
            {
                Session["anlik"] = 0;
            }
            else
            {
                Session["anlik"] = 1;
            }
            var urunler = veri.urunler.OrderBy(n=>n.urunid).Skip(atlanacakSatir).Take(5).ToList();
            return View(urunler);
        }
        public ActionResult Detay(int? id)
        {
            if (id != null)
            {
                var urun = veri.urunler.Find(id);
                if (urun != null)
                {
                    var kategoriad = from nesne in veri.urunler join nesne2 in veri.kategori on nesne.urunkateg equals nesne2.katid where nesne.urunid == id select nesne2;
                    ViewBag.kategori = kategoriad.SingleOrDefault().katad;
                    ViewBag.urunid = id;
                    var yorumlar = from nesne in veri.yorumlar join n2 in veri.userlar on nesne.yorumuser equals n2.userid where nesne.yorumurun == id && nesne.yorumonay == 1 select new List<string> {
                        nesne.yorummetin,
                        nesne.yorumpuan.ToString(),
                        nesne.yorumtarih.ToString(),
                        n2.userad,
                        nesne.yorumid.ToString()

                    };


                    ViewBag.yorumlar = yorumlar.ToList();
                    
                        //var yorumOrtalama = (from nesne in veri.yorumlar where nesne.yorumurun == id && nesne.yorumonay == 1 select nesne.yorumpuan).Average();
                        //ViewBag.puanYildiz = (int)yorumOrtalama;
                        ViewBag.puanYildiz = (int)5;
                                            

                        var altyorumlar = from n1 in veri.altyorum
                                          join n2
                 in veri.yorumlar on n1.altyorumustid equals n2.yorumid
                                          join n3 in veri.userlar on n1.altyorumuser equals n3.userid
                                          where n1.altyorumonay == 1 && n2.yorumonay == 1
                                          select new List<string>
                                      {
                                          n1.altyorumid.ToString(),
                                          n3.userad,
                                          n1.altyorummetin,
                                          n1.altyorumpuan.ToString(),
                                          n1.altyorumtarih.ToString(),
                                          n1.altyorumustid.ToString()
                                      };
                        var alt = from n1 in veri.yorumlar
                                  join n2 in veri.altyorum on n1.yorumid equals n2.altyorumustid
                                  where n2.altyorumonay == 1
                                  select new
                                  {
                                      n1.yorumid,
                                      n2.altyorumid
                                  };
                        var dalt = alt.GroupBy(n1 => n1.yorumid).Select(yeni => new List<int> { yeni.FirstOrDefault().yorumid, yeni.Count() }).OrderBy(n2 => n2.FirstOrDefault()).ToList();
                        ViewBag.dalt = dalt;

                        ViewBag.altyorumlarim = altyorumlar;
                    
                    return View(urun);
                }
                else
                {
                    return RedirectToAction("Urunler");
                }
            }
            else
            {
                return RedirectToAction("Urunler");
            }
        }
        public ActionResult YorumEkle(yorumlar yorum)
        {
            yorumlar eklenen = new yorumlar();
            eklenen.yorumurun=yorum.yorumurun;
            eklenen.yorumpuan =Convert.ToInt32( Request.Cookies["puan"].Value);
            eklenen.yorumtarih = DateTime.Now.Date;
            eklenen.yorummetin = yorum.yorummetin;
            eklenen.yorumonay = 0;
            eklenen.yorumuser = Convert.ToInt32(Session["userid"]);
            veri.yorumlar.Add(eklenen);
            veri.SaveChanges();

            return RedirectToAction("Detay");
        }
        public ActionResult Cevapla(altyorum degerler)
        {
            altyorum _altyorum = new altyorum();
            _altyorum.altyorummetin = degerler.altyorummetin;
            _altyorum.altyorumpuan = Convert.ToInt32(Request.Cookies["puan"].Value);
            _altyorum.altyorumuser = Convert.ToInt32(Session["userid"]);
            _altyorum.altyorumtarih = DateTime.Now.Date;
            _altyorum.altyorumustid = degerler.altyorumustid;
            _altyorum.altyorumonay = 0;
            veri.altyorum.Add(_altyorum);
            veri.SaveChanges();

            return RedirectToAction("Detay");

        }
      

        public ActionResult SepetEkle(int id)
        {
            if (id != null)
            {
                int userid = Convert.ToInt32(Session["userid"]);
                var eklenecekurun = veri.urunler.Find(id);
                if (eklenecekurun != null)
                {
                   
                    var eklenmismi = from nesne in veri.sepette where nesne.suserid == userid && nesne.surunid == id select nesne;
                    if (eklenmismi.Any())
                    {
                        eklenmismi.SingleOrDefault().sadet = eklenmismi.SingleOrDefault().sadet+1;
                        veri.SaveChanges();
                    }
                    else
                    {
                        sepette eklenen = new sepette();
                        eklenen.suserid = Convert.ToInt32(Session["userid"]);
                        eklenen.surunid = id;
                        eklenen.sadet = 1;
                        veri.sepette.Add(eklenen);
                        veri.SaveChanges();
                    }
                    
                }
                UrunAdet();
             
                return RedirectToAction("Urunler");

            }
            else
            {
                return RedirectToAction("Urunler");
            }

        }
        public ActionResult Sepet()
        {
            var sepettekiler = from nesne in veri.sepette join nesne2 in veri.urunler on nesne.surunid equals nesne2.urunid select new sepetim {
                sepetid=nesne.sepetid,
                sepetadet=nesne.sadet,
                sepetfiyat=(int)nesne2.urunfiyat,
                sepetresim=nesne2.urunresim,
                sepeturun=nesne2.urunad
            };
            GenelToplam();
            return View(sepettekiler);
        }
        public ActionResult SepetUrunSil(int? id)
        {
            if (id != null)
            {
                var silinecek = veri.sepette.Find(id);
                if (silinecek != null)
                {
                    veri.sepette.Remove(silinecek);
                    veri.SaveChanges();
                    UrunAdet();
                }
                GenelToplam();
                return RedirectToAction("Sepet");
            }
            else
            {
                return RedirectToAction("Sepet");
            }

        }
        public ActionResult Arttir(int? id)
        {
            if (id != null)
            {
                var artan = veri.sepette.Find(id);
                if (artan != null)
                {
                    artan.sadet = artan.sadet + 1;
                    veri.SaveChanges();
                }
                GenelToplam();
                return RedirectToAction("Sepet");
            }
            else
            {
                return RedirectToAction("Sepet");
            }

        }
        public ActionResult Azalt(int? id)
        {
            if (id != null)
            {
                var azalan = veri.sepette.Find(id);
                if (azalan != null && azalan.sadet - 1 != 0)
                {
                    azalan.sadet = azalan.sadet - 1;

                    veri.SaveChanges();
                }
                else if(azalan !=null)
                {
                    veri.sepette.Remove(azalan);
                    veri.SaveChanges();
                    UrunAdet();
                }
                GenelToplam();
                return RedirectToAction("Sepet");

            }
            else
            {
                return RedirectToAction("Sepet");
            }
        }
    }
}