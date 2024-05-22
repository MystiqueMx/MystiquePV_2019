package crc645bd43fab4133b172;


public class DetachableResultReceiver
	extends android.os.ResultReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceiveResult:(ILandroid/os/Bundle;)V:GetOnReceiveResult_ILandroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Utils.DetachableResultReceiver, Fresco", DetachableResultReceiver.class, __md_methods);
	}


	public DetachableResultReceiver (android.os.Handler p0)
	{
		super (p0);
		if (getClass () == DetachableResultReceiver.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Utils.DetachableResultReceiver, Fresco", "Android.OS.Handler, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}


	public void onReceiveResult (int p0, android.os.Bundle p1)
	{
		n_onReceiveResult (p0, p1);
	}

	private native void n_onReceiveResult (int p0, android.os.Bundle p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
