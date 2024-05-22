// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.SMS.SendSmsDelegate
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using log4net;
using MystiqueMC.Helpers.SMS.Modelos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;


namespace MystiqueMC.Helpers.SMS
{
  public class SendSmsDelegate
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly string MQ_SERVER = ConfigurationManager.AppSettings.Get(nameof (MQ_SERVER));
    private readonly string MQ_VHOST = ConfigurationManager.AppSettings.Get(nameof (MQ_VHOST));
    private readonly string MQ_QUEUE = ConfigurationManager.AppSettings.Get(nameof (MQ_QUEUE));
    private readonly int MQ_PORT = int.Parse(ConfigurationManager.AppSettings.Get(nameof (MQ_PORT)));
    private readonly string MQ_USER = ConfigurationManager.AppSettings.Get(nameof (MQ_USER));
    private readonly string MQ_PASS = ConfigurationManager.AppSettings.Get(nameof (MQ_PASS));

    public Sms SendSms(Sms request)
    {
      try
      {
        request.Id = Guid.NewGuid().ToString();
        using (IConnection connection = new ConnectionFactory()
        {
          HostName = this.MQ_SERVER,
          VirtualHost = this.MQ_VHOST,
          Port = this.MQ_PORT,
          UserName = this.MQ_USER,
          Password = this.MQ_PASS
        }.CreateConnection())
        {
          using (IModel model = connection.CreateModel())
          {
            model.QueueDeclare(this.MQ_QUEUE, true, false, false, (IDictionary<string, object>) null);
            string s = JsonConvert.SerializeObject((object) request, new JsonSerializerSettings()
            {
              ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
            });
            model.BasicPublish("", this.MQ_QUEUE, (IBasicProperties) null, Encoding.UTF8.GetBytes(s));
          }
        }
        return request;
      }
      catch (Exception ex)
      {
        this._logger.Error((object) ex.StackTrace);
        return (Sms) null;
      }
    }
  }
}
