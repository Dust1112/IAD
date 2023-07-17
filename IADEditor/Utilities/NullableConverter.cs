using System;
using System.Globalization;
using System.Windows.Data;

namespace IADEditor.Utilities
{
    public class NullableConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return Activator.CreateInstance(targetType); }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return null; }
            return value;
        }
    }
}