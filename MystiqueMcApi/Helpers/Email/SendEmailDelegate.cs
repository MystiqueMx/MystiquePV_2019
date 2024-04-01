using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MimeKit;
using MailKit.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using MailKit.Net.Smtp;

namespace MystiqueMcApi.Helpers.Email
{
    public class SendEmailDelegate
    {
        //private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// El servidor SMTP que se utilizara para el envio de correos
        /// </summary>
        private readonly string SMTP_SERVER = ConfigurationManager.AppSettings.Get("SMTP_SERVER");
        /// <summary>
        /// Puerto de acceso al servidor para envio de correos
        /// </summary>
        private readonly int SMTP_PORT = int.Parse(ConfigurationManager.AppSettings.Get("SMTP_PORT"));
        /// <summary>
        /// Credenciales de acceso a servidor SMTP
        /// </summary>
        private readonly string SMTP_USER = ConfigurationManager.AppSettings.Get("SMTP_USER");
        /// <summary>
        /// Credenciales de acceso a servidor SMTP
        /// </summary>
        private readonly string SMTP_PASS = ConfigurationManager.AppSettings.Get("SMTP_PASS");



        private readonly string USESSL = ConfigurationManager.AppSettings.Get("USESSL");
        /// <summary>
        ///  Delegate para el envio de correos electrónicos
        /// </summary>
        /// <param name="Para">Correo electronico de destino</param>
        /// <param name="Asunto">Asunto del correo</param>
        /// <param name="Contenido">Cuerpo del correo</param>
        /// <param name="De">Correo electronico de remitente</param>
        /// <param name="Cc">Arreglos de correos electronicos que se anexan al campo CC</param>
        /// <param name="ResponderA">Correo electronico que se anexa a ReplyToList</param>
        /// <returns>True si no ocurre una excepcion en el envío</returns>
        [Obsolete]
        public bool SendEmail(string Para, string Asunto, string Contenido, string De = "", string[] Cc = null, string ResponderA = null)
        {
            try
            {
                var Remitente = string.IsNullOrEmpty(De) ? SMTP_USER : De;

                var client = new System.Net.Mail.SmtpClient(SMTP_SERVER, SMTP_PORT)
                {
                    Credentials = new System.Net.NetworkCredential(SMTP_USER, SMTP_PASS),
                    EnableSsl = false
                };
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(Remitente, Para)
                {
                    Subject = Asunto,
                    Body = Contenido,
                    IsBodyHtml = true,
                };

                if (Cc != null)
                    foreach (var copiaPara in Cc)
                        mailMessage.CC.Add(new System.Net.Mail.MailAddress(copiaPara));

                if (ResponderA != null)
                    mailMessage.ReplyToList.Add(new System.Net.Mail.MailAddress(ResponderA));

                client.Send(mailMessage);
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
                //_logger.Error(smtpException.StackTrace);
                Console.WriteLine(smtpException.StackTrace);
            }
            return false;
        }



        public bool SendEmailSSL(string Para, string CorreoPara, string Asunto, string Contenido, string CorreoDe = "", string NombreCorreoDe = "")
        {
            #region SendEmailSSL
            bool respuesta = false;
            try
            {

                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(NombreCorreoDe, CorreoDe));

                mimeMessage.To.Add(new MailboxAddress(Para, CorreoPara));

                mimeMessage.Subject = Asunto; //Subject 
                var bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = Contenido;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                bool useSSL = Convert.ToBoolean(USESSL);

                if (useSSL)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };
                }

                using (var client = new SmtpClient())
                {
                    if (useSSL)
                    {
                        client.Connect(SMTP_SERVER, SMTP_PORT, SecureSocketOptions.Auto);
                    }
                    else
                    {
                        client.Connect(SMTP_SERVER, SMTP_PORT, SecureSocketOptions.None);
                    }

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(SMTP_USER, SMTP_PASS);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return respuesta;
            #endregion           
        }
    }
}