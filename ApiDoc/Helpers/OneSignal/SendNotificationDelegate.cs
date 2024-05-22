using log4net;
using MystiqueMC.Helpers.OneSignal.Modelos;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace MystiqueMC.Helpers.Emails
{
    public class SendNotificationDelegate
    {
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region OneSignalNotification
        //string OBLIGATORIO
        readonly string ONESIGNAL_ENDPOINT = ConfigurationManager.AppSettings.Get("ONESIGNAL_ENDPOINT");
        //string OBLIGATORIO
        readonly string ONESIGNAL_AUTHHEADER = ConfigurationManager.AppSettings.Get("ONESIGNAL_AUTHHEADER");
        readonly string ONESIGNAL_AUTHHEADER_CLIENTE = ConfigurationManager.AppSettings.Get("ONESIGNAL_AUTHHEADER_CLIENTE");
        //string OBLIGATORIO
        readonly string ONESIGNAL_MCAPPID = ConfigurationManager.AppSettings.Get("ONESIGNAL_MCAPPID");

        readonly string ONESIGNAL_APPID_DOCTOR = ConfigurationManager.AppSettings.Get("ONESIGNAL_MCAPPID_DOCTOR");
        readonly string ONESIGNAL_APPID_CLIENTE = ConfigurationManager.AppSettings.Get("ONESIGNAL_MCAPPID_CLIENTE");
        //string OBLIGATORIO
        readonly string ONESIGNAL_MCTEMPLATEID = ConfigurationManager.AppSettings.Get("ONESIGNAL_MCTEMPLATEID");
        //string[] OBLIGATORIO
        readonly string ONESIGNAL_MCAPPSEGMENTS = ConfigurationManager.AppSettings.Get("ONESIGNAL_MCAPPSEGMENTS");
        //string OPCIONAL
        readonly string ONESIGNAL_CHANNELID = ConfigurationManager.AppSettings.Get("ONESIGNAL_CHANNELID");
        readonly string ONESIGNAL_CHANNELID_CLIENTE = ConfigurationManager.AppSettings.Get("ONESIGNAL_CHANNELID_CLIENTE");
        //string OPCIONAL
        readonly string ONESIGNAL_BADGETYPE = ConfigurationManager.AppSettings.Get("ONESIGNAL_BADGETYPE");
        //int OPCIONAL
        readonly string ONESIGNAL_BADGECOUNT = ConfigurationManager.AppSettings.Get("ONESIGNAL_BADGECOUNT");
        //int OPCIONAL
        readonly string ONESIGNAL_TTL = ConfigurationManager.AppSettings.Get("ONESIGNAL_TTL");
        #endregion
        public bool SendNotification<T>(T notificacion) where T : NotificacionBase
        {
            try
            {
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var param = serializer.Serialize(notificacion);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendNotification(string Titulo, string Contenido, string[] Segmentos = null)
        {
            try
            {
                var segmentos = Segmentos ?? new string[] { ONESIGNAL_MCAPPSEGMENTS };
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionPorSegmento
                {
                    included_segments = segmentos,
                    headings = new { en = Titulo },
                    contents = new { en = Contenido },
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendNotificationPorPlayerIds(string[] PlayerIds, string Titulo, string Contenido)
        {
            try
            {
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionPorId
                {
                    include_player_ids = PlayerIds,
                    headings = new { en = Titulo },
                    contents = new { en = Contenido },
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendTemplatedNotification(string Template, string[] Segmentos = null)
        {
            try
            {
                var segmentos = Segmentos ?? new string[] { ONESIGNAL_MCAPPSEGMENTS };

                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionTemplatePorSegmento
                {
                    included_segments = segmentos,
                    template_id = Template,
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendTemplatedNotificationPorPlayerIds(string[] PlayerIds, string Template)
        {
            try
            {
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionTemplatePorIds
                {
                    include_player_ids = PlayerIds,
                    template_id = Template,
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendDefaultNotification(string Template = null, string[] Segmentos = null)
        {
            try
            {
                var segmentos = Segmentos ?? new string[] { ONESIGNAL_MCAPPSEGMENTS };
                var template = Template ?? ONESIGNAL_MCTEMPLATEID;

                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionTemplatePorSegmento
                {
                    included_segments = segmentos,
                    template_id = template,
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
        public bool SendDefaultNotificationPorPlayerIds(string[] PlayerIds, string Template = null)
        {
            try
            {

                var template = Template ?? ONESIGNAL_MCTEMPLATEID;

                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionTemplatePorIds
                {
                    include_player_ids = PlayerIds,
                    template_id = template,
                    app_id = ONESIGNAL_MCAPPID,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #region MEDIPLACE

        //ENVIAR NOTIFICACION A DOCTOR
        public bool EnviarNuevaSolicitudDoctor(string[] PlayerIds, string Titulo, string Contenido)
        {
            try
            {
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionPorId
                {
                    include_player_ids = PlayerIds,
                    headings = new { en = Titulo },
                    contents = new { en = Contenido },
                    app_id = ONESIGNAL_APPID_DOCTOR,
                    android_channel_id = ONESIGNAL_CHANNELID,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }


        //ENVIAR NOTIFICACION A CLIENTE
        public bool EnviarRespuestaSolicitudCliente(string[] PlayerIds, string Titulo, string Contenido)
        {
            try
            {
                var request = WebRequest.Create(ONESIGNAL_ENDPOINT) as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("Authorization", ONESIGNAL_AUTHHEADER_CLIENTE);
                var serializer = new JavaScriptSerializer();
                var obj = new NotificacionPorId
                {
                    include_player_ids = PlayerIds,
                    headings = new { en = Titulo },
                    contents = new { en = Contenido },
                    app_id = ONESIGNAL_APPID_CLIENTE,
                    android_channel_id = ONESIGNAL_CHANNELID_CLIENTE,
                    ios_badgeType = ONESIGNAL_BADGETYPE,
                    ios_badgeCount = int.Parse(ONESIGNAL_BADGECOUNT),
                    ttl = int.Parse(ONESIGNAL_TTL)
                };
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;


                using (var writer = request.GetRequestStream())
                    writer.Write(byteArray, 0, byteArray.Length);

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseContent = reader.ReadToEnd();

                _logger.Debug("Notificacion enviada: " + responseContent);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }



        #endregion
    }

}