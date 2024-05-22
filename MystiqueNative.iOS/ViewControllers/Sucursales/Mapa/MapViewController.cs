using CoreLocation;
using Foundation;
//using Google.Maps;
using System;
using System.Collections.Generic;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MapViewController : UIViewController
    {
        int sucursal;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
         //  List<Marker> markers = new List<Marker>();

            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);
            mapView.AddGestureRecognizer(g);
            //32.639585, -115.449436
            double latitud = 32.639585;
            double longitud = -115.449436;

            CLLocationCoordinate2D coordenadas = new CLLocationCoordinate2D(latitud, longitud);

           // var camera = CameraPosition.FromCamera(latitude: latitud,
                              //              longitude: longitud,
                                  //          zoom: 12, bearing: 0, viewingAngle: 30);

           // mapView = MapView.FromCamera(new CoreGraphics.CGRect(0, 0, 100, 100), camera);

            mapView.MyLocationEnabled = true;
            

           // var marker = Marker.FromPosition(coordenadas);
            
            //Marker marker2 = Marker.FromPosition();
            //marker.Icon = Marker.MarkerImage(UIColor.Cyan);
            if (AppDelegate.Beneficios.Sucursales.Count > 0)

            {
                foreach (var s in AppDelegate.Beneficios.Sucursales)
                {
                   // Marker markerOpt1 = new Marker();

                    //markerOpt1.Position = (new CLLocationCoordinate2D(Convert.ToDouble(s.Latitud), Convert.ToDouble(s.Longitud)));
                    //markerOpt1.Title = (s.Nombre);
                    //markerOpt1.Icon = UIImage.FromBundle("markerMap");

                    //markerOpt1.Map = mapView;
                 //   markers.Add(markerOpt1);
                   
                }
            }
            else
            {
                AppDelegate.Beneficios.Sucursales.CollectionChanged += Sucursales_CollectionChanged;
            }


            mapView.InfoTapped += (_Mapa, args) => {
                if(args.Marker != null)
                {
                   // sucursal = markers.FindIndex(c => c.Equals(args.Marker));
                    PerformSegue("MARCADOR_SEGUE", this);
                }
            }; 
              

            View = mapView;
       
        }


        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "MARCADOR_SEGUE")
            {

                var controller = segue.DestinationViewController as BeneficiosTableViewController;

                var item = AppDelegate.Beneficios.Sucursales[sucursal];

               // AppDelegate.Beneficios.Beneficios = new System.Collections.ObjectModel.ObservableCollection<Models.BeneficiosSucursal>();

              //  controller.SucursalSeleccionada = item;
            }
        }

        //[Export("mapView:didTapMarker:")]
        //public bool TappedMarker(MapView mapView, Marker marker)
        //{
        //    var id = marker.Title;
        //    Console.WriteLine(id);

        //    return true;
        //}




        //public void OnMapReady(MapView googleMap)
        //{

        //    var _map = googleMap;

        //    if (AppDelegate.Beneficios.Sucursales.Count > 0)

        //    {
        //        foreach (var s in AppDelegate.Beneficios.Sucursales)
        //        {
        //            Marker markerOpt1 = new Marker();

        //            markerOpt1.Position = (new CLLocationCoordinate2D(Convert.ToDouble(s.Latitud), Convert.ToDouble(s.Longitud)));
        //            markerOpt1.Title = (s.Nombre);
        //            markerOpt1.Icon = UIImage.FromBundle("markerMap");

        //            markerOpt1.Map = mapView;
        //        }
        //    }
        //    else {
        //        AppDelegate.Beneficios.Sucursales.CollectionChanged += Sucursales_CollectionChanged; }
        //}

        private void Sucursales_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
         {
        foreach (var s in AppDelegate.Beneficios.Sucursales)
        {
            //Marker markerOpt1 = new Marker(); markerOpt1.Position = (new CLLocationCoordinate2D(Convert.ToDouble(s.Latitud), Convert.ToDouble(s.Longitud)));
            //markerOpt1.Title = (s.Nombre);
            //markerOpt1.Icon = UIImage.FromBundle("markerMap");
            //markerOpt1.Map = mapView;
        }
    }
    public MapViewController(IntPtr handle) : base(handle)
    {
    }

    }
}