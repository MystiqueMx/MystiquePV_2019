using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Views
{
    public class SquareFrameLayout : FrameLayout, ViewTreeObserver.IOnPreDrawListener
    {
        #region CTOR
        public SquareFrameLayout(Context context) : base(context) { }
        public SquareFrameLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public SquareFrameLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
        public SquareFrameLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }
        protected SquareFrameLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        #endregion
        #region ViewTreeObserver.IOnPreDrawListener
        public bool OnPreDraw()
        {
            if (Width == Height) return true;
            var squareSize = Math.Min(Width, Height);

            var lp = LayoutParameters;
            lp.Width = squareSize;
            lp.Height = squareSize;
            RequestLayout();
            return false;
        }
        #endregion
        #region Overrides
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (IsInEditMode)
            {
                if(Math.Abs(heightMeasureSpec) < Math.Abs(widthMeasureSpec))
                {
                    base.OnMeasure(heightMeasureSpec, heightMeasureSpec);
                }
                else
                {
                    base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
                }

            }

            this.ViewTreeObserver.AddOnPreDrawListener(this);
        }
        #endregion
    }
}