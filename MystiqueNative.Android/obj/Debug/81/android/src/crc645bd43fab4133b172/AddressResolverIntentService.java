package crc645bd43fab4133b172;


public class AddressResolverIntentService
	extends mono.android.app.IntentService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onHandleIntent:(Landroid/content/Intent;)V:GetOnHandleIntent_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Utils.AddressResolverIntentService, Fresco", AddressResolverIntentService.class, __md_methods);
	}


	public AddressResolverIntentService ()
	{
		super ();
		if (getClass () == AddressResolverIntentService.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Utils.AddressResolverIntentService, Fresco", "", this, new java.lang.Object[] {  });
		}
	}


	public void onHandleIntent (android.content.Intent p0)
	{
		n_onHandleIntent (p0);
	}

	private native void n_onHandleIntent (android.content.Intent p0);

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
