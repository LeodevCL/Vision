using Microsoft.Win32;
using System;
using System.Windows;

namespace Vision
{
    public static class SettingsManager
    {
        public static void Save(string key, int value) 
        {
            try
            {
                RegistryKey rkey = Registry.CurrentUser.CreateSubKey("Vision");
                rkey.SetValue(key, value);
                rkey.Close();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No tiene los permisos suficientes para esto.\nEjecute el programa como Administrador.", "Seguridad de Windows", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static int Load(string key) 
        {
            try
            {
                RegistryKey rkey = Registry.CurrentUser.CreateSubKey("Vision");
                object value = rkey.GetValue(key);
                rkey.Close();
                return value != null? (int)value : 0;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No tiene los permisos suficientes para esto.\nEjecute el programa como Administrador.", "Seguridad de Windows", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return 0;
        }
    }
}
