using System.Collections.Generic;

namespace ApiDoc.Models.Salidas
{
    public class EspecialidadesResponse : ResponseBase
    {
        public IEnumerable<CustomSelectItem> Data { get; set; }
    }
}