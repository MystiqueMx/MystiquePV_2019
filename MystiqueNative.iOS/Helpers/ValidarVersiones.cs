using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace MystiqueNative.iOS.Helpers
{
    public class ValidarVersiones
    {
        
        public static bool ValidarAppVersion()
        {
            NSObject Appversion = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
#if DEBUG
            return true;
#else
            //PRODUCCION:
            if (MystiqueApp.VersionIOS == Appversion.ToString() || MystiqueApp.VersionIOSPruebas == Appversion.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
#endif

        }
    }
}