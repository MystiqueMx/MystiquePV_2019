using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MystiqueMcApi.Models;
using MystiqueMC.DAL;

namespace MystiqueMcApi.Helpers.Facturacion
{
    public static class EnvioPostFacturacionProvider
    {
        //[AutomaticRetry(Attempts = 3)]
        //public static async Task EnviarFactura(Guid id, string directorioAplicacion, bool reenviando)
        //{
        //    var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //    logger.Debug(reenviando
        //        ? $"Reenviando facturaCliente por correo : {id}"
        //        : $"Enviando facturaCliente por correo : {id}");

        //    using (var contexto = new MystiqueMeEntities())
        //    {
        //        var dbFacturaCliente = contexto.facturaCliente
        //            .Include(c=>c.ticketSucursal)
        //            .Include(c=>c.receptorCliente)
        //            .FirstOrDefault(c=> c.idFacturaClienteId == id);
        //        if(dbFacturaCliente == null) throw new ArgumentException();

        //        const string fromTitle = "Facturacion City Salads App";
        //        var email = dbFacturaCliente.receptorCliente.correo;
        //        var subject = $"Factura consumo {dbFacturaCliente.ticketSucursal.folio}";
        //        var body =
        //            $"Factura del consumo {dbFacturaCliente.ticketSucursal.folio} del día {dbFacturaCliente.ticketSucursal.fechaCompra:dd/MM/yyyy} adjunta a este correo. ( 2 documentos )";
        //        var toTitle =
        //            $"{dbFacturaCliente.receptorCliente.clientes.nombre} {dbFacturaCliente.receptorCliente.clientes.paterno}";
        //        var pdfFile = $"{directorioAplicacion}{dbFacturaCliente.archivoPdfRuta}";
        //        var xmlFile = $"{directorioAplicacion}{dbFacturaCliente.archivoXmlRuta}";
        //        var sendEmailDelegate = new SendEmail();
        //        await sendEmailDelegate.SendEmailAsync(email, subject, body, toTitle, fromTitle, new[] { pdfFile, xmlFile });
        //    }
            
        //}
    }
}