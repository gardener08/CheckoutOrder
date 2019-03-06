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
            _checkoutOrderUnderTest.MarkDownItem("Tomato Soup", .37);
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.37, _checkoutOrderUnderTest.TotalGroceryBill);
        }
    }
}
