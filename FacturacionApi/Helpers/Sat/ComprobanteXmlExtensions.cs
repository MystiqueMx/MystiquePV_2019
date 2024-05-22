using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using FacturacionApi.Data.Sat.Catalogos;
using FacturacionApi.Helpers.Criptografia;

namespace FacturacionApi.Helpers.Sat
{
    public static class ComprobanteXmlExtensions
    {
        private const string SchemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd";
        
        public static string ToXml(this Comprobante comprobante)
        {
            string result;
            #if DEBUG
            var watch = System.Diagnostics.Stopwatch.StartNew();
            #endif
            using (var sw = new StringWriter())
            {
                var xmlNameSpace = new XmlSerializerNamespaces();
                xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
                xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlNameSpace.Add("schemaLocation", SchemaLocation);
                //var enc = Encoding.GetEncoding("ISO-8859-1");
                var xserializer = new XmlSerializer(typeof(Comprobante));
                using (var xtw = new XmlTextWriter(sw))
                {
                    xserializer.Serialize(xtw, comprobante, xmlNameSpace);
                    result = sw.ToString().Replace("encoding=\"utf-16\"","");
                }
            }
            #if DEBUG
            watch.Stop();
            System.Diagnostics.Trace.WriteLine($"Comprobante XML Serialization: {watch.ElapsedMilliseconds} ms");
            #endif
            return result;
        }

        public static string ToXmlCadenaOriginal(this Comprobante comprobante, string rutaXslt)
        {
            string cadenaOriginal;
            try
            {
                var xml = comprobante.ToXml();
                using (var xmlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    using (var xsltCadenaOriginal = XmlReader.Create(rutaXslt))
                    {
                        using (var mResult = new MemoryStream())
                        {
                            var myXPaArchXmLthDocument = new XPathDocument(xmlMemoryStream).CreateNavigator();
                            var myXslTransform = new XslCompiledTransform(true);
                            
                            myXslTransform.Load(xsltCadenaOriginal);
                            myXslTransform.Transform(myXPaArchXmLthDocument, null, mResult);
                            
                            cadenaOriginal = Utf8ByteArrayToString(mResult.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cadenaOriginal;
        }

        private static string Utf8ByteArrayToString(byte[] characters)
        {
            var encoding = new UTF8Encoding();
            var constructedString = encoding.GetString(characters);
            return (constructedString);
        }
    }

}