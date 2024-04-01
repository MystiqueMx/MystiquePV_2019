using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace MystiqueNative.Droid.Animations
{
    public class RevealAnimation : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public const string ExtraCircularRevealX = "EXTRA_CIRCULAR_REVEAL_X";
        public const string ExtraCircularRevealY = "EXTRA_CIRCULAR_REVEAL_Y";

        private readonly View _mView;

        private readonly Activity _mActivity;

        private readonly int _revealX;
        private readonly int _revealY;

        public RevealAnimation(View view, Intent intent, Activity activity)
        {
            _mView = view;
            _mActivity = activity;

            var x = intent.HasExtra(ExtraCircularRevealX);
            var y = intent.HasExtra(ExtraCircularRevealY);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop && x && y)
            {
                view.Visibility = ViewStates.Invisible;

                _revealX = intent.GetIntExtra(ExtraCircularRevealX, 0);
                _revealY = intent.GetIntExtra(ExtraCircularRevealY, 0);

                var viewTreeObserver = view.ViewTreeObserver;
                if (viewTreeObserver.IsAlive)
                {
                    viewTreeObserver.AddOnGlobalLayoutListener(this);
                }
            }
            else
            {
                view.Visibility = ViewStates.Visible;
            }
        }
        public void OnGlobalLayout()
        {
            RevealActivity(_revealX, _revealY);
            _mView.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
        }

        public void RevealActivity(int x, int y)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var finalRadius = (float)(Math.Max(_mView.Width, _mView.Height) * 1.1);

                // create the animator for this view (the start radius is zero)
                var circularReveal = ViewAnimationUtils.CreateCircularReveal(_mView, x, y, 0, finalRadius);
                circularReveal.SetDuration(300);
                circularReveal.SetInterpolator(new AccelerateInterpolator());

                // make the view visible and start the animation
                _mView.Visibility = ViewStates.Visible;
                circularReveal.Start();
            }
            else
            {
                _mActivity.Finish();
            }
        }

        public void UnRevealActivity()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                _mActivity.Finish();
            }
            else
            {
                var finalRadius = (float)(Math.Max(_mView.Width, _mView.Height) * 1.1);
                var circularReveal = ViewAnimationUtils.CreateCircularReveal(
                        _mView, _revealX, _revealY, finalRadius, 0);

                circularReveal.SetDuration(300);
                circularReveal.AddListener(new RevealAnimatorAdapter(_mView, _mActivity));
                circularReveal.Start();
            }
        }


    }

    public class RevealAnimatorAdapter : AnimatorListenerAdapter
    {
        private readonly View _mView;

        private readonly Activity _mActivity;
        public RevealAnimatorAdapter(View mView, Activity mActivity)
        {
            this._mView = mView;
            this._mActivity = mActivity;
        }
        public override void OnAnimationEnd(Animator animation)
        {
            _mView.Visibility = ViewStates.Invisible;
            _mActivity.Finish();
            _mActivity.OverridePendingTransition(0, 0);
        }
    }
}