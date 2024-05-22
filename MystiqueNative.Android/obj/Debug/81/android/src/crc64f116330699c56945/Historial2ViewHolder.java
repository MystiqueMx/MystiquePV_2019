package crc64f116330699c56945;


public class Historial2ViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Historial2ViewHolder, Fresco", Historial2ViewHolder.class, __md_methods);
	}


	public Historial2ViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == Historial2ViewHolder.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Historial2ViewHolder, Fresco", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}

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
