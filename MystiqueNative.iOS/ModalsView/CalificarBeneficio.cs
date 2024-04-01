using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class CalificarBeneficio : UIViewController
    {

        int Calificacion;
        public string IdBeneficio { get; set; }
        public string IdSucursal { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);
            star1.Image = UIImage.FromBundle("ic_emptyStar");
            star2.Image = UIImage.FromBundle("ic_emptyStar");
            star3.Image = UIImage.FromBundle("ic_emptyStar");
            star4.Image = UIImage.FromBundle("ic_emptyStar");
            star5.Image = UIImage.FromBundle("ic_emptyStar");

            star1.AddGestureRecognizer(new UITapGestureRecognizer(() => ActualizarCalicacion(1)));
            star2.AddGestureRecognizer(new UITapGestureRecognizer(() => ActualizarCalicacion(2)));
            star3.AddGestureRecognizer(new UITapGestureRecognizer(() => ActualizarCalicacion(3)));
            star4.AddGestureRecognizer(new UITapGestureRecognizer(() => ActualizarCalicacion(4)));
            star5.AddGestureRecognizer(new UITapGestureRecognizer(() => ActualizarCalicacion(5)));

        }

        public void ActualizarCalicacion(int calificacion)
        {


            switch (calificacion)
            {
                case 1:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_emptyStar");
                    star3.Image = UIImage.FromBundle("ic_emptyStar");
                    star4.Image = UIImage.FromBundle("ic_emptyStar");
                    star5.Image = UIImage.FromBundle("ic_emptyStar");
                    Calificacion = 1;
                    break;
                case 2:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_star");
                    star3.Image = UIImage.FromBundle("ic_emptyStar");
                    star4.Image = UIImage.FromBundle("ic_emptyStar");
                    star5.Image = UIImage.FromBundle("ic_emptyStar");
                    Calificacion = 2;
                    break;
                case 3:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_star");
                    star3.Image = UIImage.FromBundle("ic_star");
                    star4.Image = UIImage.FromBundle("ic_emptyStar");
                    star5.Image = UIImage.FromBundle("ic_emptyStar");
                    Calificacion = 3;
                    break;
                case 4:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_star");
                    star3.Image = UIImage.FromBundle("ic_star");
                    star4.Image = UIImage.FromBundle("ic_star");
                    star5.Image = UIImage.FromBundle("ic_emptyStar");
                    Calificacion = 4;
                    break;
                case 5:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_star");
                    star3.Image = UIImage.FromBundle("ic_star");
                    star4.Image = UIImage.FromBundle("ic_star");
                    star5.Image = UIImage.FromBundle("ic_star");
                    Calificacion = 5;
                    break;

                default:
                    star1.Image = UIImage.FromBundle("ic_star");
                    star2.Image = UIImage.FromBundle("ic_star");
                    star3.Image = UIImage.FromBundle("ic_star");
                    star4.Image = UIImage.FromBundle("ic_star");
                    star5.Image = UIImage.FromBundle("ic_star");
                    Calificacion = 0;
                    break;
            }
        }
        public CalificarBeneficio(IntPtr handle) : base(handle)
        {
        }

        partial void CancelarCalificar_TouchUpInside(UIButton sender)
        {
            DismissModalViewController(true);
        }

        partial void CalificarButton_TouchUpInside(UIButton sender)
        {
            AppDelegate.Beneficios.InsertarCalificacionBeneficio(IdBeneficio, Calificacion);
            DismissViewController(true, () =>
            {
                AppDelegate.Beneficios.ObtenerDetalleBeneficios(IdBeneficio, IdSucursal);
            });

        }
    }
}