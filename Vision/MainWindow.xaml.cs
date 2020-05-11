using System;
using System.Windows;
using System.Windows.Input;
using Vision.ViewModels;

namespace Vision
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel;
        public MainWindow()
        {
            //Información adicional: Se debe establecer la propiedad 'UriSource' o la propiedad 'StreamSource'.
            InitializeComponent();
            try
            {
                ViewModel = (MainViewModel)FindResource("MainVM");

                if (App.TracedFile != null && App.TracedFile.Exists)
                {
                    ViewModel.IniciarCarga(App.TracedFile.FullName);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.ToString(), "Advertencia de inicio", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (ViewModel.PresentationRunning)
                    {
                        ViewModel.DetenerPresentacion();
                    }
                    else
                    {
                        this.Close();
                    }
                    break;
                case Key.LeftCtrl:
                    ViewModel.CtrlVision = !ViewModel.CtrlVision;
                    break;
                case Key.Left:
                    ViewModel.MoverAnterior();
                    break;
                case Key.Right:
                    ViewModel.MoverSiguiente();
                    break;
                case Key.Up:
                    ViewModel.ZoomIn();
                    break;
                case Key.Down:
                    ViewModel.ZoomOut();
                    break;
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
