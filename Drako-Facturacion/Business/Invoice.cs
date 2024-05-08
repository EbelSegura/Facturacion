using Drako_Facturacion.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using static Drako_Facturacion.Models.ViewModels.FacturaViewModel;
using System.Drawing;
using System.IO;
using Drako_Facturacion.Utils;

namespace Drako_Facturacion.Business
{
    public class Invoice
    {
        FacturaViewModel oFactura;
        Comprobante oComprobante;
        private string error = "";
        private string pathXML = @"C:\Users\ebels\source\repos\Drako-Facturacion\FACTURA.xml";
        private string pathCadenaOriginal = @"C:\Users\ebels\source\repos\Drako-Facturacion\xslt4.0\cadenaoriginal_4_0.xslt";
        private string pathCer = @"C:\Users\ebels\source\repos\Drako-Facturacion\Drako-Facturacion\Files\CSD_Sucursal_1_EKU9003173C9_20230517_223850.cer";
        private string pathKey = @"C:\Users\ebels\source\repos\Drako-Facturacion\Drako-Facturacion\Files\CSD_Sucursal_1_EKU9003173C9_20230517_223850.key";
        #region Propiedades
        public string Error
        {
            get
            {
                return Error;
            }
        }
        public bool IsError
        {
            get
            {
                if (error != "")
                    return true;
                return false;
            }
        }
        #endregion

        public Invoice(FacturaViewModel oFactura)
        {
            this.oFactura = oFactura;
            oComprobante = new Comprobante();
        }

        public void Create()
        {
            CreateXML();
            SellarXML();
            TimbrarXML();
        }

        private void CreateXML()
        {
            oComprobante.Version = "4.0";
            oComprobante.Serie = oFactura.Serie;
            oComprobante.Folio = oFactura.Folio.ToString();
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //oComprobante.Sello = ""; //FALTANTE
            oComprobante.FormaPago = "99";
            //oComprobante.NoCertificado = numerocertificado; PENDIENTE
            //oComprobante.Certificado = ""; //FALTANTE
            //oComprobante.SubTotal = 10m; SE VAN A CALCULAR
            //oComprobante.Descuento = 1m; SE VAN A CALCULAR
            oComprobante.Moneda = oFactura.Moneda;
            //oComprobante.Total = 9; SE VA A CALCULAR
            oComprobante.TipoDeComprobante = "I";
            oComprobante.MetodoPago = oFactura.TipoDePago;
            oComprobante.LugarExpedicion = "54720";

            ComprobanteEmisor oEmisor = new ComprobanteEmisor();
            oEmisor.Rfc = "AEN001011AJ9";
            oEmisor.Nombre = "ARPON ENTERPRISE";
            oEmisor.RegimenFiscal = "605";

            ComprobanteReceptor oReceptor = new ComprobanteReceptor();
            oReceptor.Rfc = oFactura.RazonSocial;
            oReceptor.Nombre = oFactura.RFCCliente;
            oReceptor.DomicilioFiscalReceptor = oFactura.UsoCFDI;

            //ASIGNO EMISOR Y RECEPTOR
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;

            List<ComprobanteConcepto> lstConceptos = new List<ComprobanteConcepto>();
            foreach (Concepto oConceptoVM in oFactura.conceptos)
            {
                ComprobanteConcepto oConcepto = new ComprobanteConcepto();
                oConcepto.ClaveProdServ = oConceptoVM.claveProducto;
                oConcepto.Cantidad = oConceptoVM.cantidad;
                oConcepto.ClaveUnidad = oConceptoVM.claveUnidad;
                oConcepto.Descripcion = oConceptoVM.descripcion;
                oConcepto.ValorUnitario = oConceptoVM.precioUnitario;
                oConcepto.Descuento = oConceptoVM.descuento == null ? 0 : (decimal)oConceptoVM.descuento;
                oConcepto.Importe = (oConceptoVM.cantidad * oConceptoVM.precioUnitario);
                oConcepto.ObjetoImp = "02";
                lstConceptos.Add(oConcepto);
            }

            oComprobante.Conceptos = lstConceptos.ToArray();

            CreateXMLFile();

        }

        private void SellarXML()
        {
            string CadenaOriginal = "";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathCadenaOriginal);

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
            {
                transformador.Transform(pathXML, xwo);
                CadenaOriginal = sw.ToString();
            }
            SelloDigital oSelloDigital = new SelloDigital();
            oComprobante.Certificado = oSelloDigital.Certificado(pathCer);
            oComprobante.Sello = oSelloDigital.Sellar(CadenaOriginal, pathKey, oFactura.ClavePrivada);

            CreateXMLFile();
        }

        private void TimbrarXML()
        {

        }

        #region Helpers
        private void CreateXMLFile()
        {
            //SERIALIZAMOS---------------------------------------------------------------------------------

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
            xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

            string sXml = "";

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }
            }


            //GUARADMOS EL STRING EN UN ARCHIVO
            System.IO.File.WriteAllText(pathXML, sXml);
        }

        #endregion
    }
}