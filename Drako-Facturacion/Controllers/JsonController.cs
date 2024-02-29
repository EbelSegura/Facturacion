using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Drako_Facturacion.Controllers
{
    public class JsonController : Controller
    {
        public class ElementJson
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        [HttpPost]
        public JsonResult ProductosServicios(string cad)
        {
            List<ElementJson> lst = new List<ElementJson>();
            using (Drako_Facturacion.Models.SAT.SATEntities db =
                new Models.SAT.SATEntities())
            {
                lst = (from d in db.Cat_ClaveProdServ
                       where d.Codigo.Contains(cad) || d.Descripcion.Contains(cad)
                       select new ElementJson
                       {
                           Id = d.Descripcion,
                           Value = (d.Codigo + " " + d.Descripcion)
                       }).Take(20).ToList();
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ClaveUnidad(string cad)
        {
            List<ElementJson> lst = new List<ElementJson>();
            using (Drako_Facturacion.Models.SAT.SATEntities db =
                new Models.SAT.SATEntities())
            {
                lst = (from d in db.Cat_UMedida
                       where d.Codigo.Contains(cad) || d.Nombre.Contains(cad)
                       select new ElementJson
                       {
                           Id = d.Nombre,
                           Value = (d.Codigo + " " + d.Nombre)
                       }).Take(20).ToList();
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

    }
}