namespace ApiDoc.Models.Entradas
{
    public class DetalleSucursalRequest : RequestBase
    {
        public int Sucursal { get; set; }
        public AppLanguage Idioma { get; set; } = AppLanguage.Spanish;
    }
}