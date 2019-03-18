using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    public class GroupDiscountEligibleEachesItemHolder : ShoppingCartItemHolder
    {
        public GroupDiscountEligibleEachesItemHolder(StockItem inventoryItem) : base(inventoryItem)
        {
        }

        public override void ScanItem(string itemName)
        {
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = _stockItem.UnitPrice,
                ItemPrice = _stockItem.UnitPrice,
            };
            CartItems.Add(currentItemBeingScanned);

            int groupDiscountsGiven = GroupDiscountsGiven;
            int cartItemsCountAfterScan = CartItems.Count;
            if (GroupingDiscountValid(_stockItem, cartItemsCountAfterScan, groupDiscountsGiven, _stockItem.GrpDiscount.MaxNumberOfDiscounts))
            {
                ApplyGroupingDiscountForEachesItemAtSale();
            }
        }

        private void ApplyGroupingDiscountForEachesItemAtSale()
        {
            GroupDiscount discountForThisItemType = _stockItem.GrpDiscount;
            double pricePerItem = discountForThisItemType.PriceForGroup /
                                  discountForThisItemType.QuantityToGetDiscount;

            int numberOfItemsToGetDiscount = _stockItem.GrpDiscount.QuantityToGetDiscount;
            int totalShoppingCartItemsOfThisType = CartItems.Count;
            int firstItemIndexToGetDiscount = totalShoppingCartItemsOfThisType - numberOfItemsToGetDiscount;

            for (int i = firstItemIndexToGetDiscount; i < totalShoppingCartItemsOfThisType; i++)
            {
                ShoppingCartItem itemToApplyDiscountTo = CartItems.ElementAt(i);
                itemToApplyDiscountTo.UnitPrice = pricePerItem;
                itemToApplyDiscountTo.ItemPrice = pricePerItem;
            }
            GroupDiscountsGiven++;
        }

        public bool GroupingDiscountValid(StockItem item, int currentItemPositionOneBased, int groupDiscountsGiven, int maxNumberOfDiscounts)
        {
            GroupDiscount grpDiscount = item.GrpDiscount;
            if (grpDiscount != null)
            {
                int nextItemToTriggerGroupDiscount = (groupDiscountsGiven + 1) * grpDiscount.QuantityToGetDiscount;
                if ((nextItemToTriggerGroupDiscount > (maxNumberOfDiscounts * grpDiscount.QuantityToGetDiscount))
                    && (maxNumberOfDiscounts > 0))
                {
                    return false;
                }
                bool discountEligible = (currentItemPositionOneBased == nextItemToTriggerGroupDiscount);
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
    }
}
