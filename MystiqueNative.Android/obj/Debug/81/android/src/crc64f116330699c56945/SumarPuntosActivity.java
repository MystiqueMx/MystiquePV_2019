package crc64f116330699c56945;


public class SumarPuntosActivity
	extends crc6402a654862b66bf02.BaseActivity
	implements
		mono.android.IGCUserPeer,
		android.media.MediaPlayer.OnCompletionListener,
		android.media.MediaPlayer.OnPreparedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"n_onPause:()V:GetOnPauseHandler\n" +
			"n_onStop:()V:GetOnStopHandler\n" +
			"n_onOptionsItemSelected:(Landroid/view/MenuItem;)Z:GetOnOptionsItemSelected_Landroid_view_MenuItem_Handler\n" +
			"n_onCompletion:(Landroid/media/MediaPlayer;)V:GetOnCompletion_Landroid_media_MediaPlayer_Handler:Android.Media.MediaPlayer/IOnCompletionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onPrepared:(Landroid/media/MediaPlayer;)V:GetOnPrepared_Landroid_media_MediaPlayer_Handler:Android.Media.MediaPlayer/IOnPreparedListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("MystiqueNative.Droid.SumarPuntosActivity, Fresco", SumarPuntosActivity.class, __md_methods);
	}


	public SumarPuntosActivity ()
	{
		super ();
		if (getClass () == SumarPuntosActivity.class) {
			mono.android.TypeManager.Activate ("MystiqueNative.Droid.SumarPuntosActivity, Fresco", "", this, new java.lang.Object[] {  });
		}
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();


	public void onPause ()
	{
		n_onPause ();
	}

	private native void n_onPause ();


	public void onStop ()
	{
		n_onStop ();
	}

	private native void n_onStop ();


	public boolean onOptionsItemSelected (android.view.MenuItem p0)
	{
		return n_onOptionsItemSelected (p0);
	}

	private native boolean n_onOptionsItemSelected (android.view.MenuItem p0);


	public void onCompletion (android.media.MediaPlayer p0)
	{
		n_onCompletion (p0);
	}

	private native void n_onCompletion (android.media.MediaPlayer p0);


	public void onPrepared (android.media.MediaPlayer p0)
	{
		n_onPrepared (p0);
	}

	private native void n_onPrepared (android.media.MediaPlayer p0);

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
