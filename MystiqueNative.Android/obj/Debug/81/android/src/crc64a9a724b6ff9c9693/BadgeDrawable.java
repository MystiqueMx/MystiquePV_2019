package crc64a9a724b6ff9c9693;


public class BadgeDrawable
	extends android.graphics.drawable.Drawable
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getOpacity:()I:GetGetOpacityHandler\n" +
			"n_draw:(Landroid/graphics/Canvas;)V:GetDraw_Landroid_graphics_Canvas_Handler\n" +
			"n_setAlpha:(I)V:GetSetAlpha_IHandler\n" +
			"n_setColorFilter:(Landroid/graphics/ColorFilter;)V:GetSetColorFilter_Landroid_graphics_ColorFilter_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Views.BadgeDrawable, Fresco", BadgeDrawable.class, __md_methods);
	}


	public BadgeDrawable ()
	{
		super ();
		if (getClass () == BadgeDrawable.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Views.BadgeDrawable, Fresco", "", this, new java.lang.Object[] {  });
		}
	}

	public BadgeDrawable (android.content.Context p0)
	{
		super ();
		if (getClass () == BadgeDrawable.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Views.BadgeDrawable, Fresco", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}


	public int getOpacity ()
	{
		return n_getOpacity ();
	}

	private native int n_getOpacity ();


	public void draw (android.graphics.Canvas p0)
	{
		n_draw (p0);
	}

	private native void n_draw (android.graphics.Canvas p0);


	public void setAlpha (int p0)
	{
		n_setAlpha (p0);
	}

	private native void n_setAlpha (int p0);


	public void setColorFilter (android.graphics.ColorFilter p0)
	{
		n_setColorFilter (p0);
	}

	private native void n_setColorFilter (android.graphics.ColorFilter p0);

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
