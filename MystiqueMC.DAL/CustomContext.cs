using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueMC.DAL
{
    public partial class MystiqueMeEntities : DbContext
    {
        public override int SaveChanges()
        {            
            SavingChanges();
            return base.SaveChanges();
        }

        private void SavingChanges()
        {
            var notUpperCaseProperties = new System.Collections.Generic.List<string>();
            notUpperCaseProperties.Add("urlImagen");
            notUpperCaseProperties.Add("imagenTitulo");
            notUpperCaseProperties.Add("imagenDescuento");
            notUpperCaseProperties.Add("imagenExt");
            notUpperCaseProperties.Add("ruta");
            notUpperCaseProperties.Add("imagen");
            notUpperCaseProperties.Add("archivoPdfRuta");
            notUpperCaseProperties.Add("archivoXmlRuta");
            notUpperCaseProperties.Add("logoTickets");
            notUpperCaseProperties.Add("logoUrl");
            notUpperCaseProperties.Add("logoTickets");
            notUpperCaseProperties.Add("urlImagenCodigo");
            notUpperCaseProperties.Add("urlFotoPerfil");
            notUpperCaseProperties.Add("urlTerminosCondiciones");
            notUpperCaseProperties.Add("urlDocumento");
            notUpperCaseProperties.Add("password");
            notUpperCaseProperties.Add("UserName");
            notUpperCaseProperties.Add("customerId");
            notUpperCaseProperties.Add("tokenId");
            notUpperCaseProperties.Add("tarjetaId");
            notUpperCaseProperties.Add("nombreHabiente");
            notUpperCaseProperties.Add("tipo");
            notUpperCaseProperties.Add("nombreColonia");
            notUpperCaseProperties.Add("nombreQuienRealizo");
            notUpperCaseProperties.Add("direccionEntrega");
            notUpperCaseProperties.Add("quienRecibe");
            notUpperCaseProperties.Add("ordenOP");
            notUpperCaseProperties.Add("idOrdenPedido");
            notUpperCaseProperties.Add("descripcion");
            notUpperCaseProperties.Add("nota");
            

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    Type entityType = entry.Entity.GetType();
                    var entityProperties = entityType.GetProperties();
                    foreach (var entityProperty in entityProperties)
                    {
                        if (Type.GetTypeCode(entityProperty.PropertyType) == TypeCode.String && !notUpperCaseProperties.Any(a => a.Equals(entityProperty.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            entityProperty.SetValue(entry.Entity, Convert.ToString(entityProperty.GetValue(entry.Entity, null)).ToUpper(), null);
                        }
                    }

                    //entry.Entity. = entry.Entity.Name.ToUpper();
                }
            }
        }

    }
}
