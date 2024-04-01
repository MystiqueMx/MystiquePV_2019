using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BarronWellnessMovil.Droid.Helpers
{
    /// <summary>
    /// <para> Adapter y event args para recycler views </para>
    /// </summary>
    public class BaseRecyclerViewAdapter : RecyclerView.Adapter
    {

        public event EventHandler<RecyclerClickEventArgs> ItemClick;
        public event EventHandler<RecyclerClickEventArgs> ItemLongClick;
        public event EventHandler<RecyclerClickEventArgs> ItemClick2;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            throw new NotImplementedException();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            throw new NotImplementedException();
        }

        public override int ItemCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected void OnClick(RecyclerClickEventArgs args) => ItemClick?.Invoke(this, args);
        protected void OnLongClick(RecyclerClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        protected void OnClick2(RecyclerClickEventArgs args) => ItemClick2?.Invoke(this, args);
    }
    public class RecyclerClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}