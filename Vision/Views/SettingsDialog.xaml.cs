using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vision.Views
{
    /// <summary>
    /// Lógica de interacción para SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
            check_instance.IsChecked = SettingsManager.Load("InstanceType") == 1 ? true : false;
            check_hidden.IsChecked = SettingsManager.Load("IgnoreHidden") == 1 ? true : false;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            int value1 = check_instance.IsChecked.Value ? 1 : 0;
            SettingsManager.Save("InstanceType", value1);

            int value2 = check_hidden.IsChecked.Value ? 1 : 0;
            SettingsManager.Save("IgnoreHidden", value2);

            DialogResult = true;
        }

        private void OnCheck(object sender, RoutedEventArgs e)
        {
            //int value = (((CheckBox)sender).IsChecked.Value) ? 1 : 0;
            //SettingsManager.Save("InstanceType", value);
        }

    }
}
