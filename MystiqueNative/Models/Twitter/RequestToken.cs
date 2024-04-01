using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Twitter
{
    public class RequestToken
    {
        public string OauthToken { get; set; }
        public string OauthTokenSecret { get; set; }
        public bool OauthCallbackConfirmed { get; set; }
        public string UserId { get; set; }
        public string ScreenName { get; set; }
    }
}
