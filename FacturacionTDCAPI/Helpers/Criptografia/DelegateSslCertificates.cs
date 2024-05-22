using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace FacturacionTDCAPI.Helpers.Criptografia
{
    public class DelegateSslCertificates : IDisposable
    {
        private readonly Chilkat.Cert _certificado;
        #region Ctor
        public DelegateSslCertificates(string pathCertificado)
        {
            if (!File.Exists(pathCertificado)) throw new FileNotFoundException(pathCertificado);

            _certificado = new Chilkat.Cert();

            if (!_certificado.LoadFromFile(pathCertificado))
                throw new CryptographicException(_certificado.LastErrorText);
        }
        #endregion

        #region Methods

        public string ObtenerNumeroCertificado()
        {
            string serialNum;
            try
            {
                serialNum = _certificado.SerialNumber;
                serialNum = HexadecimalToString(serialNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return serialNum;
        }
        public string ObtenerCertificadoComoString()
        {
            string certificado;
            try
            {
                var certificadoEnconded = _certificado.GetEncoded();
                certificado = certificadoEnconded
                    .Replace("\r", "")
                    .Replace("\n", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return certificado;
        }
        #endregion

        #region Helpers
        private static string HexadecimalToString(string data)
        {
            var sData = "";

            while (data.Length > 0)
            {
                var data1 = Convert.ToChar(Convert.ToUInt32(data.Substring(0, 2), 16)).ToString();
                sData = sData + data1;
                data = data.Substring(2, data.Length - 2);
            }
            return sData;
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            _certificado?.Dispose();
        }
        #endregion
    }
}