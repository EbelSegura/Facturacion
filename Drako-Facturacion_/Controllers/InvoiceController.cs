using Drako_Facturacion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Drako_Facturacion.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            FacturaViewModel model = new FacturaViewModel();

            model.Fecha = DateTime.Now;

            Combobox();

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(FacturaViewModel model)
        {
            try
            {

                Business.Invoice oInvoice = new Business.Invoice(model);
                oInvoice.Create();

                return Content("1");
            }
            catch (Exception ex)
            {

            }
            return Content("0");
        }

        #region Second ActionResult
        public ActionResult AddConcepto(Models.ViewModels.FacturaViewModel.Concepto oConcepto)
        {

            return View(oConcepto);
        }

        #endregion

        #region HELPERS

        private void Combobox()
        {
            //FORMA PAGO
            Models.SAT.SATEntities db2 = new Models.SAT.SATEntities();
            var lstFormaPago = from d in db2.FormasPago
                               select new { code = d.IdFormaPago, name = d.Descripcion };
            SelectList slcFormaPago = new SelectList(lstFormaPago, "code", "name");

            ViewBag.lstFormaPago = slcFormaPago;

            //TIPO PAGO
            var lstMetodoDePago = from d in db2.Cat_MetodoPago
                                select new { code = d.Codigo, name = d.Descripcion };
            SelectList slcMetodoDePago = new SelectList(lstMetodoDePago, "code", "name");

            ViewBag.lstMetodoDePago = slcMetodoDePago;

            //USO CFDI
            var lstUsoCfdi = from d in db2.Cat_UsoCFDi
                             select new { code = d.Codigo, name = d.Descripcion };
            SelectList slcUsoCfdi = new SelectList(lstUsoCfdi, "code", "name");

            ViewBag.lstUsoCfdi = slcUsoCfdi;

            //USO MONEDA
            var lstMoneda = from d in db2.Cat_Moneda
                            where d.Codigo == "MXN" || d.Codigo == "USD"
                            select new { code = d.Codigo, name = d.Codigo };
                            //select new { code = d.Codigo, name = d.Descripcion };
            SelectList slcMoneda = new SelectList(lstMoneda, "name", "code");

            ViewBag.lstMoneda = slcMoneda;

            //USO REGIMEN FISCAL
            var lstRegimenFiscal = from d in db2.Cat_Regimen
                            select new { code = d.Codigo, name = d.Descripcion };
            SelectList slcRegimenFiscal = new SelectList(lstRegimenFiscal, "code", "name");

            ViewBag.lstRegimenFiscal = slcRegimenFiscal;

        }

        #endregion


    }
}