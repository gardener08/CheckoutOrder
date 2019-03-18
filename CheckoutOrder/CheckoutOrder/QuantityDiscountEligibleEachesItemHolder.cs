using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class QuantityDiscountEligibleEachesItemHolder : ShoppingCartItemHolder
    {
        public QuantityDiscountEligibleEachesItemHolder(StockItem inventoryItem) : base(inventoryItem)
        {
        }

        public override void ScanItem(string itemName)
        {
            int quantityDiscountsGiven = this.QuantityDiscountsGiven;
            int cartItemsCount = CartItems.Count;
            if (QuantityDiscountValid(_stockItem, cartItemsCount, quantityDiscountsGiven))
            {
                QuantityDiscount qtyDiscount = _stockItem.QtyDiscount;
                double priceWithDiscount = _stockItem.UnitPrice - (_stockItem.UnitPrice * qtyDiscount.Discount);
                ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
                {
                    UnitPrice = priceWithDiscount,
                    ItemPrice = priceWithDiscount
                };
                CartItems.Add(currentItemBeingScanned);
            }
            else
            {
                ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
                {
                    UnitPrice = _stockItem.UnitPrice,
                    ItemPrice = _stockItem.UnitPrice,
                };
                CartItems.Add(currentItemBeingScanned);
            }
            UpdateQuantityDiscountCountIfDiscountWasFullyUsed(_stockItem);
        }

        public bool QuantityDiscountValid(StockItem item, int currentItemPosition, int quantityDiscountsGiven)
        {
            QuantityDiscount qtyDiscount = item.QtyDiscount;
            if (qtyDiscount != null)
            {
                if ((qtyDiscount.MaximumNumberOfDiscounts > 0) && (quantityDiscountsGiven > qtyDiscount.MaximumNumberOfDiscounts))
                {
                    return false;
                }
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

        private void UpdateQuantityDiscountCountIfDiscountWasFullyUsed(StockItem inventoryItem)
        {
            int quantityDiscountsGiven = this.QuantityDiscountsGiven;
            int updatedCartItemsCount = this.CartItems.Count;
            QuantityDiscount qtyDiscount = inventoryItem.QtyDiscount;

            if ((qtyDiscount.MaximumNumberOfDiscounts > 0) && (quantityDiscountsGiven > qtyDiscount.MaximumNumberOfDiscounts))
            {
                return;
            }
            int itemsToGetAFullDiscount = (quantityDiscountsGiven + 1) *
                (inventoryItem.QtyDiscount.QuantityUnderDiscount + inventoryItem.QtyDiscount.FullPriceItems);

            if ((updatedCartItemsCount % itemsToGetAFullDiscount) == 0)
            {
                this.QuantityDiscountsGiven++;
            }
        }
    }
}
