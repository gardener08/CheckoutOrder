﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutOrder
{
    class MarkedDownEachesItemHolder : ShoppingCartItemHolder
    {
        public MarkedDownEachesItemHolder(StockItem inventoryItem) : base(inventoryItem)
        {
        }

        public override void ScanItem(string itemName)
        {
            double priceWithMarkdown = _stockItem.UnitPrice - _stockItem.Markdown;
            ShoppingCartItem currentItemBeingScanned = new ShoppingCartItem()
            {
                UnitPrice = priceWithMarkdown,
                ItemPrice = priceWithMarkdown
            };
            CartItems.Add(currentItemBeingScanned);
        }
    }
}
