using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using MystiqueNative.ViewModels;
using UIKit;

namespace MystiqueNative.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        public static AuthViewModel Auth { get; set; }
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        
    }
}
