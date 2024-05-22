namespace MystiqueMC.Controllers
{
    public class Agrupador
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal? CostoExtra { get; set; }
        public bool DebeConfirmarPorSeparado { get; set; }
        public bool PuedeAgregarExtra { get; set; }
        public int Indice { get; set; }
        public object Opciones { get; set; }
    }
}