package crc64f116330699c56945;


public class FacebookUserRequest
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.GraphRequest.Callback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCompleted:(Lcom/facebook/GraphResponse;)V:GetOnCompleted_Lcom_facebook_GraphResponse_Handler:Xamarin.Facebook.GraphRequest/ICallbackInvoker, Xamarin.Facebook.Core.Android\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.FacebookUserRequest, Fresco", FacebookUserRequest.class, __md_methods);
	}


	public FacebookUserRequest ()
	{
		super ();
		if (getClass () == FacebookUserRequest.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.FacebookUserRequest, Fresco", "", this, new java.lang.Object[] {  });
		}
	}


	public void onCompleted (com.facebook.GraphResponse p0)
	{
		n_onCompleted (p0);
	}

	private native void n_onCompleted (com.facebook.GraphResponse p0);

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
