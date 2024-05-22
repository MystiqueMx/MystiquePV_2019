using MystiqueMC.DAL;

namespace MystiqueMC.Controllers
{
    internal class OpcionAgrupador
    {
        public int Id { get; set; }
        public Insumos Insumo { get; set; }
        public Productos Producto { get; set; }
    }
}