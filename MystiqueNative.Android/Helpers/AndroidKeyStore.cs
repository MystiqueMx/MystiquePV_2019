using System;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Security;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;

namespace MystiqueNative.Droid.Utils
{
    internal class AndroidKeyStore
    {
        private const string AndroidKeyStoreKey = "AndroidKeyStore"; // this is an Android const value
        private const string AesAlgorithm = "AES";
        private const string CipherTransformationAsymmetric = "RSA/ECB/PKCS1Padding";
        private const string CipherTransformationSymmetric = "AES/GCM/NoPadding";
        private const string PrefsMasterKey = "SecureStorageKey";
        private const int InitializationVectorLen = 12; // Android supports an IV of 12 for AES/GCM

        internal AndroidKeyStore(Context context, string keystoreAlias, bool alwaysUseAsymmetricKeyStorage)
        {
            _alwaysUseAsymmetricKey = alwaysUseAsymmetricKeyStorage;
            _appContext = context;
            _alias = keystoreAlias;

            _keyStore = KeyStore.GetInstance(AndroidKeyStoreKey);
            _keyStore.Load(null);
        }

        private readonly Context _appContext;
        private readonly string _alias;
        private readonly KeyStore _keyStore;
        private readonly bool _alwaysUseAsymmetricKey;

        private ISecretKey GetKey()
        {
            // If >= API 23 we can use the KeyStore's symmetric key
            if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.M && !_alwaysUseAsymmetricKey)
                return GetSymmetricKey();

            // NOTE: KeyStore in < API 23 can only store asymmetric keys
            // specifically, only RSA/ECB/PKCS1Padding
            // So we will wrap our symmetric AES key we just generated
            // with this and save the encrypted/wrapped key out to
            // preferences for future use.
            // ECB should be fine in this case as the AES key should be
            // contained in one block.

            // Get the asymmetric key pair
            var keyPair = GetAsymmetricKeyPair();

            using (var prefs = _appContext.GetSharedPreferences(_alias, FileCreationMode.Private))
            {
                var existingKeyStr = prefs.GetString(PrefsMasterKey, null);

                if (!string.IsNullOrEmpty(existingKeyStr))
                {
                    var wrappedKey = Convert.FromBase64String(existingKeyStr);

                    var unwrappedKey = UnwrapKey(wrappedKey, keyPair.Private);
                    var kp = unwrappedKey.JavaCast<ISecretKey>();

                    return kp;
                }
                else
                {
                    var keyGenerator = KeyGenerator.GetInstance(AesAlgorithm);
                    var defSymmetricKey = keyGenerator.GenerateKey();

                    var wrappedKey = WrapKey(defSymmetricKey, keyPair.Public);

                    using (var prefsEditor = prefs.Edit())
                    {
                        prefsEditor.PutString(PrefsMasterKey, Convert.ToBase64String(wrappedKey));
                        prefsEditor.Commit();
                    }

                    return defSymmetricKey;
                }
            }
        }

        // API 23+ Only
        private ISecretKey GetSymmetricKey()
        {
            var existingKey = _keyStore.GetKey(_alias, null);

            if (existingKey != null)
            {
                var existingSecretKey = existingKey.JavaCast<ISecretKey>();
                return existingSecretKey;
            }

            var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, AndroidKeyStoreKey);
            var builder = new KeyGenParameterSpec.Builder(_alias, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(KeyProperties.BlockModeGcm)
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
                .SetRandomizedEncryptionRequired(false);

            keyGenerator.Init(builder.Build());

            return keyGenerator.GenerateKey();
        }

        public KeyPair GetAsymmetricKeyPair()
        {
            var asymmetricAlias = $"{_alias}.asymmetric";

            var privateKey = _keyStore.GetKey(asymmetricAlias, null)?.JavaCast<IPrivateKey>();
            var publicKey = _keyStore.GetCertificate(asymmetricAlias)?.PublicKey;

            // Return the existing key if found
            if (privateKey != null && publicKey != null)
                return new KeyPair(publicKey, privateKey);

            // Otherwise we create a new key
            var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, AndroidKeyStoreKey);

