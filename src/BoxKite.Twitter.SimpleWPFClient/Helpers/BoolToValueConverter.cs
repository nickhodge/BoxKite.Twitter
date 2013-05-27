using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
#if (WINDOWS_PHONE)
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
#elif(WINDOWSSTORE)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#elif(WINDOWS)

#endif

namespace BoxKite.Twitter.Helpers
{

    // Source: http://geekswithblogs.net/codingbloke/archive/2010/05/28/a-generic-boolean-value-converter.aspx
    // slightly modified for Windows 8 XAML, Windows Phone 8 XAML and WPF4.5. Yes, these are different enough for an #elif barf

    public class BoolToStringConverter : BoolToValueConverter<String> { }
    public class BoolToBrushConverter : BoolToValueConverter<Brush> { }
    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }


#if (WINDOWS_PHONE)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
#elif(WINDOWSSTORE)
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null && value.Equals(TrueValue);
        }
#elif(WINDOWS)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
#endif
    }
}
