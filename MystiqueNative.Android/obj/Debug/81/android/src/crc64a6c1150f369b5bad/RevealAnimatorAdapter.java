package crc64a6c1150f369b5bad;


public class RevealAnimatorAdapter
	extends android.animation.AnimatorListenerAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAnimationEnd:(Landroid/animation/Animator;)V:GetOnAnimationEnd_Landroid_animation_Animator_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Animations.RevealAnimatorAdapter, Fresco", RevealAnimatorAdapter.class, __md_methods);
	}


	public RevealAnimatorAdapter ()
	{
		super ();
		if (getClass () == RevealAnimatorAdapter.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Animations.RevealAnimatorAdapter, Fresco", "", this, new java.lang.Object[] {  });
		}
	}

	public RevealAnimatorAdapter (android.view.View p0, android.app.Activity p1)
	{
		super ();
		if (getClass () == RevealAnimatorAdapter.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Animations.RevealAnimatorAdapter, Fresco", "Android.Views.View, Mono.Android:Android.App.Activity, Mono.Android", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public void onAnimationEnd (android.animation.Animator p0)
	{
		n_onAnimationEnd (p0);
	}

	private native void n_onAnimationEnd (android.animation.Animator p0);

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
