using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ApiDoc.Helpers
{
    public class SendEmail
    {
        private readonly string _smtpServer = ConfigurationManager.AppSettings.Get("SMTP_SERVER");
        private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings.Get("SMTP_PORT"));
        private readonly string _smtpUser = ConfigurationManager.AppSettings.Get("SMTP_USER");
        private readonly string _smtpPass = ConfigurationManager.AppSettings.Get("SMTP_PASS");
        private readonly string _fromAddress = ConfigurationManager.AppSettings.Get("EMAIL_ENVIO");
        private readonly string _usessl = ConfigurationManager.AppSettings.Get("USESSL");

        public Task SendEmailAsync(string email, string subject, string message, string fromAddress, string toAdressTitle, string fromAdressTitle)
        {
            #region SendEmailSSL
            var respuesta = false;
            try
            {               

                var mimeMessage = new MimeMessage();
                
                mimeMessage.From.Add(new MailboxAddress(fromAdressTitle, fromAddress));    

                mimeMessage.To.Add(new MailboxAddress(toAdressTitle, email));

                mimeMessage.Subject = subject; //Subject 
                var bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = message;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                var useSSL = Convert.ToBoolean(_usessl);

                if (useSSL)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new SmtpClient())
                {
                    if (useSSL)
                    {
                        client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.Auto);
                    }
                    else
                    {
                        client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.None);
                    }

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_smtpUser, _smtpPass);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                
            }

            return Task.FromResult(respuesta);
            #endregion           
        }
        public async Task SendEmailAsync(string to, string subject, string message, string toTitle, string fromTitle, string[] attachments)
        {
            #region SendEmailSSL

            var mimeMessage = new MimeMessage();
            var bodyBuilder = new BodyBuilder {HtmlBody = message};
            var useSsl = Convert.ToBoolean(_usessl);

            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment);
            }

            mimeMessage.From.Add(new MailboxAddress(fromTitle, _fromAddress));    

            mimeMessage.To.Add(new MailboxAddress(toTitle, to));

            mimeMessage.Subject = subject; //Subject 
            
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            if (useSsl)
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
            }

            using (var client = new SmtpClient())
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.ConnectAsync(_smtpServer, _smtpPort,
                    useSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }

            #endregion
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

                mimeMessage.Subject = Asunto;
                var bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = Contenido;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                bool useSSL = Convert.ToBoolean(_usessl);

                if (useSSL)
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };
                }

                using (var client = new SmtpClient())
                {
                    if (useSSL)
                    {
                        client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.Auto);
                    }
                    else
                    {
                        client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.None);
                    }

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_smtpUser, _smtpPass);
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