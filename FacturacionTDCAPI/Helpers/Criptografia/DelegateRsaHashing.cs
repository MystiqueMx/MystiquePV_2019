using Chilkat;
using System;
using System.Configuration;
using System.IO;

namespace FacturacionTDCAPI.Helpers.Criptografia
{
    public class DelegateRsaHashing : IDisposable
    {
        private readonly string _chilkatLicenseKey = ConfigurationManager.AppSettings["CHILKAT_LICENSE_KEY"];
        private readonly Rsa _rsa;
        private Cert _certificado;
       
    //#region Ctor
    public DelegateRsaHashing(string pathPrivateKey, string passwordPrivateKey, string pathCertificado)
        {
            if (!File.Exists(pathPrivateKey)) throw new FileNotFoundException(pathPrivateKey);

            _certificado = new Chilkat.Cert();
            _rsa = new Rsa
            {
                EncodingMode = "base64",
                LittleEndian = false,
            };
            var privateKey = new PrivateKey();

            //if (!_certificado.LoadFromFile(pathCertificado))
            //    throw new CryptographicException(_certificado.LastErrorText);

            if (!privateKey.LoadPkcs8EncryptedFile(pathPrivateKey, passwordPrivateKey))
                throw new Exception(privateKey.LastErrorText);

            //if (!_certificado.SetPrivateKey(privateKey))
            //    throw new Exception(_certificado.LastErrorText);

            if (!_rsa.UnlockComponent(_chilkatLicenseKey))
                throw new AccessViolationException(_rsa.LastErrorText);

            if (!_rsa.ImportPrivateKey(privateKey.GetXml()))
                throw new AccessViolationException(_rsa.LastErrorText);

        }

        //#endregion
        #region Methods
        public string FirmarConLlavePrivada(string cadena)
        {
            string selloDigital;
            try
            {
                //agregar llave privada al cer
                //if (!_certificado.SetPrivateKey(_privateKey))
                //    throw new CryptographicException(_certificado.LastErrorText);
            
                
                //Firmar cadenaOriginal dentro byteArray
                selloDigital = _rsa.SignStringENC(cadena, "SHA-256");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return selloDigital;
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            _rsa?.Dispose();
        }

        #endregion
    }
}