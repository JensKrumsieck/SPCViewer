using System;
using System.Globalization;
using System.Windows.Data;

namespace SPCViewer.WPF
{
    public class MultiBindingConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts value into array of objects
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
            values.Clone();

        /// <summary>
        /// no need to do that
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
