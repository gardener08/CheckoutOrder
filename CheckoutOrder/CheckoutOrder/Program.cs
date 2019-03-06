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

        public readonly IDictionary<string,CheckoutItem> ItemsAvailableForSale = new Dictionary<string, CheckoutItem>
        {
            {
                "Tomato Soup", new CheckoutItem("Tomato Soup", .42)
            }

        };

        public Program()
        {
            TotalGroceryBill = 0;
        }
        static void Main(string[] args)
        {
            Program programToRun = new Program();
            CheckoutItem itemToScan = new CheckoutItem("Test", 0);
            programToRun.ScanItem(itemToScan);
        }

        public void ScanItem(CheckoutItem itemToScan)
        {
            TotalGroceryBill = TotalGroceryBill + itemToScan.CurrentPrice;
        }

        public void MarkDownItem(string itemName, double newPrice)
        {
            ItemsAvailableForSale[itemName].CurrentPrice = newPrice;
        }
    }

    public class CheckoutItem
    {
        public string ItemName { get; set; }
        public double CurrentPrice { get; set; }

        public CheckoutItem(string itemName, double currentPrice)
        {
            ItemName = itemName;
            CurrentPrice = currentPrice;
        }
    }

}
