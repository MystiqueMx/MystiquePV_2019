using System;
using Android.Content;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;

namespace MystiqueNative.Droid.Helpers
{
    public static class PreferencesHelper
    {
        private const string KeyName = "COM.MYSTIQUE.APP_FRESCO_PREFERENCES";
        #region SETTINGS NAME

        private const string LoginMethod = "LoginMethodEnabled";
        private const string FingerprintUser = "FPUsuario";
        private const string FingerprintPassword = "FPPassword";
        private const string FbUser = "FBUsuario";
        private const string FbPassword = "FBPassword";

        private const string ReferenceCheckbox = "RefCheckbox";
        //const string REFERENCE_CHECKBOX = "RefCheckbox";
        #endregion
        #region SETTINGS
        public static void SetRememberMeAuth(string user, string password)
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutString(FingerprintUser, user);
            authPref.PutString(FingerprintPassword, password);
            authPref.Commit();
        }
        [Java.Lang.Deprecated]
        public static void ClearCredentials()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutString(FingerprintUser, string.Empty);
            authPref.PutString(FingerprintPassword, string.Empty);
            authPref.Commit();
        }
        [Java.Lang.Deprecated]
        public static string GetRemembermeUser()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return authPref.GetString(FingerprintUser, string.Empty);
        }
        [Java.Lang.Deprecated]
        public static string GetRemembermePassword()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return authPref.GetString(FingerprintPassword, string.Empty);
        }
        public static void SetReferenceDialog(bool checkbox)
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutBoolean(ReferenceCheckbox, checkbox);
            authPref.Commit();
        }
        public static bool GetReferenceDialog()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return authPref.GetBoolean(ReferenceCheckbox, false);
        }
        [Java.Lang.Deprecated]
        public static void UpdatePassword(string password)
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            if (!string.IsNullOrEmpty(authPref.GetString(FingerprintUser, string.Empty)))
            {
                var editor = authPref.Edit();
                editor.PutString(FingerprintPassword, password);
                editor.Commit();
            }
        }
        [Java.Lang.Deprecated]
        internal static void SetFacebookAuth(string email, string facebookId)
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutString(FbUser, email);
            authPref.PutString(FbPassword, facebookId);
            authPref.Commit();
        }
        [Java.Lang.Deprecated]
        public static string GetFacebookUser()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return authPref.GetString(FbUser, string.Empty);
        }
        [Java.Lang.Deprecated]
        public static string GetFacebookPassword()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return authPref.GetString(FbPassword, string.Empty);
        }
        [Java.Lang.Deprecated]
        public static void ClearFacebookCredentials()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutString(FbUser, string.Empty);
            authPref.PutString(FbPassword, string.Empty);
            authPref.Commit();
        }
        public static void SetSavedLoginMethod(AuthMethods method)
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private).Edit();
            authPref.PutInt(LoginMethod, (int)method);
            authPref.Commit();
        }
        public static AuthMethods GetSavedLoginMethod()
        {
            var authPref = CurrentActivityDelegate.Instance.Activity.GetSharedPreferences(KeyName, FileCreationMode.Private);
            return (AuthMethods)authPref.GetInt(LoginMethod, 0);
        }
        #endregion
    }
}