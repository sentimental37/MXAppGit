using MXApp.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MXApp.Converters
{
    public class MenuTypeConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var menuItemType = (MenuItemType)value;

            switch (menuItemType)
            {
                case MenuItemType.Home:
                    return "ic_home.png";
                case MenuItemType.Production:
                    return "ic_production.png";
                case MenuItemType.Shipping:
                    return "ic_shipping.png";
                case MenuItemType.Inventory:
                    return "ic_inventory.png";
                case MenuItemType.Receiving:
                    return "ic_receiving.png";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
