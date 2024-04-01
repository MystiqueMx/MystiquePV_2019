using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;

namespace MystiqueNative.Models
{
    public class Profile : BaseModel
    {
        [JsonProperty("clienteId")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        [JsonProperty("paterno")]
        public string Paterno { get; set; }
        [JsonProperty("materno")]
        public string Materno { get; set; }
        [JsonProperty("telefono")]
        public string Telefono { get; set; }
        [JsonProperty("colonia")]
        public string Colonia { get; set; }
        [JsonProperty("fechaNacimiento")]
        public string FechaNacimiento { get; set; }
        [JsonProperty("sexo")]
        public string Sexo { get; set; }
        public string IdConekta { get; set; }
        [JsonProperty("membresiaId")]
        public string IdMembresia { get; set; }
        [JsonProperty("fechaFinMembresia")]
        public string ExpiracionMembresiaAsString { get; set; }
        [JsonProperty("folioMembresia")]
        public string FolioMembresia { get; set; }
        [JsonProperty("folioGuidMembresia")]
        public string GuidMembresia { get; set; }
        [JsonProperty("urlFotoPerfil")]
        public string Foto { get; set; }
        [JsonProperty("contrasenia")]
        public string Password { get; set; }
        [JsonProperty("facebookId")]
        public string FacebookId { get; set; }
        [JsonProperty("catColoniaId")]
        public string ColoniaId { get; set; }
        [JsonProperty("catCiudadId")]
        public string CiudadId { get; set; }
        [JsonProperty("puntosActuales")]
        public string PuntosActuales { get; set; }
        [JsonProperty("registroCompleto")]
        public bool RegistroCompleto { get; set; }

        /// <summary>
        /// //////////////////////////////////////
        /// </summary>
        public string NombreCompleto
        {
            get
            {
                return string.Format("{0} {1} {2}", Nombre, Paterno, Materno);
            }
        }
        public string FechaNacimientoConFormatoIngles
        {
            get
            {
                if (string.IsNullOrEmpty(FechaNacimiento))
                    return FechaNacimiento;
                else
                    return DateTime.Parse(FechaNacimiento).ToString("MM/dd/yyyy");
            }
        }
        public string FechaNacimientoConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(FechaNacimiento))
                    return FechaNacimiento;
                else
                {
                    if(1900 > FechaNacimientoAsDateTime.Value.Year)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return DateTime.Parse(FechaNacimiento).ToString("dd/MM/yyyy");
                    }
                }
            }
        }
        public DateTime? FechaNacimientoAsDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(FechaNacimiento))
                    return null;
                else
                    return DateTime.Parse(FechaNacimiento);
            }
        }
        public string ExpiracionMembresiaConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(ExpiracionMembresiaAsString))
                    return ExpiracionMembresiaAsString;
                else
                    return DateTime.Parse(ExpiracionMembresiaAsString).ToString("dd/MM/yyyy");
            }
        }
        public string TelefonoConFormato
        {
            get
            {
                if (UInt64.TryParse(Telefono, out UInt64 PhoneAsInt))
                {
                    return String.Format("{0:(###) ###-####}", PhoneAsInt);
                }
                else
                {
                    return Telefono;
                }
            }
        }
        public string SexoAsString
        {
            get
            {
                if (int.TryParse(Sexo, out int SexoAsInt))
                {
                    return SexosHelper.IntToGender[SexoAsInt];
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public int SexoAsInt
        {
            get
            {
                if (int.TryParse(Sexo, out int s))
                {
                    return s;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int CiudadAsInt
        {
            get
            {
                if (int.TryParse(CiudadId, out int c))
                {
                    return c;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int ColoniaAsInt
        {
            get
            {
                if (int.TryParse(ColoniaId, out int c))
                {
                    return c;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int PuntosActualesAsInt
        {
            get
            {
                if (float.TryParse(PuntosActuales, out float p))
                    return (int)p;
                else
                    return 0;
            }
        }
    }
}
