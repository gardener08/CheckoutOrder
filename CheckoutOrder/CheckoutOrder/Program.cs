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
            {"Bananas", new StockItem { ItemName = "Bananas", UnitPrice = .45, Markdown = 0, PriceCategory = "byWeight"}},
            {"Oranges", new StockItem { ItemName = "Oranges", UnitPrice = 1.00, Markdown = 0, PriceCategory = "byWeight"}}
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

        // TODO: Refactor ScanItem methods.  They are getting too complex.
        public void ScanItem(string itemName)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            if (QuantityDiscountValid(itemToScan))
            {
                QuantityDiscount qtyDiscount = itemToScan.QtyDiscount;
                double priceWithDiscount = itemToScan.UnitPrice - (itemToScan.UnitPrice * qtyDiscount.Discount);
                TotalGroceryBill += priceWithDiscount;
            }
            else if (itemToScan.Markdown > 0)
            {
                double priceWithMarkdown = itemToScan.UnitPrice - itemToScan.Markdown;
                TotalGroceryBill = TotalGroceryBill + priceWithMarkdown;
            }
            else
            {
                TotalGroceryBill += itemToScan.UnitPrice;
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
                if (QuantityDiscountValid(itemToScan))
                {
                    QuantityDiscount qtyDiscount = itemToScan.QtyDiscount;
                    double unitPriceWithDiscount = itemToScan.UnitPrice - (itemToScan.UnitPrice * qtyDiscount.Discount);
                    double itemPrice = unitPriceWithDiscount * itemWeight;
                    TotalGroceryBill += itemPrice;
                }
                else
                {
                    double itemPrice = (itemToScan.UnitPrice - itemToScan.Markdown) * itemWeight;
                    TotalGroceryBill += itemPrice;
                }
                itemToScan.NumberOfThisItemInCart++;
            }
        }

        public void MarkDownItem(string itemName, double markdown)
        {
            ItemsAvailableForSale[itemName].Markdown = markdown;
        }

        public bool QuantityDiscountValid(StockItem item)
        {
            QuantityDiscount qtyDiscount = item.QtyDiscount;
            if (qtyDiscount != null)
            {
                bool discountEligible = item.NumberOfThisItemInCart >= qtyDiscount.QuantityToGetDiscount;
                bool discountExceeded =
                    item.NumberOfThisItemInCart >=
                    (qtyDiscount.QuantityToGetDiscount + qtyDiscount.QuantityUnderDiscount);
                if (discountEligible && !discountExceeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool GroupingDiscountValid(StockItem item)
        {
            GroupDiscount grpDiscount = item.GrpDiscount;
            if (grpDiscount != null)
            {
                bool discountEligible = (item.NumberOfThisItemInCart == grpDiscount.QuantityToGetDiscount);
                if (discountEligible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public void ApplyGroupingDiscountToItemForSale(string itemName, int quantityToGetDiscount, double priceForGroup)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                PriceForGroup = priceForGroup,
                QuantityToGetDiscount = quantityToGetDiscount
            };
            itemToScan.GrpDiscount = grpDiscount;
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
        public GroupDiscount GrpDiscount { get; set; }
        public int NumberOfThisItemInCart { get; set; }
    }

    public class QuantityDiscount
    {
        public int QuantityToGetDiscount { get; set; }
        public int QuantityUnderDiscount { get; set; }
        public double Discount { get; set; }
    }

    public class GroupDiscount
    {
        public int QuantityToGetDiscount { get; set; }
        public double PriceForGroup { get; set; }
    }

}
