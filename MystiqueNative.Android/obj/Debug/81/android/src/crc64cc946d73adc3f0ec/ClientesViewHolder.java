package crc64cc946d73adc3f0ec;


public class ClientesViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Activities.ClientesViewHolder, Fresco", ClientesViewHolder.class, __md_methods);
	}


	public ClientesViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ClientesViewHolder.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Activities.ClientesViewHolder, Fresco", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
