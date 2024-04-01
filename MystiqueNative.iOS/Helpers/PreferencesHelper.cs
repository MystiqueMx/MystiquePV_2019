using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace MystiqueNative.iOS.Helpers
{
    public static class PreferencesHelper
    {
        #region Declaraciones Variables
        const string _USER = "USR";
        const string _PASSWORD = "PASS";
        const string MODALSHOW = "MODAL";

        const string _MOSTRARMENSAJE = "true";

        const string _USERFB = "USR_FB";
        const string _PASSWORDFB = "PASS_FB";

        const string _USERGOOGLE = "USR_GOOGLE";
        const string _PASSWORDGOOGLE = "PASS_GOOGLE";

        const string _USERTWITTER = "USR_TWITTER";
        const string _PASSWORDTWITTER = "PASS_TWITTER";
        #endregion

        #region Set Credencials
        public static void SetCredentials(string user, string password)
        {
            NSUserDefaults.StandardUserDefaults.SetString(user, _USER);
            NSUserDefaults.StandardUserDefaults.SetString(password, _PASSWORD);
        }
        public static void SetCredentialsFB(string user, string password)
        {
            NSUserDefaults.StandardUserDefaults.SetString(user, _USERFB);
            NSUserDefaults.StandardUserDefaults.SetString(password, _PASSWORDFB);
        }
        public static void SetCredentialsGoogle(string user, string password)
        {
            NSUserDefaults.StandardUserDefaults.SetString(user, _USERGOOGLE);
            NSUserDefaults.StandardUserDefaults.SetString(password, _PASSWORDGOOGLE);
        }
        public static void SetCredentialsTwitter(string user, string password)
        {
            NSUserDefaults.StandardUserDefaults.SetString(user, _USERTWITTER);
            NSUserDefaults.StandardUserDefaults.SetString(password, _PASSWORDTWITTER);
        }
        #endregion

        #region Get Usernames

        public static string GetUser()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_USER);
        }
        public static string GetUserFB()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_USERFB);
        }
        public static string GetUserGoogle()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_USERGOOGLE);
        }
        public static string GetUserTwitter()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_USERTWITTER);
        }
        #endregion

        #region Get Passwords
        public static string GetPassword()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORD);
        }
        public static string GetPasswordFB()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDFB);
        }
        public static string GetPasswordGoogle()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDGOOGLE);
        }
        public static string GetPasswordTwitter()
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDTWITTER);
        }
        #endregion

        #region Is User Save
        public static bool IsUserSave()
        {
            return !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_USER)) &&
                   !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORD));
        }
        public static bool IsUserSaveFB()
        {
            return !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_USERFB)) &&
                   !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDFB));
        }
        public static bool IsUserSaveGoogle()
        {
            return !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_USERGOOGLE)) &&
                   !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDGOOGLE));
        }
        public static bool IsUserSaveTwitter()
        {
            return !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_USERTWITTER)) &&
                   !string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_PASSWORDTWITTER));
        }
        #endregion

        public static bool GetModalShow()
        {
            return NSUserDefaults.StandardUserDefaults.BoolForKey(MODALSHOW);
        }
        public static void SetModalShow(bool dismiss)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(dismiss, MODALSHOW);
        }
        public static void UpdatePassword(string password)
        {
            if (!string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(_USER)))
                NSUserDefaults.StandardUserDefaults.SetString(password, _PASSWORD);
        }
        public static void SetMostrarMensaje(bool Mostrar)
        {

            NSUserDefaults.StandardUserDefaults.SetBool(Mostrar, _MOSTRARMENSAJE);
        }
        public static bool GetMostrar()
        {
            return NSUserDefaults.StandardUserDefaults.BoolForKey(_MOSTRARMENSAJE);
        }

    }
}