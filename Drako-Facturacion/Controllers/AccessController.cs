using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Drako_Facturacion.Models;

namespace Drako_Facturacion.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                    using (FacturacionEntities db = new FacturacionEntities())
                {
                    string ePass = Utils.Encrypt.GetSHA1(password);
                    var lstUser=from d in db.users
                              where d.email == email && d.idState == 1
                              && d.passwords == ePass
                              select d;

                    if(lstUser.Count() > 0)
                    {
                        var oUser = lstUser.First();
                        Session["Users"] = oUser;

                        return Content("1");
                    }
                    else
                    {
                        return Content("Usuario o contraseña incorrecta");
                    }
                }


            }catch (Exception ex)
            {
                return Content("Ocurrio un error");
            }
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            Session["Users"] = null;

            return Content("1");
        }

    }
}