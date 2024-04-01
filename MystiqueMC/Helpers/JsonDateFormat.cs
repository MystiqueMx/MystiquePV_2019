using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MystiqueMC.Helpers
{
    public static class NewtonsoftJsonExtensions { public static ActionResult ToJsonResult(this object obj) { var content = new ContentResult(); content.Content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }); content.ContentType = "application/json"; return content; } }
}