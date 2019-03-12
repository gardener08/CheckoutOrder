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

        public IDictionary<string, ShoppingCartItemHolder> ShoppingCart { get; }

        public Program()
        {
            TotalGroceryBill = 0;
            ShoppingCart = new Dictionary<string, ShoppingCartItemHolder>()
            {
                {"Tomato Soup", new ShoppingCartItemHolder()},
                {"Bananas", new ShoppingCartItemHolder()},
                {"Oranges", new ShoppingCartItemHolder()},
                {"Wheaties", new ShoppingCartItemHolder()}
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
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemToScan.ItemName];
            int quantityDiscountsGiven = shoppingCartHolderOfThisItemType.QuantityDiscountsGiven;
            int cartItemsCount = shoppingCartHolderOfThisItemType.CartItems.Count;
            if (QuantityDiscountValid(itemToScan, cartItemsCount, quantityDiscountsGiven))
            {
                ScanEachesItemWithQuantityDiscount(itemToScan, itemName);
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
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemName];
            shoppingCartHolderOfThisItemType.CartItems.Add(currentItemBeingScanned);
        }

        private void ScanEachesItemWithQuantityDiscount(StockItem itemToAdd, string itemName)
        {
            QuantityDiscount qtyDiscount = itemToAdd.QtyDiscount;
            double priceWithDiscount = itemToAdd.UnitPrice - (itemToAdd.UnitPrice * qtyDiscount.Discount);
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = priceWithDiscount,
                ItemPrice = priceWithDiscount
            };

            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemName];
            shoppingCartHolderOfThisItemType.CartItems.Add(currentItemBeingScanned);

            UpdateQuantityDiscountCountIfDiscountWasFullyUsed(itemToAdd);
        }

        private void UpdateQuantityDiscountCountIfDiscountWasFullyUsed(StockItem inventoryItem)
        {
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[inventoryItem.ItemName];
            int quantityDiscountsGiven = shoppingCartHolderOfThisItemType.QuantityDiscountsGiven;
            int updatedCartItemsCount = shoppingCartHolderOfThisItemType.CartItems.Count;
            int itemsToGetAFullDiscount = (quantityDiscountsGiven + 1) *
                   (inventoryItem.QtyDiscount.QuantityUnderDiscount + inventoryItem.QtyDiscount.FullPriceItems);

            if ((updatedCartItemsCount % itemsToGetAFullDiscount) == 0)
            {
                shoppingCartHolderOfThisItemType.QuantityDiscountsGiven++;
            }
        }

        private void AddScannedEachesItemWithMarkdown(StockItem itemToAdd, string itemName)
        {
            double priceWithMarkdown = itemToAdd.UnitPrice - itemToAdd.Markdown;
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = priceWithMarkdown,
                ItemPrice = priceWithMarkdown
            };
            ShoppingCart[itemName].CartItems.Add(currentItemBeingScanned);
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
                AddScannedWeighedItem(itemToScan, itemName, itemWeight);              
                itemToScan.NumberOfThisItemInCart++;
                if (itemToScan.QtyDiscount != null)
                {
                    ApplyQuantityDiscountForWeighedItemAtSale(itemName);
                    UpdateQuantityDiscountCountIfDiscountWasFullyUsed(itemToScan);
                }

            }
            ComputeTotalBill();
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
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemName];
            shoppingCartHolderOfThisItemType.CartItems.Add(currentItemBeingScanned);
        }

        public void MarkDownItem(string itemName, double markdown)
        {
            ItemsAvailableForSale[itemName].Markdown = markdown;
        }

        public bool QuantityDiscountValid(StockItem item, int currentItemPosition, int quantityDiscountsGiven)
        {
            QuantityDiscount qtyDiscount = item.QtyDiscount;
            if (qtyDiscount != null)
            {
                int itemsToGetAFullDiscount = (quantityDiscountsGiven + 1) *
                                       (qtyDiscount.QuantityUnderDiscount + qtyDiscount.FullPriceItems);
                int startDiscountAt = (itemsToGetAFullDiscount - qtyDiscount.QuantityUnderDiscount);

                if ((currentItemPosition < itemsToGetAFullDiscount) && (currentItemPosition >= startDiscountAt))
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

        public void ApplyGroupingDiscountSpecial(string itemName, int quantityToGetDiscount, double priceForGroup)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                PriceForGroup = priceForGroup,
                QuantityToGetDiscount = quantityToGetDiscount
            };
            itemToScan.GrpDiscount = grpDiscount;
        }

        public void ApplyQuantityDiscountSpecial(string itemName, int quantityToGetDiscount, int quantityUnderDiscount, double discount)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            itemToScan.QtyDiscount = new QuantityDiscount()
            {
                FullPriceItems = quantityToGetDiscount,
                QuantityUnderDiscount = quantityUnderDiscount,
                Discount = discount
            };
        }

        private void ApplyGroupingDiscountForEachesItemAtSale(string itemName)
        {
            List<ShoppingCartItem> shoppingCartItemsOfThisType = (List<ShoppingCartItem>)(ShoppingCart[itemName].CartItems);
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
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemName];
            List<ShoppingCartItem> shoppingCartItemsOfThisType = (List<ShoppingCartItem>)(shoppingCartHolderOfThisItemType.CartItems);
            shoppingCartItemsOfThisType.Sort();

            StockItem inventoryItem = ItemsAvailableForSale[itemName];
            int itemsInCart = shoppingCartItemsOfThisType.Count;

            StockItem inventoryItemOfThisType = ItemsAvailableForSale[itemName];
            QuantityDiscount qtyDiscount = inventoryItemOfThisType.QtyDiscount;
            double unitPriceWithDiscount = inventoryItemOfThisType.UnitPrice - (inventoryItemOfThisType.UnitPrice * qtyDiscount.Discount);


            for (int position = 0; position < shoppingCartItemsOfThisType.Count; position++)
            {
                if (QuantityDiscountValid(inventoryItem, position,
                    shoppingCartHolderOfThisItemType.QuantityDiscountsGiven))
                {
                    ShoppingCartItem itemToApplyDiscountTo = shoppingCartItemsOfThisType.ElementAt(position);
                    double itemPrice = unitPriceWithDiscount * itemToApplyDiscountTo.ItemWeight;
                    itemToApplyDiscountTo.UnitPrice = unitPriceWithDiscount;
                    itemToApplyDiscountTo.ItemPrice = itemPrice;
                }
            }
        }

        public double ComputeTotalBill()
        {
            double totalBill = 0;
            foreach (string key in ShoppingCart.Keys)
            {
                IList<ShoppingCartItem> shoppingCartItemsOfThisType = ShoppingCart[key].CartItems;
                foreach (ShoppingCartItem item in shoppingCartItemsOfThisType)
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
        public int FullPriceItems { get; set; }
        public int QuantityUnderDiscount { get; set; }
        public double Discount { get; set; }
    }

    public class GroupDiscount
    {
        public int QuantityToGetDiscount { get; set; }
        public double PriceForGroup { get; set; }
    }

    public class ShoppingCartItemHolder
    {
        public IList<ShoppingCartItem> CartItems { get; }
        public int QuantityDiscountsGiven { get; set; }

        public ShoppingCartItemHolder()
        {
            CartItems = new List<ShoppingCartItem>();
        }
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
