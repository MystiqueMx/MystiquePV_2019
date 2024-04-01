using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Content.Res;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MystiqueNative.Droid.Fragments
{
    public class BenefitStarsDialogFragment : AppCompatDialogFragment
    {
        public event EventHandler<StarsDialogEventArgs> DialogClosed;
        private Drawable _emptyStarDrawable;
        private Drawable _starDrawable;
        private readonly Dictionary<int, ImageView> _starStatusManager = new Dictionary<int, ImageView>();
        private ImageView _starImageView1;
        private ImageView _starImageView2;
        private ImageView _starImageView3;
        private ImageView _starImageView4;
        private ImageView _starImageView5;
        private int _starCount;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_fragment_benefit_stars, container, false);

            _emptyStarDrawable = AppCompatResources.GetDrawable(Activity, Resource.Drawable.star_outline);
            _starDrawable = AppCompatResources.GetDrawable(Activity, Resource.Drawable.star);

            _starImageView1 = view.FindViewById<ImageView>(Resource.Id.star1);
            _starImageView2 = view.FindViewById<ImageView>(Resource.Id.star2);
            _starImageView3 = view.FindViewById<ImageView>(Resource.Id.star3);
            _starImageView4 = view.FindViewById<ImageView>(Resource.Id.star4);
            _starImageView5 = view.FindViewById<ImageView>(Resource.Id.star5);
            view.FindViewById<Button>(Resource.Id.dialog_close).Click += Cancel_Click; 
            view.FindViewById<Button>(Resource.Id.dialog_choose).Click += Choose_Click;

            _starImageView1.Click += Star1_Click;
            _starImageView2.Click += Star2_Click;
            _starImageView3.Click += Star3_Click;
            _starImageView4.Click += Star4_Click;
            _starImageView5.Click += Star5_Click;

            _starStatusManager.Add(1, _starImageView1);
            _starStatusManager.Add(2, _starImageView2);
            _starStatusManager.Add(3, _starImageView3);
            _starStatusManager.Add(4, _starImageView4);
            _starStatusManager.Add(5, _starImageView5);

            _starCount = 0;

            return view;
        }

        private void Choose_Click(object sender, EventArgs e)
        {
            if (_starCount > 0) // TODO CLEAN UP THIS CAST EMBEDDING A TOAST
            {
                DialogClosed?.Invoke(this, new StarsDialogEventArgs { Stars = _starCount });
                Dismiss();
            }
            else
            {
                (Activity as Helpers.BaseActivity)?.SendToast("Por favor seleccione la calificación");
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogClosed?.Invoke(this, null);
            Dismiss();
        }

        private void Star5_Click(object sender, EventArgs e)
        {
            _starCount = 5;
            UpdateView();
        }

        private void Star4_Click(object sender, EventArgs e)
        {
            _starCount = 4;
            UpdateView();
        }

        private void Star3_Click(object sender, EventArgs e)
        {
            _starCount = 3;
            UpdateView();
        }

        private void Star2_Click(object sender, EventArgs e)
        {
            _starCount = 2;
            UpdateView();
        }

        private void Star1_Click(object sender, EventArgs e)
        {
            _starCount = 1;
            UpdateView();
        }

        private void UpdateView()
        {
            foreach(var s in _starStatusManager.Keys)
                _starStatusManager[s].SetImageDrawable(s > _starCount ? _emptyStarDrawable : _starDrawable);
        }
    }
    public class StarsDialogEventArgs
    {
        public int Stars { get; set; }
    }
}