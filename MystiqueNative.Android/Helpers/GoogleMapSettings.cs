using Android.Gms.Maps.Model;

namespace MystiqueNative.Droid.Helpers
{
    public class GoogleMapSettings
    {
        public GoogleMapSettings(LatLng marker, int zoom)
        {
            this.DefaultLocation = marker;
            this.DefaultZoom = zoom;
        }

        public LatLng DefaultLocation { get; }
        public int DefaultZoom { get; }
    }
}