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
            {"Tomato Soup", new StockItem("Tomato Soup", .42, 0, "eaches")},
            {"Bananas", new StockItem("Bananas", .45, 0, "byWeight")}
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
            double priceWithMarkdown = itemToScan.UnitPrice - itemToScan.Markdown;
            TotalGroceryBill = TotalGroceryBill + priceWithMarkdown;
        }

        public void ScanItem(string itemName, double itemWeight)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            if (itemToScan.PriceCategory == "eaches")
            {
                ScanItem(itemName);
            }
            else if (itemToScan.PriceCategory == "byWeight")
            {
                double itemPrice = (itemToScan.UnitPrice - itemToScan.Markdown) * itemWeight;
                TotalGroceryBill += itemPrice;
            }
        }

        public void MarkDownItem(string itemName, double markdown)
        {
            ItemsAvailableForSale[itemName].Markdown = markdown;
        }
    }

    public class StockItem
    {
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public double Markdown { get; set; }
        public string PriceCategory { get; set; }

        public StockItem(string itemName, double unitPrice, double markdown, string priceCategory)
        {
            ItemName = itemName;
            UnitPrice = unitPrice;
            Markdown = Markdown;
            PriceCategory = priceCategory;
        }
    }

}
