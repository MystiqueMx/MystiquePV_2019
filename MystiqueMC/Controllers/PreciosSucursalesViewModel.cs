using System;

namespace MystiqueMC.Controllers
{
    internal class PreciosSucursalesViewModel
    {
        public int idSucursalProducto { get; set; }
        public string NombreSucursal { get; set; }
        public decimal Precio { get; set; }
        public bool activo { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}