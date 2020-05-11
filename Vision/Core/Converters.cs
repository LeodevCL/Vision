using System;
using System.Windows;
using System.Windows.Data;

namespace Vision
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }

    public class BooleanToListOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return (double)0.25;
            }
            return (double)1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

    public class BooleanToVisibilityInverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RealAngleToStandardAngle : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                if ((double)value > -360 && (double)value < 360) // angulo normal
                {
                    return (double)value;
                }

                return (double)value % 360;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }

    public class ImageToResolution : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Picture)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(((Picture)value).Path);
                return img.Width + "x" + img.Height + " pixeles";
            }
            return "Resolución no disponible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }


    //public class ImageConverter : IValueConverter
    //{
    //    public object Convert(
    //        object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        return new System.Windows.Media.Imaging.BitmapImage(new Uri(value.ToString()));
    //    }

    //    public object ConvertBack(
    //        object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}
}
