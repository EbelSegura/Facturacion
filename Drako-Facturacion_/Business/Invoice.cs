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
        private string pathXMLTimbrado = @"C:\Users\ebels\source\repos\Drako-Facturacion\FacturaTimbrada.xml";
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

        public Invoice (FacturaViewModel oFactura)
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
            decimal totalDescuento = 0,totalConceptos = 0;
            string numeroCertificado, aa, b, c;
            SelloDigital.leerCER(pathCer, out aa, out b, out c, out numeroCertificado);


            oComprobante.Version = "4.0";
            oComprobante.Serie = oFactura.Serie;
            oComprobante.Folio = oFactura.Folio.ToString();
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //oComprobante.Sello = ""; //FALTANTE
            oComprobante.FormaPago = oFactura.FormaPago;
            oComprobante.NoCertificado = numeroCertificado; 
            //oComprobante.Certificado = ""; //FALTANTE
            //oComprobante.SubTotal = 10m; SE VAN A CALCULAR
            //oComprobante.Descuento = 1m; SE VAN A CALCULAR
            oComprobante.Moneda = oFactura.Moneda;
            //oComprobante.Total = 9; SE VA A CALCULAR
            oComprobante.TipoDeComprobante = "I";
            oComprobante.MetodoPago = oFactura.TipoDePago;
            oComprobante.LugarExpedicion = "54720";

            ComprobanteEmisor oEmisor = new ComprobanteEmisor();
            oEmisor.Rfc = "EKU9003173C9";
            oEmisor.Nombre = "ESCUELA KEMPER URGATE";
            oEmisor.RegimenFiscal = "601";

            ComprobanteReceptor oReceptor = new ComprobanteReceptor();
            oReceptor.Rfc = oFactura.RFCCliente;
            oReceptor.Nombre = oFactura.RazonSocial;
            oReceptor.UsoCFDI = oFactura.UsoCFDI;
            oReceptor.RegimenFiscalReceptor = oFactura.UsoCFDI;
            oReceptor.DomicilioFiscalReceptor = oFactura.CP;
            //oReceptor.UsoCFDI = oFactura.LugarExpedicion;

            //ASIGNO EMISOR Y RECEPTOR
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;

            List<ComprobanteConcepto> lstConceptos = new List<ComprobanteConcepto>();
            foreach (Concepto oConceptoVM in oFactura.conceptos) {

                decimal importeTotal = oConceptoVM.cantidad * oConceptoVM.precioUnitario;
                ComprobanteConcepto oConcepto = new ComprobanteConcepto();
                
                oConcepto.ClaveProdServ = oConceptoVM.claveProducto;
                oConcepto.Cantidad = oConceptoVM.cantidad;
                oConcepto.ClaveUnidad = oConceptoVM.claveUnidad;
                oConcepto.Descripcion = oConceptoVM.descripcion;
                oConcepto.ValorUnitario = oConceptoVM.precioUnitario;
                oConcepto.Descuento = oConceptoVM.descuento == null? 0: (decimal) oConceptoVM.descuento;
                oConcepto.Importe = (oConceptoVM.cantidad*oConceptoVM.precioUnitario);
                oConcepto.ObjetoImp = "02";
                
                lstConceptos.Add(oConcepto);

                totalConceptos += importeTotal;
                if (oConceptoVM.descuento != null && oConceptoVM.descuento < 0)
                    totalDescuento += (decimal)oConceptoVM.descuento;
            }

            oComprobante.Conceptos = lstConceptos.ToArray();

            if(totalDescuento > 0)
                oComprobante.Descuento = totalDescuento;
                oComprobante.SubTotal = totalConceptos;
            oComprobante.Total = totalConceptos - totalDescuento;

            CreateXMLFile();

        }

        private void SellarXML()
        {
            string CadenaOriginal = "";
            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathCadenaOriginal);

            using (StringWriter sw = new StringWriter ())
            using (XmlWriter xwo = XmlWriter.Create(sw,transformador.OutputSettings))
            {
                transformador.Transform(pathXML, xwo);
                CadenaOriginal = sw.ToString ();
            }
            SelloDigital oSelloDigital = new SelloDigital();
            oComprobante.Certificado = oSelloDigital.Certificado(pathCer);
            oComprobante.Sello = oSelloDigital.Sellar(CadenaOriginal, pathKey, oFactura.ClavePrivada);

            CreateXMLFile();
        }

        private void TimbrarXML()
        {
            byte[] bXml = System.IO.File.ReadAllBytes(pathXML);
            Timbrado.Timbrado oTimbrar = new Timbrado.Timbrado();
            oTimbrar.Timbrar(bXml);
            System.IO.File.WriteAllBytes(pathXMLTimbrado, oTimbrar.XMLTimbrado);
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