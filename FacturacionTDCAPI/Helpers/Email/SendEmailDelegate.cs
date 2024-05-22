
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NLog;
using WebGrease.Css.Extensions;
namespace FacturacionTDCAPI.Helpers.Email
{
    public class SendEmailDelegate
    {
        private static readonly Logger _logger
          = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// El servidor SMTP que se utilizara para el envio de correos
        /// </summary>
        private readonly string _smtpServer = ConfigurationManager.AppSettings.Get("SMTP_SERVER");
        /// <summary>
        /// Puerto de acceso al servidor para envio de correos
        /// </summary>
        private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings.Get("SMTP_PORT"));
        /// <summary>
        /// Credenciales de acceso a servidor SMTP
        /// </summary>
        private readonly string _smtpUser = ConfigurationManager.AppSettings.Get("SMTP_USER");
        /// <summary>
        /// Credenciales de acceso a servidor SMTP
        /// </summary>
        private readonly string _smtpPass = ConfigurationManager.AppSettings.Get("SMTP_PASS");
        /// <summary>
        ///  Delegate para el envio de correos electrónicos
        /// </summary>
        /// <param name="para">Correo electronico de destino</param>
        /// <param name="asunto">Asunto del correo</param>
        /// <param name="contenido">Cuerpo del correo</param>
        /// <param name="de">Correo electronico de remitente</param>
        /// <param name="cc">Arreglos de correos electronicos que se anexan al campo CC</param>
        /// <param name="responderA">Correo electronico que se anexa a ReplyToList</param>
        /// <returns>True si no ocurre una excepcion en el envío</returns>
        public async Task<bool> SendEmail(string para, string asunto, string contenido, string de = "", string[] cc = null, string responderA = null)
        {
            try
            {
                var fromAdressTitle = string.IsNullOrEmpty(de) ? _smtpUser : de;

                //To Address    
                var toAddress = para;
                var toAdressTitle = para;
                var subject = asunto;
                var bodyContent = contenido;


                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(fromAdressTitle, _smtpUser));

                //To many emails
                var toAdresses = toAddress.Replace(" ", "").Split(';');
                foreach (var address in toAdresses)
                {
                    mimeMessage.To.Add(new MailboxAddress(toAdressTitle, address));
                }

                mimeMessage.Subject = subject; //Subject 
                var bodyBuilder = new BodyBuilder { HtmlBody = bodyContent };

                mimeMessage.Body = bodyBuilder.ToMessageBody();

                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_smtpUser, _smtpPass);
                    await client.SendAsync(mimeMessage);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (System.Net.Mail.SmtpException smtpException)
            {
                // The connection to the SMTP server failed.
                //-or-
                //Authentication failed.
                //-or-
                //The operation timed out.
                //-or-
                //EnableSsl is set to true but the DeliveryMethod property is set to SpecifiedPickupDirectory or PickupDirectoryFromIis.
                //-or-
                //EnableSsl is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.
                _logger.Error(smtpException);
            }
            return false;
        }

        /// <summary>
        ///  Delegate para el envio de correos electrónicos
        /// </summary>
        /// <param name="para">Correo electronico de destino</param>
        /// <param name="asunto">Asunto del correo</param>
        /// <param name="contenido">Cuerpo del correo</param>
        /// <param name="de">Correo electronico de remitente</param>
        /// <param name="cc">Arreglos de correos electronicos que se anexan al campo CC</param>
        /// <param name="responderA">Correo electronico que se anexa a ReplyToList</param>
        /// <param name="files">Archivos adjuntos</param>
        /// <returns>True si no ocurre una excepcion en el envío</returns>
        public async Task<bool> SendEmail(string para, string asunto, string contenido, string de = "", string[] cc = null, string responderA = null, string[] files = null)
        {
            try
            {
                var fromAdressTitle = string.IsNullOrEmpty(de) ? _smtpUser : de;

                //To Address    
                var toAddress = para;
                var toAdressTitle = para;
                var subject = asunto;
                var bodyContent = contenido;


                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(fromAdressTitle, _smtpUser));

                //To many emails
                var toAdresses = toAddress.Replace(" ", "").Split(';');
                foreach (var address in toAdresses)
                {
                    mimeMessage.To.Add(new MailboxAddress(toAdressTitle, address));
                }

                mimeMessage.Subject = subject; //Subject 
                var bodyBuilder = new BodyBuilder { HtmlBody = bodyContent };

                if (files != null && files.Any())
                {
                    files.ForEach(c => bodyBuilder.Attachments.Add(c));
                }

                mimeMessage.Body = bodyBuilder.ToMessageBody();


                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_smtpUser, _smtpPass);
                    await client.SendAsync(mimeMessage);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (System.Net.Mail.SmtpException smtpException)
            {
                // The connection to the SMTP server failed.
                //-or-
                //Authentication failed.
                //-or-
                //The operation timed out.
                //-or-
                //EnableSsl is set to true but the DeliveryMethod property is set to SpecifiedPickupDirectory or PickupDirectoryFromIis.
                //-or-
                //EnableSsl is set to true, but the SMTP mail server did not advertise STARTTLS in the response to the EHLO command.
                _logger.Error(smtpException);
            }
            return false;
        }
    }
}