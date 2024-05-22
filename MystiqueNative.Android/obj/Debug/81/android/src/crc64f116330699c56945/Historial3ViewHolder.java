package crc64f116330699c56945;


public class Historial3ViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Historial3ViewHolder, Fresco", Historial3ViewHolder.class, __md_methods);
	}


	public Historial3ViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == Historial3ViewHolder.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Historial3ViewHolder, Fresco", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
