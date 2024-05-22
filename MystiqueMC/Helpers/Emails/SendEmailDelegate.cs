// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Emails.SendEmailDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using log4net;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Reflection;


namespace MystiqueMC.Helpers.Emails
{
  public class SendEmailDelegate
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly string SMTP_SERVER = ConfigurationManager.AppSettings.Get(nameof (SMTP_SERVER));
    private readonly int SMTP_PORT = int.Parse(ConfigurationManager.AppSettings.Get(nameof (SMTP_PORT)));
    private readonly string SMTP_USER = ConfigurationManager.AppSettings.Get(nameof (SMTP_USER));
    private readonly string SMTP_PASS = ConfigurationManager.AppSettings.Get(nameof (SMTP_PASS));

    public bool SendEmail(
      string Para,
      string Asunto,
      string Contenido,
      string De = "",
      string[] Cc = null,
      string ResponderA = null)
    {
      try
      {
        string from = string.IsNullOrEmpty(De) ? this.SMTP_USER : De;
        SmtpClient smtpClient = new SmtpClient(this.SMTP_SERVER, this.SMTP_PORT)
        {
          Credentials = (ICredentialsByHost) new NetworkCredential(this.SMTP_USER, this.SMTP_PASS),
          EnableSsl = false
        };
        MailMessage message = new MailMessage(from, Para)
        {
          Subject = Asunto,
          Body = Contenido,
          IsBodyHtml = true
        };
        if (Cc != null)
        {
          foreach (string address in Cc)
            message.CC.Add(new MailAddress(address));
        }
        if (ResponderA != null)
          message.ReplyToList.Add(new MailAddress(ResponderA));
        smtpClient.Send(message);
        return true;
      }
      catch (SmtpException ex)
      {
        this._logger.Error((object) ex.StackTrace);
      }
      return false;
    }
  }
}
