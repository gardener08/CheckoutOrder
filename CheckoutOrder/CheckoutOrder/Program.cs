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
            {"Tomato Soup", new StockItem { ItemName="Tomato Soup", UnitPrice = .42, Markdown = 0, PriceCategory = "eaches"}},
            {"Bananas", new StockItem { ItemName = "Bananas", UnitPrice = .45, Markdown = 0, PriceCategory = "byWeight"}}
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
            QuantityDiscount qtyDiscount = itemToScan.QtyDiscount;
            if (qtyDiscount != null)
            {
                bool discountEligible = itemToScan.NumberOfThisItemInCart >= qtyDiscount.QuantityToGetDiscount;
                bool discountExceeded =
                    itemToScan.NumberOfThisItemInCart >=
                    (qtyDiscount.QuantityToGetDiscount + qtyDiscount.QuantityUnderDiscount);
                if (discountEligible && !discountExceeded)
                {
                    double priceWithDiscount = itemToScan.UnitPrice - (itemToScan.UnitPrice*qtyDiscount.Discount);
                    TotalGroceryBill += priceWithDiscount;
                }
                else
                {
                    TotalGroceryBill += itemToScan.UnitPrice;
                }
            }
            else
            {
                double priceWithMarkdown = itemToScan.UnitPrice - itemToScan.Markdown;
                TotalGroceryBill = TotalGroceryBill + priceWithMarkdown;
            }
            itemToScan.NumberOfThisItemInCart++;
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

        public void ApplyQuantityDiscount(string itemName, int quantityToGetDiscount, int quantityUnderDiscount, double discount)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            itemToScan.QtyDiscount = new QuantityDiscount()
            {
                QuantityToGetDiscount = quantityToGetDiscount,
                QuantityUnderDiscount = quantityUnderDiscount,
                Discount = discount
            };
        }
    }

    public class StockItem
    {
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public double Markdown { get; set; }
        public string PriceCategory { get; set; }
        public QuantityDiscount QtyDiscount { get; set; }
        public int NumberOfThisItemInCart { get; set; }
    }

    public class QuantityDiscount
    {
        public int QuantityToGetDiscount { get; set; }
        public int QuantityUnderDiscount { get; set; }
        public double Discount { get; set; }
    }

}
