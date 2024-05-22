package crc64fc24bcaec41df248;


public class DireccionViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.HazPedido.Direccion.DireccionViewHolder, Fresco", DireccionViewHolder.class, __md_methods);
	}


	public DireccionViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == DireccionViewHolder.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.HazPedido.Direccion.DireccionViewHolder, Fresco", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
