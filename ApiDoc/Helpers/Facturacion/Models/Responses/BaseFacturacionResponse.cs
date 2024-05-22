namespace ApiDoc.Helpers.Facturacion.Models.Responses
{
    public class BaseFacturacionResponse
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }

        public bool IsSuccessful => (ResponseTypes) ResponseCode == ResponseTypes.CodigoOk;
    }
}