            var end = DateTime.UtcNow.AddYears(20);
            var startDate = new Java.Util.Date();
            var endDate = new Java.Util.Date(end.Year, end.Month, end.Day);

#pragma warning disable CS0618
            var builder = new KeyPairGeneratorSpec.Builder(CurrentActivityDelegate.Instance.AppContext)
                .SetAlias(asymmetricAlias)
                .SetSerialNumber(Java.Math.BigInteger.One)
                .SetSubject(new Javax.Security.Auth.X500.X500Principal($"CN={asymmetricAlias} CA Certificate"))
                .SetStartDate(startDate)
                .SetEndDate(endDate);

            generator.Initialize(builder.Build());
#pragma warning restore CS0618

            return generator.GenerateKeyPair();
        }

        private static byte[] WrapKey(IKey keyToWrap, IKey withKey)
        {
            var cipher = Cipher.GetInstance(CipherTransformationAsymmetric);
            cipher.Init(CipherMode.WrapMode, withKey);
            return cipher.Wrap(keyToWrap);
        }

        private static IKey UnwrapKey(byte[] wrappedData, IKey withKey)
        {
            var cipher = Cipher.GetInstance(CipherTransformationAsymmetric);
            cipher.Init(CipherMode.UnwrapMode, withKey);
            var unwrapped = cipher.Unwrap(wrappedData, KeyProperties.KeyAlgorithmAes, KeyType.SecretKey);
            return unwrapped;
        }

        internal byte[] Encrypt(string data)
        {
            var key = GetKey();

            // Generate initialization vector
            var iv = new byte[InitializationVectorLen];
            var sr = new SecureRandom();
            sr.NextBytes(iv);

            Cipher cipher;

            // Attempt to use GCMParameterSpec by default
            try
            {
                cipher = Cipher.GetInstance(CipherTransformationSymmetric);
                cipher.Init(CipherMode.EncryptMode, key, new GCMParameterSpec(128, iv));
            }
            catch (Java.Security.InvalidAlgorithmParameterException)
            {
                // If we encounter this error, it's likely an old bouncycastle provider version
                // is being used which does not recognize GCMParameterSpec, but should work
                // with IvParameterSpec, however we only do this as a last effort since other
                // implementations will error if you use IvParameterSpec when GCMParameterSpec
                // is recognized and expected.
                cipher = Cipher.GetInstance(CipherTransformationSymmetric);
                cipher.Init(CipherMode.EncryptMode, key, new IvParameterSpec(iv));
            }

            var decryptedData = Encoding.UTF8.GetBytes(data);
            var encryptedBytes = cipher.DoFinal(decryptedData);

            // Combine the IV and the encrypted data into one array
            var r = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, r, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, r, iv.Length, encryptedBytes.Length);

            return r;
        }

        internal string Decrypt(byte[] data)
        {
            if (data.Length < InitializationVectorLen)
                return null;

            var key = GetKey();

            // IV will be the first 16 bytes of the encrypted data
            var iv = new byte[InitializationVectorLen];
            Buffer.BlockCopy(data, 0, iv, 0, InitializationVectorLen);

            Cipher cipher;

            // Attempt to use GCMParameterSpec by default
            try
            {
                cipher = Cipher.GetInstance(CipherTransformationSymmetric);
                cipher.Init(CipherMode.DecryptMode, key, new GCMParameterSpec(128, iv));
            }
            catch (InvalidAlgorithmParameterException)
            {
                // If we encounter this error, it's likely an old bouncycastle provider version
                // is being used which does not recognize GCMParameterSpec, but should work
                // with IvParameterSpec, however we only do this as a last effort since other
                // implementations will error if you use IvParameterSpec when GCMParameterSpec
                // is recognized and expected.
                cipher = Cipher.GetInstance(CipherTransformationSymmetric);
                cipher.Init(CipherMode.DecryptMode, key, new IvParameterSpec(iv));
            }

            // Decrypt starting after the first 16 bytes from the IV
            var decryptedData = cipher.DoFinal(data, InitializationVectorLen, data.Length - InitializationVectorLen);

            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}