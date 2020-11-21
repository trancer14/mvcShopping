using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcProjee.Models;

namespace MvcProjee.Controllers
{
    public class LoginController : Controller
    {
        Database1Entities veri = new Database1Entities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(userlar kisi)
        {
            var varolanKisi = from nesne in veri.userlar where nesne.userad == kisi.userad && nesne.usersifre == kisi.usersifre select nesne;
            if (varolanKisi.Any())
            {
                Session["userid"] = varolanKisi.SingleOrDefault().userid;
                return Redirect("/Site/Index");
            }
            else
            {
                ViewBag.hata = "Kullanıcı Adı ve Şifre Hatalı";
                return View();
            }
        }
    }
}