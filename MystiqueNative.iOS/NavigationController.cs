using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class NavigationController : UINavigationController
    {
        public NavigationController (IntPtr handle) : base (handle)
        {
        }
    }
}