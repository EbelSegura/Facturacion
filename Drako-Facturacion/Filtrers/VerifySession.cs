using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Drako_Facturacion.Controllers;
using Drako_Facturacion.Models;

namespace Drako_Facturacion.Filtrers
{
    public class VerifySession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            users oUser = (users)HttpContext.Current.Session["Users"];

            if (oUser == null)
            {
                if (filterContext.Controller is AccessController == false)
                {
                    filterContext.HttpContext.Response.Redirect("/Access/Index");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}