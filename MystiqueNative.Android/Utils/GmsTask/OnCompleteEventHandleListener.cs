using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Utils.Location
{
    public class OnCompleteEventHandleListener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Task> _completeAction;
        public OnCompleteEventHandleListener(Action<Task> completeAction)
        {
            _completeAction = completeAction;
        }

        public void OnComplete(Task task) => _completeAction(task);
    }

}