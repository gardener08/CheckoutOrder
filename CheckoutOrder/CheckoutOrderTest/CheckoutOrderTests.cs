using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CheckoutOrder;

namespace CheckoutOrderTest
{
    public class CheckoutOrderTests
    {
        private Program _checkoutOrderUnderTest = null;
        public CheckoutOrderTests()
        {
            _checkoutOrderUnderTest = new Program();
        }

        [Fact]
        public void ScanItem()
        {
            CheckoutItem itemToScan = new CheckoutItem("TestItem", 2);
            itemToScan.CurrentPrice = 2.0;
            _checkoutOrderUnderTest.ScanItem(itemToScan);
            Assert.Equal(2.0, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanItemTwiceAndReturnTotal()
        {
            CheckoutItem itemToScan = new CheckoutItem("TestItem", 2);
            itemToScan.CurrentPrice = 2.0;
            _checkoutOrderUnderTest.ScanItem(itemToScan);
            _checkoutOrderUnderTest.ScanItem(itemToScan);
            Assert.Equal(4.0, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanItemFromInventoryOnce()
        {
            _checkoutOrderUnderTest.ScanItem(_checkoutOrderUnderTest.ItemsAvailableForSale["Tomato Soup"]);
            Assert.Equal(.42, _checkoutOrderUnderTest.TotalGroceryBill);
        }
    }
}
