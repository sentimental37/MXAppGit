using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXApp.Views
{

    public class MainViewMenuItem
    {
        public MainViewMenuItem()
        {
            TargetType = typeof(MainViewDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public MenuItemType MenuType { get; set; }
        public Type TargetType { get; set; }
    }
    public enum MenuItemType
    {
        Production,
        Shipping,
        Receiving,
        Inventory,
        Home
    }
}