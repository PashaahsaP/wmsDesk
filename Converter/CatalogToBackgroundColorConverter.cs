using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WmsDesk
{
    public class CatalogToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var greenColor = Color.FromArgb(255, 90, 193, 104);
            var yellowColor = Color.FromArgb(180, 230, 241, 42);
            var redColor = Color.FromArgb(255, 245, 6, 57);
            if (value is OrderItem catalog)
            {
                if(catalog.id != null && catalog.name != "")
                    return new SolidColorBrush(greenColor);
                if(catalog.id != null && catalog.name == "")
                    return new SolidColorBrush(yellowColor);
            }
            return new SolidColorBrush(redColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
