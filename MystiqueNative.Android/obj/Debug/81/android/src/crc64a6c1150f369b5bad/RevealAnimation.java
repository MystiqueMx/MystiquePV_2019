package crc64a6c1150f369b5bad;


public class RevealAnimation
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.ViewTreeObserver.OnGlobalLayoutListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGlobalLayout:()V:GetOnGlobalLayoutHandler:Android.Views.ViewTreeObserver/IOnGlobalLayoutListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Animations.RevealAnimation, Fresco", RevealAnimation.class, __md_methods);
	}


	public RevealAnimation ()
	{
		super ();
		if (getClass () == RevealAnimation.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Animations.RevealAnimation, Fresco", "", this, new java.lang.Object[] {  });
		}
	}

	public RevealAnimation (android.view.View p0, android.content.Intent p1, android.app.Activity p2)
	{
		super ();
		if (getClass () == RevealAnimation.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Animations.RevealAnimation, Fresco", "Android.Views.View, Mono.Android:Android.Content.Intent, Mono.Android:Android.App.Activity, Mono.Android", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}


	public void onGlobalLayout ()
	{
		n_onGlobalLayout ();
	}

	private native void n_onGlobalLayout ();

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
