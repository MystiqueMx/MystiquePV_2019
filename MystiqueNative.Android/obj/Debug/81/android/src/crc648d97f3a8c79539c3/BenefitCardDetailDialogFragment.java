package crc648d97f3a8c79539c3;


public class BenefitCardDetailDialogFragment
	extends android.support.v7.app.AppCompatDialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Fragments.BenefitCardDetailDialogFragment, Fresco", BenefitCardDetailDialogFragment.class, __md_methods);
	}


	public BenefitCardDetailDialogFragment ()
	{
		super ();
		if (getClass () == BenefitCardDetailDialogFragment.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Fragments.BenefitCardDetailDialogFragment, Fresco", "", this, new java.lang.Object[] {  });
		}
	}

	public BenefitCardDetailDialogFragment (java.lang.String p0, java.lang.String p1)
	{
		super ();
		if (getClass () == BenefitCardDetailDialogFragment.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Fragments.BenefitCardDetailDialogFragment, Fresco", "System.String, mscorlib:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);

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
