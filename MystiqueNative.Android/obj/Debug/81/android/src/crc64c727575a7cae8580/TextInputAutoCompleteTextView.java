package crc64c727575a7cae8580;


public class TextInputAutoCompleteTextView
	extends android.support.v7.widget.AppCompatAutoCompleteTextView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateInputConnection:(Landroid/view/inputmethod/EditorInfo;)Landroid/view/inputmethod/InputConnection;:GetOnCreateInputConnection_Landroid_view_inputmethod_EditorInfo_Handler\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.Utils.Views.TextInputAutoCompleteTextView, Fresco", TextInputAutoCompleteTextView.class, __md_methods);
	}


	public TextInputAutoCompleteTextView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == TextInputAutoCompleteTextView.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Utils.Views.TextInputAutoCompleteTextView, Fresco", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
		}
	}


	public TextInputAutoCompleteTextView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == TextInputAutoCompleteTextView.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Utils.Views.TextInputAutoCompleteTextView, Fresco", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
		}
	}


	public TextInputAutoCompleteTextView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == TextInputAutoCompleteTextView.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.Utils.Views.TextInputAutoCompleteTextView, Fresco", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
		}
	}


	public android.view.inputmethod.InputConnection onCreateInputConnection (android.view.inputmethod.EditorInfo p0)
	{
		return n_onCreateInputConnection (p0);
	}

	private native android.view.inputmethod.InputConnection n_onCreateInputConnection (android.view.inputmethod.EditorInfo p0);

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
