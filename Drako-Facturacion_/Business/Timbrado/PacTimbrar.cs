using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drako_Facturacion.Business.Timbrado
{
    public abstract class PacTimbrar
    {
        public string Error { get; set; }

        public byte[] XMLTimbrado { get; set; }
        public abstract bool Timbrar(byte[] bXml);

    }
}