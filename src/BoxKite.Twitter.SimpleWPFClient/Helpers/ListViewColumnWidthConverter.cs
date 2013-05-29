// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.CodeDom;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Data;

namespace BoxKite.Twitter.Helpers
{
    public class WidthConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The parent Listview.</param>
        /// <param name="type">The type.</param>
        /// <param name="parameter">
        /// If no parameter is given, the remaning with will be returned.
        /// If the parameter is an integer acts as MinimumWidth, the remaining with will be returned only if it's greater than the parameter
        /// If the parameter is anything else, it's taken to be a percentage. Eg: 0.3* = 30%, 0.15* = 15%
        /// </param>
        /// <param name="culture">The culture.</param>
        /// <returns>The width, as calculated by the parameter given</returns>
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            //NOTE the evil hard-coded number 90 here!

            if (type == typeof(Double))
                return ((double) value) - 90;
            else
            {
                double retValue = Double.Parse(value.ToString()) - 90;
                return retValue;
           }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
