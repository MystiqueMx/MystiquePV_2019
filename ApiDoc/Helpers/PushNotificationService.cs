using ApiDoc.Models;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace ApiDoc.Helpers
{
    public class PushNotificationService
    {
        public static void enviarNotificaciones(NotificacionPush notificacion)
        {
            string url = ConfigurationManager.AppSettings["OneSignalWebUrl"];
            string api_id = ConfigurationManager.AppSettings["OneSignalAppId"];
            string api_key = ConfigurationManager.AppSettings["OneSignalWebApiKey"];

            string iconoSmall = ConfigurationManager.AppSettings["iconoSmall"];
            string iconoLarge = ConfigurationManager.AppSettings["iconoLarge"];

            var request = WebRequest.Create(url) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", api_key);

            var serializer = new JavaScriptSerializer();

            var obj = new
            {
                app_id = api_id,
                headings = new { en = notificacion.headings },
                contents = new { en = notificacion.contents },
                include_player_ids = new string[] { notificacion.playerId },
                small_icon = iconoSmall,
                large_icon = iconoLarge
            };

            var param = serializer.Serialize(obj);

            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
        }

    }
}