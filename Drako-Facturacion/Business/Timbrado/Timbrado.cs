using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Drako_Facturacion.Business.Timbrado
{
    public class Timbrado : PacTimbrar
    {

        public override bool Timbrar(byte[] bXml)
        {
            switch (ConfigurationManager.AppSettings["Pac"])
            {
                case "1":
                    {
                        PacTimbrarFC oPacTimbrarFC = new PacTimbrarFC();
                        if (!oPacTimbrarFC.Timbrar(bXml))
                        {
                            Error = oPacTimbrarFC.Error;
                        }
                        else
                        {
                            XMLTimbrado = oPacTimbrarFC.XMLTimbrado;
                            return true;
                        }
                    }
                    break;


            }

            return false;
        }
    }
}