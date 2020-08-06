using System.Windows;

namespace Vision.Views
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
            check_instance.IsChecked = SettingsManager.Load("InstanceType") == 1 ? true : false;
            check_hidden.IsChecked = SettingsManager.Load("IgnoreHidden") == 1 ? true : false;
            check_selection.IsChecked = SettingsManager.Load("OnlySelected") == 1 ? true : false;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            int value1 = check_instance.IsChecked.Value ? 1 : 0;
            SettingsManager.Save("InstanceType", value1);

            int value2 = check_hidden.IsChecked.Value ? 1 : 0;
            SettingsManager.Save("IgnoreHidden", value2);

            int value3 = check_selection.IsChecked.Value ? 1 : 0;
            SettingsManager.Save("OnlySelected", value3);

            DialogResult = true;
        }
    }
}
