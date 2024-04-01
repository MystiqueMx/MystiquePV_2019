using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Views
{
    public sealed class VerticalTextView : TextView
    {
        private readonly bool _topDown;
        private IAttributeSet _attrs;
        public VerticalTextView(Context ctx, IAttributeSet attrs): base(ctx, attrs)
        {
            this._attrs = attrs;
            if (Android.Views.Gravity.IsVertical(Gravity) && (Gravity & GravityFlags.VerticalGravityMask) == GravityFlags.Bottom)
            {
                Gravity = (Gravity & GravityFlags.HorizontalGravityMask | GravityFlags.Top);
                _topDown = false;
            }
            else
                _topDown = true;
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(MeasuredHeight, MeasuredWidth);
        }
        protected override void OnDraw(Canvas canvas)
        {
            Paint.Color = new Color(CurrentTextColor);
            Paint.DrawableState = GetDrawableState();
            canvas.Save();
            if (_topDown)
            {
                canvas.Translate(Width, 0);
                canvas.Rotate(90);
            }
            else
            {
                canvas.Translate(0, Height);
                canvas.Rotate(-90);
            }
            canvas.Translate(CompoundPaddingLeft, CompoundPaddingTop);

            Layout.Draw(canvas);
            canvas.Restore();
        }
    }
}