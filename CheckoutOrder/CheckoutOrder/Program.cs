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
        private bool _itemsHaveBeenScanned;

        public Program()
        {
            TotalGroceryBill = 0;
            ShoppingCart = new Dictionary<string, ShoppingCartItemHolder>();
            ShoppingCart.Add("Tomato Soup", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Tomato Soup"]));
            ShoppingCart.Add("Bananas", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Bananas"]));
            ShoppingCart.Add("Oranges", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Oranges"]));
            ShoppingCart.Add("Wheaties", new ShoppingCartItemHolder(this.ItemsAvailableForSale["Wheaties"]));
            _itemsHaveBeenScanned = false;
        }

        static void Main(string[] args)
        {
            Program programToRun = new Program();
            programToRun.ScanItem("Tomato Soup");
        }

        public void ScanItem(string itemName)
        {
            _itemsHaveBeenScanned = true;
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            {
                AddScannedEachesItem(itemToScan, itemName);
            }

            ComputeTotalBill();
        }

        private void AddScannedEachesItem(StockItem itemToAdd, string itemName)
        {
            ShoppingCartItemHolder itemHolder = ShoppingCart[itemToAdd.ItemName];
            itemHolder.ScanItem(itemName);
        }

        public void ScanItem(string itemName, double itemWeight)
        {
            _itemsHaveBeenScanned = true;
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
            if (_itemsHaveBeenScanned)
            {
                throw new InvalidStoreOperationException("Markdowns and Specials cannot be applied after items have been scanned.");
            }
            StockItem inventoryItem = ItemsAvailableForSale[itemName];
            ItemsAvailableForSale[itemName].Markdown = markdown;
            MarkedDownEachesItemHolder shoppingCartHolderOfThisItemType = new MarkedDownEachesItemHolder(inventoryItem);
            ShoppingCart[inventoryItem.ItemName] = shoppingCartHolderOfThisItemType;
        }

        public void ApplyGroupingDiscountSpecial(string itemName, int quantityToGetDiscount, double priceForGroup,
            int maxNumberOfDiscounts)
        {
            if (_itemsHaveBeenScanned)
            {
                throw new InvalidStoreOperationException("Markdowns and Specials cannot be applied after items have been scanned.");
            }
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                PriceForGroup = priceForGroup,
                QuantityToGetDiscount = quantityToGetDiscount,
                MaxNumberOfDiscounts = maxNumberOfDiscounts
            };
            itemToScan.GrpDiscount = grpDiscount;

            if (itemToScan.PriceCategory == "eaches")
            {
                GroupDiscountEligibleEachesItemHolder shoppingCartHolderOfThisItemType =
                    new GroupDiscountEligibleEachesItemHolder(itemToScan);
                ShoppingCart[itemToScan.ItemName] = shoppingCartHolderOfThisItemType;
            }
        }

        public void ApplyQuantityDiscountSpecial(string itemName, int quantityToGetDiscount, int quantityUnderDiscount, double discount, int maxNumberOfDiscounts)
        {
            if (_itemsHaveBeenScanned)
            {
                throw new InvalidStoreOperationException("Markdowns and Specials cannot be applied after items have been scanned.");
            }
            StockItem itemToScan = ItemsAvailableForSale[itemName];
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                FullPriceItems = quantityToGetDiscount,
                QuantityUnderDiscount = quantityUnderDiscount,
                Discount = discount,
                MaximumNumberOfDiscounts = maxNumberOfDiscounts
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
}
