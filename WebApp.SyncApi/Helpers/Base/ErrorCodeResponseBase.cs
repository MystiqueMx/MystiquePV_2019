using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.SyncApi.Helpers.Base
{
    public class ErrorCodeResponseBase
    {
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }


    public class ResponseBase
    {
        #region CTOR
        public ResponseBase()
        {
        }
        public ResponseBase(ErrorCodeResponseBase estatus)
        {
            this.EstatusPeticion = estatus;
        }
        #endregion

        [JsonProperty("estatusPeticion")]
        public ErrorCodeResponseBase EstatusPeticion { get; set; }
    }
    public class ListResponseBase : ResponseBase
    {
        #region CTOR
        public ListResponseBase() : base()
        {
            this.Results = Enumerable.Empty<object>();
        }
        public ListResponseBase(ErrorCodeResponseBase estatus) : base(estatus)
        {
            this.Results = Enumerable.Empty<object>();
        }
        public ListResponseBase(ErrorCodeResponseBase estatus, IEnumerable<object> results) : base(estatus)
        {
            this.Results = results;
        }
        #endregion

        [JsonProperty("results")]
        public IEnumerable<object> Results { get; set; }
    }
}