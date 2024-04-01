using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using MystiqueNative.Droid.Helpers;

namespace MystiqueNative.Droid.Views
{
    public class BadgeDrawable : Drawable
    {
        private readonly float _mTextSize;
        private readonly Paint _mBadgePaint;
        private readonly Paint _mBadgePaint1;
        private readonly Paint _mTextPaint;
        private readonly Rect _mTxtRect = new Rect();

        private string _mCount = "";
        private bool _mWillDraw = false;

        public BadgeDrawable(Context context)
        {
            _mTextSize = context.Resources.GetDimension(Resource.Dimension.abc_action_bar_content_inset_material);
            _mBadgePaint = new Paint
            {
                Color = Color.Red,
                AntiAlias = true
            };
            _mBadgePaint.SetStyle(Paint.Style.Fill);

            _mBadgePaint1 = new Paint
            {
                Color = Color.Red,
                AntiAlias = true
            };
            _mBadgePaint1.SetStyle(Paint.Style.Fill);

            _mTextPaint = new Paint
            {
                Color = Color.ParseColor("#EEEEEE"),
                TextSize = _mTextSize,
                AntiAlias = true,
                TextAlign = Paint.Align.Center
            };
            _mTextPaint.SetTypeface(Typeface.DefaultBold);
        }

        public override int Opacity => (int) Format.Unknown;

        public override void Draw(Canvas canvas)
        {
            if (!_mWillDraw)
            {
                return;
            }
            var bounds = Bounds;
            float width = bounds.Right - bounds.Left;
            float height = bounds.Bottom - bounds.Top;
            // Position the badge in the top-right quadrant of the icon.

            /*Using Math.max rather than Math.min */
            //        float radius = ((Math.max(width, height) / 2)) / 2;
            var radius = width * 0.25f;
            var centerX = (width - radius - 1) + 15;
            var centerY = radius - 10;
            if (_mCount.Length <= 2)
            {
                // Draw badge circle.
                canvas.DrawCircle(centerX, centerY, radius + 9, _mBadgePaint1);
                canvas.DrawCircle(centerX, centerY, radius + 7, _mBadgePaint);
            }
            else
            {
                canvas.DrawCircle(centerX, centerY, radius + 10, _mBadgePaint1);
                canvas.DrawCircle(centerX, centerY, radius + 8, _mBadgePaint);
            }
            // Draw badge count text inside the circle.
            _mTextPaint.GetTextBounds(_mCount, 0, _mCount.Length, _mTxtRect);
            float textHeight = _mTxtRect.Bottom - _mTxtRect.Top;
            var textY = centerY + (textHeight / 2f);
            canvas.DrawText(_mCount.Length > 2 ? "99+" : _mCount, centerX, textY, _mTextPaint);
        }
        public void SetCount(int count)
        {
            _mCount = count.ToString();
            _mWillDraw = !count.Equals(0);
            InvalidateSelf();
        }

        public override void SetAlpha(int alpha)
        {
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
        }
    }
}