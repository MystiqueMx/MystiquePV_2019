using Android.Content;
using System;
using System.Threading.Tasks;

namespace MystiqueNative.Droid.Utils
{
    public static class SecureStorageHelper
    {
        internal static readonly string Alias = "COM.MYSTIQUE.APP_FRESCO_SECURE_PREFERENCES";
        private const string UsernameKey = "Username";
        private const string PasswordKey = "UsernamePass";
        public static async Task SetCredentialsAsync(string username, string password)
        {
            await SetAsync(UsernameKey, username);
            await SetAsync(PasswordKey, password);
        }
        public static Task<string> GetPasswordAsync() => GetAsync(PasswordKey);
        public static Task<string> GetUsernameAsync() => GetAsync(UsernameKey);
        public static Task<string> GetAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return PlatformGetAsync(key);
        }

        public static Task SetAsync(string key, string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));

                if (value == null)
                    throw new ArgumentNullException(nameof(value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return PlatformSetAsync(key, value);
        }

        public static bool Remove(string key)
            => PlatformRemove(key);

        public static void RemoveAll()
            => PlatformRemoveAll();
        static Task<string> PlatformGetAsync(string key)
        {
            try
            {
                var context = CurrentActivityDelegate.Instance.AppContext;

                string encStr;
                using (var prefs = context.GetSharedPreferences(Alias, FileCreationMode.Private))
                    encStr = prefs.GetString(Helpers.Utils.Md5Hash(key), null);

                string decryptedData = null;
                if (string.IsNullOrEmpty(encStr)) return Task.FromResult((string)null);
                var encData = Convert.FromBase64String(encStr);
                var ks = new AndroidKeyStore(context, Alias, AlwaysUseAsymmetricKeyStorage);
                decryptedData = ks.Decrypt(encData);

                return Task.FromResult(decryptedData);
            }
            catch (Exception ex)
            {
                Remove(key);
                Console.WriteLine(ex);
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Debug, "FRESCO PlatformGetAsync SECURESTORAGEHELPER", ex.ToString());
                return null;
            }

        }

        private static Task PlatformSetAsync(string key, string data)
        {
            try
            {
                var context = CurrentActivityDelegate.Instance.AppContext;
                var ks = new AndroidKeyStore(context, Alias, AlwaysUseAsymmetricKeyStorage);
                var encryptedData = ks.Encrypt(data);

                using (var prefs = context.GetSharedPreferences(Alias, FileCreationMode.Private))
                using (var prefsEditor = prefs.Edit())
                {
                    var encStr = Convert.ToBase64String(encryptedData);
                    prefsEditor.PutString(Helpers.Utils.Md5Hash(key), encStr);
                    prefsEditor.Commit();
                }

                return Task.CompletedTask;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Task.CompletedTask;
            }

        }

        private static bool PlatformRemove(string key)
        {
            var context = CurrentActivityDelegate.Instance.AppContext;

            key = Helpers.Utils.Md5Hash(key);

            using (var prefs = context.GetSharedPreferences(Alias, FileCreationMode.Private))
            {
                if (!prefs.Contains(key)) return false;
                using (var prefsEditor = prefs.Edit())
                {
                    prefsEditor.Remove(key);
                    prefsEditor.Commit();
                    return true;
                }
            }

            return false;
        }

        private static void PlatformRemoveAll()
        {
            var context = CurrentActivityDelegate.Instance.AppContext;

            using (var prefs = context.GetSharedPreferences(Alias, FileCreationMode.Private))
            using (var prefsEditor = prefs.Edit())
            {
                foreach (var key in prefs.All.Keys)
                    prefsEditor.Remove(key);

                prefsEditor.Commit();
            }
        }

        internal static bool AlwaysUseAsymmetricKeyStorage { get; set; } = false;
    }
}