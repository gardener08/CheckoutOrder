using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class Program
    {
        public double TotalGroceryBill { get; set; }

        public readonly IDictionary<string,StockItem> ItemsAvailableForSale = new Dictionary<string, StockItem>
        {
            {
                "Tomato Soup", new StockItem("Tomato Soup", .42, "eaches")
            }

        };

        public Program()
        {
            TotalGroceryBill = 0;
        }
        static void Main(string[] args)
        {
            Program programToRun = new Program();
            programToRun.ScanItem("Tomato Soup");
        }

        public void ScanItem(string itemName)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            TotalGroceryBill = TotalGroceryBill + itemToScan.UnitPrice;
        }

        public void MarkDownItem(string itemName, double newPrice)
        {
            ItemsAvailableForSale[itemName].UnitPrice = newPrice;
        }
    }

    public class StockItem
    {
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public string PriceCategory { get; set; }

        public StockItem(string itemName, double unitPrice, string priceCategory)
        {
            ItemName = itemName;
            UnitPrice = UnitPrice;
            PriceCategory = priceCategory;
        }
    }

}
