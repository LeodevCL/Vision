using System.Globalization;
using System.IO;
using System.Windows;

namespace Vision
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FileInfo TracedFile;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                //si es jpg, png o compatible, si pesa más de 0 kb, si EXISTE y si no produce excepcion..
                TracedFile = new FileInfo(e.Args[0]);                
            }            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //Para gestionar si puedo o no ser multi-instancia, guardar un bool en el registro y cuando se abra
            //el software lea ese valor, si es TRUE entonces ejecuta el Make, sino lo salte

            //if(true)
            //{
            //    WpfSingleInstance.Make();
            //}

            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-CL");
            if (SettingsManager.Load("InstanceType") == 1)
            {
                WpfSingleInstance.Make();
            }

            base.OnStartup(e);
        }

        public bool DoHandle { get; set; }
        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            System.Console.WriteLine("Wild exception appeared!!!" + " > " + e.Exception.ToString());
        }
    }
}
