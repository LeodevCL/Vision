using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Vision
{
    /// <summary>
    /// Lógica de interacción para GetInfoDialog.xaml
    /// </summary>
    public partial class GetInfoDialog : Window
    {
        private Picture _picture;
        public Picture Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }

        public GetInfoDialog(Picture picture) 
        {
            InitializeComponent();
            Picture = picture;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                border.Reset();
                var imagex = new BitmapImage();
                imagex.BeginInit();
                imagex.UriSource = new Uri(picture.Path);
                imagex.EndInit();
                ImageBehavior.SetAnimatedSource(image_main, imagex);

                tx_archivo.Text = picture.Nombre;
                tx_peso.Text = "Peso: " + ToFileSize(picture.Peso);
                tx_res.Text = "Resolución: " + picture.Ancho + "x" + picture.Alto;
                tx_ruta.Text = "Ruta: " + picture.Directorio;
            }));
        } 

        public static string ToFileSize(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                DialogResult = true;
            }
        }

        private void OnShow(object sender, RoutedEventArgs e)
        {
            Magic.OpenAndSelect(Picture.Path);
            DialogResult = true;
        }

    }
}
