using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Drako_Facturacion
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Filtrers.VerifySession());
        }
    }
}
