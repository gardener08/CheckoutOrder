﻿using System;
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

        [Fact]
        public void MarkDownByWeightItemFromInventory()
        {
            _checkoutOrderUnderTest.MarkDownItem("Bananas", .05);
            _checkoutOrderUnderTest.ScanItem("Bananas", 1.0);
            Assert.Equal(.40, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountEaches()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscount("Tomato Soup", 1, 1, 0.5);
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            Assert.Equal(.63, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountAndThenLoseTheDiscountEaches()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscount("Tomato Soup", 1, 1, 0.5);
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            Assert.Equal(1.05, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ValidQuantityDiscount()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            { 
                Discount = 1.0, QuantityToGetDiscount = 1, QuantityUnderDiscount = 1
            };
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount,
                NumberOfThisItemInCart = 1
            };
            bool quantityDiscountValid = _checkoutOrderUnderTest.QuantityDiscountValid(itemToScan);
            Assert.True(quantityDiscountValid);
        }

        [Fact]
        public void InvalidQuantityDiscountBeyondQuantityLimit()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                Discount = 1.0,
                QuantityToGetDiscount = 1,
                QuantityUnderDiscount = 1
            };
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount,
                NumberOfThisItemInCart = 2
            };
            bool quantityDiscountValid = _checkoutOrderUnderTest.QuantityDiscountValid(itemToScan);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void InvalidQuantityDiscountDidntBuyEnoughToGetDiscount()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                Discount = 1.0,
                QuantityToGetDiscount = 1,
                QuantityUnderDiscount = 1
            };
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount,
                NumberOfThisItemInCart = 0
            };
            bool quantityDiscountValid = _checkoutOrderUnderTest.QuantityDiscountValid(itemToScan);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountByWeight()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscount("Oranges", 2, 1, 0.5);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);

            Assert.Equal(5.00, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ValidGroupingDiscount()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3
            };
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                GrpDiscount = grpDiscount,
                NumberOfThisItemInCart = 3
            };
            bool quantityDiscountValid = _checkoutOrderUnderTest.GroupingDiscountValid(itemToScan);
            Assert.True(quantityDiscountValid);
        }

        [Fact]
        public void ApplyGroupingDiscount()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3,
                PriceForGroup = 5
            };
            _checkoutOrderUnderTest.ApplyGroupingDiscountToItemForSale("Tomato Soup", 3, 5.00);
            StockItem tomatoSoupItem = _checkoutOrderUnderTest.ItemsAvailableForSale["Tomato Soup"];
            AssertThatGroupingDiscountObjectsAreEqual(grpDiscount, tomatoSoupItem.GrpDiscount);
        }

        private void AssertThatGroupingDiscountObjectsAreEqual(GroupDiscount expectedDiscount, GroupDiscount actualDiscount)
        {
            Assert.Equal(expectedDiscount.QuantityToGetDiscount, actualDiscount.QuantityToGetDiscount);
            Assert.Equal(expectedDiscount.PriceForGroup, actualDiscount.PriceForGroup);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountEachesComputeTotalBill()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscount("Tomato Soup", 1, 1, 0.5);
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            _checkoutOrderUnderTest.ScanItem(("Tomato Soup"));
            Assert.Equal(.63, _checkoutOrderUnderTest.ComputeTotalBill());
        }

        [Fact]
        public void BuyAMarkedDownItemComputeTotalBill()
        {
            _checkoutOrderUnderTest.MarkDownItem("Tomato Soup", .04);
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Assert.Equal(.38, _checkoutOrderUnderTest.ComputeTotalBill());
        }

        [Fact]
        public void ScanWeighedItemComputeTotalBill()
        {
            _checkoutOrderUnderTest.ScanItem("Bananas", 2.2);
            Assert.Equal(.99, _checkoutOrderUnderTest.ComputeTotalBill(), 2);

        }

    }
}
