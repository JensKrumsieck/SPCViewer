using SPCViewer.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SPCViewer.WPF
{
    public class UIActionToIntConverter : IValueConverter
    {
        ///<inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (int)(UIAction)value;

        ///<inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (UIAction)(value != null && (int)value != -1 ? (int)value : 0);
    }
}
