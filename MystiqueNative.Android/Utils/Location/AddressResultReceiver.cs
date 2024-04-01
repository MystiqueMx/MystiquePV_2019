using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Utils
{
    public class DetachableResultReceiver : ResultReceiver 
    {
        public IReceiver ReceiverObj;
	
        public DetachableResultReceiver(Handler handler) : base(handler)
        {    
        }
	
        public void ClearReceiver() {
            ReceiverObj = null;
        }
	
        public void SetReceiver(IReceiver receiver) {
            ReceiverObj = receiver;
        }
	
        public interface IReceiver 
        {
            void OnReceiveResult(int resultCode, Bundle resultData);
        }
	
        protected override void OnReceiveResult (int resultCode, Bundle resultData)
        {	
            if (ReceiverObj != null) {
                ReceiverObj.OnReceiveResult(resultCode, resultData);
            } else {
                Log.Warn("DetachableResultReceiver", "Dropping result on floor for code " + resultCode + ": " + resultData);
            }
        }
    }
}