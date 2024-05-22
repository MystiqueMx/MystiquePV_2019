// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Emails.SendNotificationDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using log4net;
using MystiqueMC.Helpers.OneSignal.Modelos;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;


namespace MystiqueMC.Helpers.Emails
{
  public class SendNotificationDelegate
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly string ONESIGNAL_ENDPOINT = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_ENDPOINT));
    private readonly string ONESIGNAL_AUTHHEADER = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_AUTHHEADER));
    private readonly string ONESIGNAL_MCAPPID = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_MCAPPID));
    private readonly string ONESIGNAL_MCTEMPLATEID = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_MCTEMPLATEID));
    private readonly string ONESIGNAL_MCAPPSEGMENTS = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_MCAPPSEGMENTS));
    private readonly string ONESIGNAL_CHANNELID = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_CHANNELID));
    private readonly string ONESIGNAL_BADGETYPE = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_BADGETYPE));
    private readonly string ONESIGNAL_BADGECOUNT = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_BADGECOUNT));
    private readonly string ONESIGNAL_TTL = ConfigurationManager.AppSettings.Get(nameof (ONESIGNAL_TTL));

    public bool SendNotification<T>(T notificacion) where T : NotificacionBase
    {
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        byte[] bytes = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize((object) notificacion));
        string str = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendNotification(string Titulo, string Contenido, string[] Segmentos = null)
    {
      try
      {
        string[] strArray1 = Segmentos;
        if (strArray1 == null)
          strArray1 = new string[1]
          {
            this.ONESIGNAL_MCAPPSEGMENTS
          };
        string[] strArray2 = strArray1;
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionPorSegmento notificacionPorSegmento1 = new NotificacionPorSegmento();
        notificacionPorSegmento1.included_segments = strArray2;
        notificacionPorSegmento1.headings = (object) new
        {
          en = Titulo
        };
        notificacionPorSegmento1.contents = (object) new
        {
          en = Contenido
        };
        notificacionPorSegmento1.app_id = this.ONESIGNAL_MCAPPID;
        notificacionPorSegmento1.android_channel_id = this.ONESIGNAL_CHANNELID;
        notificacionPorSegmento1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        notificacionPorSegmento1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        notificacionPorSegmento1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionPorSegmento notificacionPorSegmento2 = notificacionPorSegmento1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) notificacionPorSegmento2));
        string str = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendNotificationPorPlayerIds(string[] PlayerIds, string Titulo, string Contenido)
    {
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionPorId notificacionPorId1 = new NotificacionPorId();
        notificacionPorId1.include_player_ids = PlayerIds;
        notificacionPorId1.headings = (object) new
        {
          en = Titulo
        };
        notificacionPorId1.contents = (object) new
        {
          en = Contenido
        };
        notificacionPorId1.app_id = this.ONESIGNAL_MCAPPID;
        notificacionPorId1.android_channel_id = this.ONESIGNAL_CHANNELID;
        notificacionPorId1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        notificacionPorId1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        notificacionPorId1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionPorId notificacionPorId2 = notificacionPorId1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) notificacionPorId2));
        string str = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendTemplatedNotification(string Template, string[] Segmentos = null)
    {
      try
      {
        string[] strArray1 = Segmentos;
        if (strArray1 == null)
          strArray1 = new string[1]
          {
            this.ONESIGNAL_MCAPPSEGMENTS
          };
        string[] strArray2 = strArray1;
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionTemplatePorSegmento templatePorSegmento1 = new NotificacionTemplatePorSegmento();
        templatePorSegmento1.included_segments = strArray2;
        templatePorSegmento1.template_id = Template;
        templatePorSegmento1.app_id = this.ONESIGNAL_MCAPPID;
        templatePorSegmento1.android_channel_id = this.ONESIGNAL_CHANNELID;
        templatePorSegmento1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        templatePorSegmento1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        templatePorSegmento1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionTemplatePorSegmento templatePorSegmento2 = templatePorSegmento1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) templatePorSegmento2));
        string str = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendTemplatedNotificationPorPlayerIds(string[] PlayerIds, string Template)
    {
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionTemplatePorIds notificacionTemplatePorIds1 = new NotificacionTemplatePorIds();
        notificacionTemplatePorIds1.include_player_ids = PlayerIds;
        notificacionTemplatePorIds1.template_id = Template;
        notificacionTemplatePorIds1.app_id = this.ONESIGNAL_MCAPPID;
        notificacionTemplatePorIds1.android_channel_id = this.ONESIGNAL_CHANNELID;
        notificacionTemplatePorIds1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        notificacionTemplatePorIds1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        notificacionTemplatePorIds1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionTemplatePorIds notificacionTemplatePorIds2 = notificacionTemplatePorIds1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) notificacionTemplatePorIds2));
        string str = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendDefaultNotification(string Template = null, string[] Segmentos = null)
    {
      try
      {
        string[] strArray1 = Segmentos;
        if (strArray1 == null)
          strArray1 = new string[1]
          {
            this.ONESIGNAL_MCAPPSEGMENTS
          };
        string[] strArray2 = strArray1;
        string str1 = Template ?? this.ONESIGNAL_MCTEMPLATEID;
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionTemplatePorSegmento templatePorSegmento1 = new NotificacionTemplatePorSegmento();
        templatePorSegmento1.included_segments = strArray2;
        templatePorSegmento1.template_id = str1;
        templatePorSegmento1.app_id = this.ONESIGNAL_MCAPPID;
        templatePorSegmento1.android_channel_id = this.ONESIGNAL_CHANNELID;
        templatePorSegmento1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        templatePorSegmento1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        templatePorSegmento1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionTemplatePorSegmento templatePorSegmento2 = templatePorSegmento1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) templatePorSegmento2));
        string str2 = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str2 = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str2));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }

    public bool SendDefaultNotificationPorPlayerIds(string[] PlayerIds, string Template = null)
    {
      try
      {
        string str1 = Template ?? this.ONESIGNAL_MCTEMPLATEID;
        HttpWebRequest httpWebRequest = WebRequest.Create(this.ONESIGNAL_ENDPOINT) as HttpWebRequest;
        httpWebRequest.KeepAlive = true;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Headers.Add("Authorization", this.ONESIGNAL_AUTHHEADER);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        NotificacionTemplatePorIds notificacionTemplatePorIds1 = new NotificacionTemplatePorIds();
        notificacionTemplatePorIds1.include_player_ids = PlayerIds;
        notificacionTemplatePorIds1.template_id = str1;
        notificacionTemplatePorIds1.app_id = this.ONESIGNAL_MCAPPID;
        notificacionTemplatePorIds1.android_channel_id = this.ONESIGNAL_CHANNELID;
        notificacionTemplatePorIds1.ios_badgeType = this.ONESIGNAL_BADGETYPE;
        notificacionTemplatePorIds1.ios_badgeCount = int.Parse(this.ONESIGNAL_BADGECOUNT);
        notificacionTemplatePorIds1.ttl = int.Parse(this.ONESIGNAL_TTL);
        NotificacionTemplatePorIds notificacionTemplatePorIds2 = notificacionTemplatePorIds1;
        byte[] bytes = Encoding.UTF8.GetBytes(scriptSerializer.Serialize((object) notificacionTemplatePorIds2));
        string str2 = (string) null;
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            str2 = streamReader.ReadToEnd();
        }
        this._logger.Debug((object) ("Notificacion enviada: " + str2));
        return true;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex);
        return false;
      }
    }
  }
}
