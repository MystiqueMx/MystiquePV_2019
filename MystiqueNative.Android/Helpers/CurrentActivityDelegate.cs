using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;

namespace MystiqueNative.Droid.Utils
{
    public static class CurrentActivityDelegate
    {
        static Lazy<ICurrentActivity> implementation = new Lazy<ICurrentActivity>(() => CreateCurrentActivity(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static ICurrentActivity Instance
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ICurrentActivity CreateCurrentActivity()
        {
            return new CurrentActivityImplementation();
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
    public interface ICurrentActivity
    {
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        BaseActivity Activity { get; set; }

        /// <summary>
        /// Gets the current Application Context.
        /// </summary>
        /// <value>The app context.</value>
        Context AppContext { get; }

        /// <summary>
        /// Fires when activity state events are fired
        /// </summary>
        event EventHandler<ActivityEventArgs> ActivityStateChanged;

        /// <summary>
        /// Initialize Current Activity Plugin with Application
        /// </summary>
        /// <param name="application"></param>
        void Init(Application application);

        /// <summary>
        /// Initialize the current activity with activity and bundle
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="bundle"></param>
        void Init(Activity activity, Bundle bundle);
    }
    /// <summary>
	/// Implementation for Feature
	/// </summary>
	[Preserve(AllMembers = true)]
    public class CurrentActivityImplementation : ICurrentActivity
    {

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public BaseActivity Activity
        {
            get => lifecycleListener?.Activity;
            set
            {
                if (lifecycleListener == null)
                    Init(value, null);
            }
        }

        /// <summary>
		/// Activity state changed event handler
		/// </summary>
        public event EventHandler<ActivityEventArgs> ActivityStateChanged;


        internal void RaiseStateChanged(Activity activity, ActivityEvent ev)
            => ActivityStateChanged?.Invoke(this, new ActivityEventArgs(activity as BaseActivity, ev));


        ActivityLifecycleContextListener lifecycleListener;

        /// <summary>
        /// Gets the current application context
        /// </summary>
        public Context AppContext =>
            Application.Context;

        /// <summary>
        /// Initialize current activity with application
        /// </summary>
        /// <param name="application">The main application</param>
        public void Init(Application application)
        {
            if (lifecycleListener != null)
                return;

            lifecycleListener = new ActivityLifecycleContextListener();
            application.RegisterActivityLifecycleCallbacks(lifecycleListener);
        }

        /// <summary>
        /// Initialize current activity with activity!
        /// </summary>
        /// <param name="activity">The main activity</param>
        /// <param name="bundle">Bundle for activity </param>
        public void Init(Activity activity, Bundle bundle) =>
           Init(activity.Application);
    }

    [Preserve(AllMembers = true)]
    class ActivityLifecycleContextListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        WeakReference<BaseActivity> currentActivity = new WeakReference<BaseActivity>(null);

        public Context Context =>
            Activity ?? Application.Context;

        public BaseActivity Activity =>
            currentActivity.TryGetTarget(out var a) ? a : null;

        CurrentActivityImplementation Current =>
            (CurrentActivityImplementation)(CurrentActivityDelegate.Instance);

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            currentActivity.SetTarget(activity as BaseActivity);
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Created);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Destroyed);
        }

        public void OnActivityPaused(Activity activity)
        {
            currentActivity.SetTarget(null);
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Paused);
        }

        public void OnActivityResumed(Activity activity)
        {
            currentActivity.SetTarget(activity as BaseActivity);
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Resumed);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.SaveInstanceState);
        }

        public void OnActivityStarted(Activity activity)
        {
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Started);
        }

        public void OnActivityStopped(Activity activity)
        {
            Current.RaiseStateChanged(activity as BaseActivity, ActivityEvent.Stopped);
        }
    }
    public class ActivityEventArgs : EventArgs
    {
        internal ActivityEventArgs(BaseActivity activity, ActivityEvent ev)
        {
            Event = ev;
            Activity = activity;
        }

        public ActivityEvent Event { get; }
        public BaseActivity Activity { get; }
    }
    public enum ActivityEvent
    {
        Created,
        Resumed,
        Paused,
        Destroyed,
        SaveInstanceState,
        Started,
        Stopped
    }
}