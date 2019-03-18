using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class QuantityDiscountEligibleWeighedItemHolder : ShoppingCartItemHolder
    {
        public QuantityDiscountEligibleWeighedItemHolder(StockItem inventoryItem) : base(inventoryItem)
        {
            _quantityDiscountEligibleGroups = new Stack<IList<ShoppingCartItem>>();
            if (inventoryItem.QtyDiscount != null)
            {
                List<ShoppingCartItem> firstQuantityDiscountEligibleGroup =
                    new List<ShoppingCartItem>(_qtyDiscount.QuantityUnderDiscount + _qtyDiscount.FullPriceItems);
                _quantityDiscountEligibleGroups.Push(firstQuantityDiscountEligibleGroup);
            }
        }

        private readonly Stack<IList<ShoppingCartItem>> _quantityDiscountEligibleGroups = null;

        public override void ScanItem(string itemName, double itemWeight)
        {
            IList<ShoppingCartItem> currentShoppingCartItemsInThisDiscountGroup = _quantityDiscountEligibleGroups.Peek();
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = _stockItem.UnitPrice,
                ItemPrice = (_stockItem.UnitPrice * itemWeight),
                ItemWeight = itemWeight
            };

            int maxDiscountItemSize = _qtyDiscount.QuantityUnderDiscount + _qtyDiscount.FullPriceItems;
            if (currentShoppingCartItemsInThisDiscountGroup.Count < maxDiscountItemSize)
            {
                currentShoppingCartItemsInThisDiscountGroup.Add(currentItemBeingScanned);
            }
            else
            {
                IList<ShoppingCartItem> nextShoppingCartItemQuantityDiscountGroup = new List<ShoppingCartItem>(maxDiscountItemSize);
                _quantityDiscountEligibleGroups.Push(nextShoppingCartItemQuantityDiscountGroup);
                nextShoppingCartItemQuantityDiscountGroup.Add(currentItemBeingScanned);
            }
            ApplyQuantityDiscountToEligibleItems();
            ConvertToCartItems();
        }

        private void ApplyQuantityDiscountToEligibleItems()
        {
            List<ShoppingCartItem> currentShoppingCartItemsInThisDiscountGroup = (List<ShoppingCartItem>) (_quantityDiscountEligibleGroups.Peek());
            currentShoppingCartItemsInThisDiscountGroup.Sort();
            for (int i = 0; i < currentShoppingCartItemsInThisDiscountGroup.Count; i++)
            {
                if (i >= _qtyDiscount.FullPriceItems)
                {
                    ShoppingCartItem currentItem = currentShoppingCartItemsInThisDiscountGroup.ElementAt(i);
                    double unitPriceWithDiscount = currentItem.UnitPrice - (currentItem.UnitPrice * _qtyDiscount.Discount);
                    double itemPrice = unitPriceWithDiscount * currentItem.ItemWeight;
                    currentItem.ItemPrice = itemPrice;
                    currentItem.UnitPrice = unitPriceWithDiscount;
                }
            }
        }

        private void ConvertToCartItems()
        {
            Stack<IList<ShoppingCartItem>> copyOfTheseCartItems = new Stack<IList<ShoppingCartItem>>(_quantityDiscountEligibleGroups);
            List<ShoppingCartItem> linearCopyOfTheseCartItems = new List<ShoppingCartItem>();
            foreach (IList<ShoppingCartItem> discountGroupItemList in copyOfTheseCartItems)
            {
                foreach (ShoppingCartItem item in discountGroupItemList)
                {
                    linearCopyOfTheseCartItems.Add(item);
                }
            }
            this.CartItems = linearCopyOfTheseCartItems;
        }
    }
}
