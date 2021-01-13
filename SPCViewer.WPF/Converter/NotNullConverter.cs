using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SPCViewer.WPF.Converter
{
    public class NotNullConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
