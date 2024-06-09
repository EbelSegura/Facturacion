using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Drako_Facturacion.Business.Timbrado
{
    public class PacTimbrarFC : PacTimbrar
    {
        public override bool Timbrar(byte[] bXml)
        {

            ServiceReference2.RespuestaCFDi respuestaCFDI = new ServiceReference2.RespuestaCFDi();

            ServiceReference2.TimbradoClient oTimbrado = new ServiceReference2.TimbradoClient();
            respuestaCFDI = oTimbrado.TimbrarTest(ConfigurationManager.AppSettings["FCUser"],
                ConfigurationManager.AppSettings["FCPassword"],
                bXml);

            if (respuestaCFDI.Documento == null)
                Error = respuestaCFDI.Mensaje;
            else
            {
                XMLTimbrado = respuestaCFDI.Documento;

                return true;
            }
            return false;

        }
    }
}