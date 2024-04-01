using FacturacionTDCAPI.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Models.Responses
{
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
}