using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ApiDoc.Helpers.Pdf.ViewModels;
using Wired.RazorPdf;

namespace ApiDoc.Helpers.Pdf
{
    public class GeneracionPdfDelegate
    {
        private readonly string _route;
        private readonly string _outputAbsolutePath;
        private readonly string _imagesAbsolutePath;
        private readonly string _templateAbsoluteFile;
        /// <summary>
        /// Generador de pdf
        /// </summary>
        /// <param name="serverPath">Directorio absoluto de la aplicacion</param>
        /// <param name="route">Directorio relativo de salida</param>
        /// <param name="templateAbsoluteFile">Ubicacion absoluta del formato pdf en razor</param>
        /// <param name="imagesAbsolutePath">Ubicacion base de las imagenes, por default Path.GetTempPath() </param>
        public GeneracionPdfDelegate(string serverPath, string route, string templateAbsoluteFile, string imagesAbsolutePath = "")
        {
            _outputAbsolutePath = $"{serverPath}{route}";
            
            if (!Directory.Exists(_outputAbsolutePath))
            {
                Directory.CreateDirectory(_outputAbsolutePath);
            }

            if (!File.Exists(templateAbsoluteFile))
            {
                throw new ArgumentException("file not found", nameof(templateAbsoluteFile));
            }
            
            this._route = route;
            this._templateAbsoluteFile = templateAbsoluteFile;
            this._imagesAbsolutePath = string.IsNullOrEmpty(imagesAbsolutePath) ? Path.GetTempPath() : imagesAbsolutePath;
        }

        public string GenerarPdfFactura(PdfFactura factura)
        {
            var outputName = $"{factura.UUID}.pdf";
            var outputFile = $"{_outputAbsolutePath}\\{outputName}";

            var generator = new StandaloneGenerator(new Wired.Razor.Parser(), _imagesAbsolutePath);

            var pdf = generator.GeneratePdf(ConfigurePageSettings, factura, _templateAbsoluteFile);
            using (var mc = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                mc.Write(pdf, 0, pdf.Length);
            }

            return $"{_route}\\{outputName}";
        }

        #region Helpers
        private static void ConfigurePageSettings(PdfWriter writer, Document document)
        {
            document.SetPageSize(PageSize.LETTER);
        }
        #endregion
    }
}