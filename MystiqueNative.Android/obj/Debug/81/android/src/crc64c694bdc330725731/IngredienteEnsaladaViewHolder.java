package crc64c694bdc330725731;


public class IngredienteEnsaladaViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.HazPedido.Ensaladas.IngredienteEnsaladaViewHolder, Fresco", IngredienteEnsaladaViewHolder.class, __md_methods);
	}


	public IngredienteEnsaladaViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == IngredienteEnsaladaViewHolder.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.HazPedido.Ensaladas.IngredienteEnsaladaViewHolder, Fresco", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
