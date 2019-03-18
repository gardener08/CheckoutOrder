using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
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
}
