﻿using System;
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
            ShoppingCart = new Dictionary<string, ShoppingCartItemHolder>();
            ShoppingCart.Add("Tomato Soup", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Tomato Soup"]));
            ShoppingCart.Add("Bananas", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Bananas"]));
            ShoppingCart.Add("Oranges", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Oranges"]));
            ShoppingCart.Add("Wheaties", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Wheaties"]));
        }

        static void Main(string[] args)
        {
            Program programToRun = new Program();
            programToRun.ScanItem("Tomato Soup");
        }

        public void ScanItem(string itemName)
        {
            StockItem itemToScan = ItemsAvailableForSale[itemName];

            if (itemToScan.Markdown > 0)
            {
                AddScannedEachesItemWithMarkdown(itemToScan, itemName);
            }
            else
            {
                AddScannedEachesItem(itemToScan, itemName);
            }

            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemToScan.ItemName];
            int groupDiscountsGiven = shoppingCartHolderOfThisItemType.GroupDiscountsGiven;
            int cartItemsCountAfterScan = shoppingCartHolderOfThisItemType.CartItems.Count;
            if (GroupingDiscountValid(itemToScan, cartItemsCountAfterScan, groupDiscountsGiven))
            {
                ApplyGroupingDiscountForEachesItemAtSale(itemName);
            }
            ComputeTotalBill();
        }

        private void AddScannedEachesItem(StockItem itemToAdd, string itemName)
        {
            ShoppingCartItemHolder itemHolder = ShoppingCart[itemToAdd.ItemName];
            itemHolder.ScanItem(itemName);
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
            }
            ComputeTotalBill();
        }

        private void AddScannedWeighedItem(StockItem itemToAdd, string itemName, double itemWeight)
        {
            ShoppingCartItemHolder itemHolder = ShoppingCart[itemToAdd.ItemName];
            itemHolder.ScanItem(itemName, itemWeight);
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

        public bool GroupingDiscountValid(StockItem item, int currentItemPositionOneBased, int groupDiscountsGiven)
        {
            GroupDiscount grpDiscount = item.GrpDiscount;
            if (grpDiscount != null)
            {
                bool discountEligible = (currentItemPositionOneBased == ((groupDiscountsGiven + 1) * grpDiscount.QuantityToGetDiscount));
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
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                FullPriceItems = quantityToGetDiscount,
                QuantityUnderDiscount = quantityUnderDiscount,
                Discount = discount
            };
            itemToScan.QtyDiscount = qtyDiscount;
            if (itemToScan.PriceCategory == "byWeight")
            {
                QuantityDiscountEligibleWeighedItemHolder shoppingCartHolderOfThisItemType = new QuantityDiscountEligibleWeighedItemHolder(itemToScan);
                ShoppingCart[itemToScan.ItemName] = shoppingCartHolderOfThisItemType;
            }
            else if (itemToScan.PriceCategory == "eaches")
            {
                QuantityDiscountEligibleEachesItemHolder shoppingCartHolderOfThisItemType = new QuantityDiscountEligibleEachesItemHolder(itemToScan);
                ShoppingCart[itemToScan.ItemName] = shoppingCartHolderOfThisItemType;
            }
        }

        private void ApplyGroupingDiscountForEachesItemAtSale(string itemName)
        {
            ShoppingCartItemHolder shoppingCartHolderOfThisItemType = ShoppingCart[itemName];
            List<ShoppingCartItem> shoppingCartItemsOfThisType = (List<ShoppingCartItem>)(shoppingCartHolderOfThisItemType.CartItems);
            StockItem inventoryItemOfThisType = ItemsAvailableForSale[itemName];
            GroupDiscount discountForThisItemType = inventoryItemOfThisType.GrpDiscount;
            double pricePerItem = discountForThisItemType.PriceForGroup /
                                  discountForThisItemType.QuantityToGetDiscount;

            int numberOfItemsToGetDiscount = inventoryItemOfThisType.GrpDiscount.QuantityToGetDiscount;
            int totalShoppingCartItemsOfThisType = shoppingCartHolderOfThisItemType.CartItems.Count;
            int firstItemIndexToGetDiscount = totalShoppingCartItemsOfThisType - numberOfItemsToGetDiscount;

            for (int i = firstItemIndexToGetDiscount; i < totalShoppingCartItemsOfThisType; i++)
            {
                ShoppingCartItem itemToApplyDiscountTo = shoppingCartItemsOfThisType.ElementAt(i);
                itemToApplyDiscountTo.UnitPrice = pricePerItem;
                itemToApplyDiscountTo.ItemPrice = pricePerItem;
            }
            shoppingCartHolderOfThisItemType.GroupDiscountsGiven++;

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
        public IList<ShoppingCartItem> CartItems { get; set; }
        public int QuantityDiscountsGiven { get; set; }
        public int GroupDiscountsGiven { get; set; }

        protected QuantityDiscount _qtyDiscount;
        protected readonly StockItem _stockItem;

        public ShoppingCartItemHolder(StockItem inventoryItem)
        {
            CartItems = new List<ShoppingCartItem>();
            _qtyDiscount = inventoryItem.QtyDiscount;
            _stockItem = inventoryItem;
        }

        public ShoppingCartItemHolder()
        {
            CartItems = new List<ShoppingCartItem>();
        }

        public virtual void ScanItem(string itemName)
        {
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = _stockItem.UnitPrice,
                ItemPrice = _stockItem.UnitPrice,
            };
            CartItems.Add(currentItemBeingScanned);
        }

        public virtual void ScanItem(string itemName, double itemWeight)
        {
            double unitPrice = _stockItem.UnitPrice - _stockItem.Markdown;
            double itemPrice = unitPrice * itemWeight;
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = unitPrice,
                ItemPrice = itemPrice,
                ItemWeight = itemWeight
            };
            this.CartItems.Add(currentItemBeingScanned);
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
