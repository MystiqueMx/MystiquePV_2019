package crc648d97f3a8c79539c3;


public class BarcodeDialogFragment
	extends android.support.v7.app.AppCompatDialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"n_onDismiss:(Landroid/content/DialogInterface;)V:GetOnDismiss_Landroid_content_DialogInterface_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Fragments.BarcodeDialogFragment, Fresco", BarcodeDialogFragment.class, __md_methods);
	}


	public BarcodeDialogFragment ()
	{
		super ();
		if (getClass () == BarcodeDialogFragment.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Fragments.BarcodeDialogFragment, Fresco", "", this, new java.lang.Object[] {  });
		}
	}

	public BarcodeDialogFragment (java.lang.String p0, int p1, java.lang.String p2)
	{
		super ();
		if (getClass () == BarcodeDialogFragment.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Fragments.BarcodeDialogFragment, Fresco", "System.String, mscorlib:ZXing.BarcodeFormat, zxing.portable:System.String, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);


	public void onDismiss (android.content.DialogInterface p0)
	{
		n_onDismiss (p0);
	}

	private native void n_onDismiss (android.content.DialogInterface p0);

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
