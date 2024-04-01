using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.Transitions;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;

namespace MystiqueNative.Droid.Activities
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Label = "Ordena y Recoge")]
    public class MenuRestauranteActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_menu_restaurante;
        #region VARIABLES

        private bool _hasPedido;
        #endregion
        #region FRAGMENTS

        private CategoriasMenuFragment _categoriasMenu;
        #endregion
        #region VIEWS

        private FrameLayout _contentLayout;
        private FrameLayout _progressBarHolder;
        private TextView _labelTotal;
        private FloatingActionButton _fab;
        #endregion
        #region LIFECYCLE
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            _categoriasMenu = new CategoriasMenuFragment();
            _categoriasMenu.OnCategoriaSelected += CategoriasMenu_OnCategoriaSelected;
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.menu_restaurante_content, _categoriasMenu)
                .Commit();
        }

        private void CategoriasMenu_OnCategoriaSelected(object sender, EventArgs e)
        {
            PerformTransitionToFragment();
            //var productosMenu = new ProductosMenuFragment();
            //SupportFragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.menu_restaurante_content, productosMenu)
            //    .AddToBackStack("Productos_" + e.ToString())
            //    .Commit();
        }

        private void PerformTransitionToFragment()
        {
            if (IsDestroyed)
            {
                return;
            }
            var previousFragment = SupportFragmentManager.FindFragmentById(Resource.Id.menu_restaurante_content);
            var nextFragment = new ProductosMenuFragment();

            var tx = SupportFragmentManager.BeginTransaction();

            // 1. Exit for Previous Fragment
            Fade fadeOut = new Fade();
            fadeOut.SetDuration(150);
            previousFragment.ExitTransition = fadeOut;

            // 2. Shared Elements Transition
            //TransitionSet enterTransitionSet = new TransitionSet();
            //enterTransitionSet.addTransition(TransitionInflater.from(this).inflateTransition(android.R.transition.move));
            //enterTransitionSet.setDuration(MOVE_DEFAULT_TIME);
            //enterTransitionSet.setStartDelay(FADE_DEFAULT_TIME);
            //nextFragment.setSharedElementEnterTransition(enterTransitionSet);

            // 3. Enter Transition for New Fragment
            var slideIn = new Slide
            {
                SlideEdge = (int)GravityFlags.End
            };
            slideIn.SetDuration(250);
            nextFragment.EnterTransition = slideIn;

            //View logo = ButterKnife.findById(this, R.id.fragment1_logo);
            //fragmentTransaction.addSharedElement(logo, logo.getTransitionName());
            tx.Replace(Resource.Id.menu_restaurante_content, nextFragment);
            tx.AddToBackStack(null);
            tx.CommitAllowingStateLoss();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!_hasPedido)
            {
                SetUpInitialView();
            }
        }

        #endregion
        #region OVERRIDES
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home) OnBackPressed();

            return true;
        }
        #endregion
        #region FIND VIEWS
        private void GrabViews()
        {
            _contentLayout = FindViewById<FrameLayout>(Resource.Id.menu_restaurante_content);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _labelTotal = FindViewById<TextView>(Resource.Id.menu_restaurante_header_count);
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
        }
        #endregion
        #region LOADING
        private void StartAnimatingLoading()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLoading()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion
        private void SetUpInitialView()
        {
            _labelTotal.Text = "$0.00";
            _fab.Visibility = ViewStates.Gone;
        }

    }
}