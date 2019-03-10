using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            {"Oranges", new StockItem { ItemName = "Oranges", UnitPrice = 1.00, Markdown = 0, PriceCategory = "byWeight"}},
            {"Wheaties", new StockItem { ItemName= "Wheaties", UnitPrice = 3.25, Markdown = 0, PriceCategory = "eaches"}},        };

        public IDictionary<string, IList<ShoppingCartItem>> ShoppingCart { get; }

        public Program()
        {
            TotalGroceryBill = 0;
            ShoppingCart = new Dictionary<string, IList<ShoppingCartItem>>()
            {
                {"Tomato Soup", new List<ShoppingCartItem>()},
                {"Bananas", new List<ShoppingCartItem>()},
                {"Oranges", new List<ShoppingCartItem>()},
                {"Wheaties", new List<ShoppingCartItem>()}
            };
        }
        static void Main(string[] args)
        {
            Program programToRun = new Program();
            programToRun.ScanItem("Tomato Soup");
        }

        public void ScanItem(string itemName)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            if (QuantityDiscountValid(itemToScan))
            {
                AddScannedEachesItemWithQuantityDiscount(itemToScan, itemName);
            }
            else if (itemToScan.Markdown > 0)
            {
                AddScannedEachesItemWithMarkdown(itemToScan, itemName);
            }
            else
            {
                AddScannedEachesItem(itemToScan, itemName);
            }
            itemToScan.NumberOfThisItemInCart++;
            if (GroupingDiscountValid(itemToScan))
            {
                ApplyGroupingDiscountForEachesItemAtSale(itemName);
            }
            ComputeTotalBill();
        }

        private void AddScannedEachesItem(StockItem itemToAdd, string itemName)
        {
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = itemToAdd.UnitPrice,
                ItemPrice = itemToAdd.UnitPrice,
            };
            ShoppingCart[itemName].Add(currentItemBeingScanned);
        }

        private void AddScannedEachesItemWithQuantityDiscount(StockItem itemToAdd, string itemName)
        {
            QuantityDiscount qtyDiscount = itemToAdd.QtyDiscount;
            double priceWithDiscount = itemToAdd.UnitPrice - (itemToAdd.UnitPrice * qtyDiscount.Discount);
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = priceWithDiscount,
                ItemPrice = priceWithDiscount
            };
            ShoppingCart[itemName].Add(currentItemBeingScanned);
        }

        private void AddScannedEachesItemWithMarkdown(StockItem itemToAdd, string itemName)
        {
            double priceWithMarkdown = itemToAdd.UnitPrice - itemToAdd.Markdown;
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = priceWithMarkdown,
                ItemPrice = priceWithMarkdown
            };
            ShoppingCart[itemName].Add(currentItemBeingScanned);
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
                    AddScannedWeighedItemWithQuantityDiscount(itemToScan, itemName, itemWeight);
                }
                else
                {
                    AddScannedWeighedItem(itemToScan, itemName, itemWeight);
                }
                itemToScan.NumberOfThisItemInCart++;
            }
            ComputeTotalBill();
        }

        private void AddScannedWeighedItemWithQuantityDiscount(StockItem itemToAdd, string itemName, double itemWeight)
        {
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = itemToAdd.UnitPrice,
                ItemPrice = itemToAdd.UnitPrice * itemWeight,
                ItemWeight = itemWeight
            };
            ShoppingCart[itemName].Add(currentItemBeingScanned);
            ApplyQuantityDiscountForWeighedItemAtSale(itemName);
        }

        private void AddScannedWeighedItem(StockItem itemToAdd, string itemName, double itemWeight)
        {
            double unitPrice = itemToAdd.UnitPrice - itemToAdd.Markdown;
            double itemPrice = unitPrice * itemWeight;
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = unitPrice,
                ItemPrice = itemPrice,
                ItemWeight = itemWeight
            };
            ShoppingCart[itemName].Add(currentItemBeingScanned);
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

        private void ApplyGroupingDiscountForEachesItemAtSale(string itemName)
        {
            List<ShoppingCartItem> shoppingCartItemsOfThisType = (List<ShoppingCartItem>)(ShoppingCart[itemName]);
            StockItem inventoryItemOfThisType = ItemsAvailableForSale[itemName];
            GroupDiscount discountForThisItemType = inventoryItemOfThisType.GrpDiscount;

            if (shoppingCartItemsOfThisType.Count >= discountForThisItemType.QuantityToGetDiscount)
            {
                double pricePerItem = discountForThisItemType.PriceForGroup /
                                      discountForThisItemType.QuantityToGetDiscount;

                int numberOfItemsToGetDiscount = inventoryItemOfThisType.GrpDiscount.QuantityToGetDiscount;
                for (int i = 0; i < numberOfItemsToGetDiscount; i++)
                {
                    ShoppingCartItem itemToApplyDiscountTo = shoppingCartItemsOfThisType.ElementAt(i);
                    itemToApplyDiscountTo.UnitPrice = pricePerItem;
                    itemToApplyDiscountTo.ItemPrice = pricePerItem;
                }
            }
        }

        private void ApplyQuantityDiscountForWeighedItemAtSale(string itemName)
        {
            List<ShoppingCartItem> shoppingCartItemsOfThisType = (List<ShoppingCartItem>)(ShoppingCart[itemName]);
            shoppingCartItemsOfThisType.Sort();

            StockItem inventoryItemOfThisType = ItemsAvailableForSale[itemName];
            QuantityDiscount qtyDiscount = inventoryItemOfThisType.QtyDiscount;
            double unitPriceWithDiscount = inventoryItemOfThisType.UnitPrice - (inventoryItemOfThisType.UnitPrice * qtyDiscount.Discount);

            int firstItemToGetDiscountByPosition = qtyDiscount.QuantityToGetDiscount;
            int lastItemToGetDiscountByPosition = firstItemToGetDiscountByPosition + (qtyDiscount.QuantityUnderDiscount - 1);
            for (int currentItemIndex = firstItemToGetDiscountByPosition; currentItemIndex <= lastItemToGetDiscountByPosition; currentItemIndex++)
            {
                ShoppingCartItem itemToApplyDiscountTo = shoppingCartItemsOfThisType.ElementAt(currentItemIndex);
                double itemPrice = unitPriceWithDiscount * itemToApplyDiscountTo.ItemWeight;
                itemToApplyDiscountTo.UnitPrice = unitPriceWithDiscount;
                itemToApplyDiscountTo.ItemPrice = itemPrice;
            }
        }

        public double ComputeTotalBill()
        {
            double totalBill = 0;
            foreach (string key in ShoppingCart.Keys)
            {
                IList<ShoppingCartItem> cartItemsWithThisDescription = ShoppingCart[key];
                foreach (ShoppingCartItem item in cartItemsWithThisDescription)
                {
                    totalBill += item.ItemPrice;
                }
            }
            this.TotalGroceryBill = totalBill;
            return totalBill;
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

    public class ShoppingCartItem : IComparable<ShoppingCartItem>
    {
        public double UnitPrice { get; set; }
        public double ItemPrice { get; set; }
        public double ItemWeight { get; set; }

        // Sort in descending order
        public int CompareTo(ShoppingCartItem itemToCompare)
        {
            return itemToCompare.ItemPrice.CompareTo(this.ItemPrice);
        }
    }
}
