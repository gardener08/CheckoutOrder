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
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.42, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanItemTwiceAndReturnTotal()
        {
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.84, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanItemFromInventoryOnce()
        {
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.42, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void MarkDownItemFromInventory()
        {
            _checkoutOrderUnderTest.MarkDownItem("Tomato Soup", .04);
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.38, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanWeighedItem()
        {
            _checkoutOrderUnderTest.ScanItem("Bananas", 2.2);
            Assert.Equal(.99, _checkoutOrderUnderTest.TotalGroceryBill, 2);

        }

        [Fact]
        public void ScanMultipleWeightedAndEachesItems()
        {
            _checkoutOrderUnderTest.ScanItem("Bananas", 2.2);
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            _checkoutOrderUnderTest.ScanItem("Bananas", 1.0);
            Assert.Equal(1.86, _checkoutOrderUnderTest.TotalGroceryBill, 2);
        }
    }
}
