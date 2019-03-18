namespace CheckoutOrder
{
    public class MarkedDownEachesItemHolder : ShoppingCartItemHolder
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
