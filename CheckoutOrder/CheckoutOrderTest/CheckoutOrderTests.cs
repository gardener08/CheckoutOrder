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
            CheckoutItem itemToScan = new CheckoutItem();
            itemToScan.CurrentPrice = 2.0;
            double totalBill = _checkoutOrderUnderTest.ScanItem(itemToScan);
            Assert.Equal(2.0, totalBill);
        }
    }
}
