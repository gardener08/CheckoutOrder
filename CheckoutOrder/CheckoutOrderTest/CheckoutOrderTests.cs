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
        public void VoidItem()
        {
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            _checkoutOrderUnderTest.VoidItem("Tomato Soup");
            Assert.Equal(0, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        private void ScanMultipleEachesItems(Program itemUnderTest, string itemName, int numberOfItemsToScan)
        {
            for (int i = 0; i < numberOfItemsToScan; i++)
            {
                itemUnderTest.ScanItem(itemName);
            }
        }

        private void ScanMultipleWeighedItemsWithTheSameWeight(Program itemUnderTest, string itemName, double itemWeight, int numberOfItemsToScan)
        {
            for (int i = 0; i < numberOfItemsToScan; i++)
            {
                itemUnderTest.ScanItem(itemName, itemWeight);
            }
        }

        [Fact]
        public void ScanItemTwiceAndReturnTotal()
        {
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 2);
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
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 2);
            Assert.Equal(.63, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountEachesAndThenVoidOne()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 2);
            _checkoutOrderUnderTest.VoidItem("Tomato Soup");
            Assert.Equal(.42, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountMultipleTimesEaches()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 4);
            Assert.Equal(1.26, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountMultipleTimesEachesAndThenVoidOne()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 4);
            _checkoutOrderUnderTest.VoidItem("Tomato Soup");
            Assert.Equal(1.05, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountMultipleTimesEachesAndThenVoidTwo()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 4);
            _checkoutOrderUnderTest.VoidItem("Tomato Soup");
            _checkoutOrderUnderTest.VoidItem("Tomato Soup");
            Assert.Equal(0.63, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountAndThenLoseTheDiscountEaches()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 3);
            Assert.Equal(1.05, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ValidQuantityDiscount()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            { 
                Discount = 1.0, FullPriceItems = 1, QuantityUnderDiscount = 1
            };
            int numberOfItemsInThisCart = 1;
            int quantityDiscountsGiven = 0;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount
            };

            QuantityDiscountEligibleEachesItemHolder shoppingCartItemHolder = new QuantityDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.QuantityDiscountValid(itemToScan, numberOfItemsInThisCart, quantityDiscountsGiven);
            Assert.True(quantityDiscountValid);
        }

        [Fact]
        public void InvalidQuantityDiscountBeyondQuantityLimit()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                Discount = 1.0,
                FullPriceItems = 1,
                QuantityUnderDiscount = 1
            };
            int numberOfItemsInThisCart = 2;
            int quantityDiscountsGiven = 0;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount
            };

            QuantityDiscountEligibleEachesItemHolder shoppingCartItemHolder = new QuantityDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.QuantityDiscountValid(itemToScan, numberOfItemsInThisCart, quantityDiscountsGiven);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void InvalidQuantityDiscountDidntBuyEnoughToGetDiscount()
        {
            QuantityDiscount qtyDiscount = new QuantityDiscount()
            {
                Discount = 1.0,
                FullPriceItems = 1,
                QuantityUnderDiscount = 1
            };
            int numberOfItemsInThisCart = 0;
            int quantityDiscountsGiven = 0;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                QtyDiscount = qtyDiscount
            };

            QuantityDiscountEligibleEachesItemHolder shoppingCartItemHolder = new QuantityDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.QuantityDiscountValid(itemToScan, numberOfItemsInThisCart, quantityDiscountsGiven);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountByWeight()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            Assert.Equal(5.00, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyEnoughWeighedItemsToGetTwoQuantityDiscounts()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);

            Assert.Equal(10, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyEnoughWeighedItemsToGetTwoQuantityDiscountsAndThenVoidAndReAddSome()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            ScanMultipleWeighedItemsWithTheSameWeight(_checkoutOrderUnderTest, "Oranges", 2.0, 6);

            _checkoutOrderUnderTest.VoidItem("Oranges");
            _checkoutOrderUnderTest.VoidItem("Oranges");
            _checkoutOrderUnderTest.VoidItem("Oranges");
            _checkoutOrderUnderTest.VoidItem("Oranges");

            ScanMultipleWeighedItemsWithTheSameWeight(_checkoutOrderUnderTest, "Oranges", 2.0, 4);

            Assert.Equal(10, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyEnoughWeighedItemsOfVaryingWeightsToGetTwoQuantityDiscounts()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 4.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 10.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 5.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 8.0);

            Assert.Equal(28.5, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyEnoughWeighedItemsOfVaryingWeightsToGetTwoQuantityDiscountsAlongWithExtraItems()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 4.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 10.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 5.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 8.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);

            Assert.Equal(34.5, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountWithExtrasAtFullPriceByWeight()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            Assert.Equal(7.00, _checkoutOrderUnderTest.TotalGroceryBill);
        }


        [Fact]
        public void BuySomeGetSomeMoreOfEqualOrLesserValueAtADiscountByWeight()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 4.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);

            Assert.Equal(8.00, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ValidGroupingDiscount()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3
            };
            int numberOfItemsInThisCart = 3;
            int groupDiscountsGiven = 0;
            int maxNumberOfDiscounts = 1;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                GrpDiscount = grpDiscount,
            };
            GroupDiscountEligibleEachesItemHolder shoppingCartItemHolder = new GroupDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.GroupingDiscountValid(itemToScan, numberOfItemsInThisCart, groupDiscountsGiven, maxNumberOfDiscounts);
            Assert.True(quantityDiscountValid);
        }

        [Fact]
        public void InvalidGroupingDiscountExceededMaximumNumberOfGroupings()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3
            };
            int numberOfItemsInThisCart = 6;
            int groupDiscountsGiven = 1;
            int maxNumberOfDiscounts = 1;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                GrpDiscount = grpDiscount,
            };
            GroupDiscountEligibleEachesItemHolder shoppingCartItemHolder = new GroupDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.GroupingDiscountValid(itemToScan, numberOfItemsInThisCart, groupDiscountsGiven, maxNumberOfDiscounts);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void InvalidGroupingDiscountHaventHitCountToGetDiscount()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3
            };
            int numberOfItemsInThisCart = 5;
            int groupDiscountsGiven = 1;
            int maxNumberOfDiscounts = 1;
            StockItem itemToScan = new StockItem()
            {
                ItemName = "TestItem",
                PriceCategory = "eaches",
                UnitPrice = 2.0,
                GrpDiscount = grpDiscount,
            };
            GroupDiscountEligibleEachesItemHolder shoppingCartItemHolder = new GroupDiscountEligibleEachesItemHolder(itemToScan);
            bool quantityDiscountValid = shoppingCartItemHolder.GroupingDiscountValid(itemToScan, numberOfItemsInThisCart, groupDiscountsGiven, maxNumberOfDiscounts);
            Assert.False(quantityDiscountValid);
        }

        [Fact]
        public void ApplyGroupingDiscount()
        {
            GroupDiscount grpDiscount = new GroupDiscount()
            {
                QuantityToGetDiscount = 3,
                PriceForGroup = 5
            };
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Tomato Soup", 3, 5.00, -1);
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
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 2);
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

        [Fact]
        public void BuySomeGetSomeMoreAtADiscountByWeightComputeTotalBill()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);

            Assert.Equal(5.00, _checkoutOrderUnderTest.ComputeTotalBill());
        }

        [Fact]
        public void BuyItemsThatGetAGroupDiscount()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 3);

            Assert.Equal(7.00, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyItemsThatGetAGroupDiscountAndExtras()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 4);

            Assert.Equal(10.25, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyItemsThatGetTwoGroupDiscounts()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 6);

            Assert.Equal(14, _checkoutOrderUnderTest.TotalGroceryBill, 2);
        }

        [Fact]
        public void BuyItemsThatGetTwoGroupDiscountsWithAnExtraItem()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 7);

            Assert.Equal(17.25, _checkoutOrderUnderTest.TotalGroceryBill, 2);
        }

        [Fact]
        public void BuyItemsThatDidntQuiteGetAGroupDiscount()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, -1);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 2);

            Assert.Equal(6.50, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanMultipleEachesItemsWithQuantityDiscountLimitAndAnExtraItem()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 2, 1, .5, 4);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Tomato Soup", 13);
            Assert.Equal(4.62, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanMultipleWeighedItemsWithQuantityDiscountLimitAndAnExtraItemSameWeight()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, .5, 4);
            ScanMultipleWeighedItemsWithTheSameWeight(_checkoutOrderUnderTest, "Oranges", 2, 13);
            Assert.Equal(22, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void ScanMultipleWeighedItemsWithQuantityDiscountLimitAndAnExtraItem()
        {
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, .5, 4);
            // Total for this group should be 8.0
            _checkoutOrderUnderTest.ScanItem("Oranges", 4.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);

            // Total for this group should be 16
            _checkoutOrderUnderTest.ScanItem("Oranges", 8.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 4.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 6.0);

            // Total for this group should be 5.5
            _checkoutOrderUnderTest.ScanItem("Oranges", 1.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            _checkoutOrderUnderTest.ScanItem("Oranges", 3.0);

            // Total for this group should be 2.5
            _checkoutOrderUnderTest.ScanItem("Oranges", 1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 1);
            _checkoutOrderUnderTest.ScanItem("Oranges", 1);

            _checkoutOrderUnderTest.ScanItem("Oranges", 10);

            Assert.Equal(42, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void BuyItemsThatGetTheLimitOnGroupDiscountsAndExtras()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, 2);
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 10);

            Assert.Equal(27, _checkoutOrderUnderTest.TotalGroceryBill);
        }

        [Fact]
        public void GroupingSpecialApplicationAttemptedWhileStoreIsOpenEachesItemsScannedAlready()
        {
            ScanMultipleEachesItems(_checkoutOrderUnderTest, "Wheaties", 10);
            Exception ex = Assert.Throws<InvalidStoreOperationException>(() => _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7.00, 2));
        }

        [Fact]
        public void QuantitySpecialApplicationAttemptedWhileStoreIsOpenWeighedItemsScannedAlready()
        {
            _checkoutOrderUnderTest.ScanItem("Oranges", 2.0);
            Exception ex = Assert.Throws<InvalidStoreOperationException>(() => _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 2, 1, 0.5, -1));
        }

        [Fact]
        public void QuantitySpecialApplicationAttemptedWhileStoreIsOpenEachesItemsScannedAlready()
        {
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Exception ex = Assert.Throws<InvalidStoreOperationException>(() => _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Tomato Soup", 1, 1, 0.5, -1));
        }

        [Fact]
        public void MarkdownAttemptedWhileStoreIsOpenEachesItemsScannedAlready()
        {
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            Exception ex = Assert.Throws<InvalidStoreOperationException>(() => _checkoutOrderUnderTest.MarkDownItem("Tomato Soup", 0.05));
        }

        [Fact]
        public void ScanningAnEachesItemAsAWeighedItem()
        {
            Exception ex = Assert.Throws<InvalidStoreOperationException>(() => _checkoutOrderUnderTest.ScanItem("Tomato Soup", 2.0));
        }

        [Fact]
        public void ScanMultipleWeightedAndEachesItemsWithSpecials()
        {
            _checkoutOrderUnderTest.ApplyGroupingDiscountSpecial("Wheaties", 3, 7, -1);
            _checkoutOrderUnderTest.MarkDownItem("Tomato Soup", 0.17);
            _checkoutOrderUnderTest.ApplyQuantityDiscountSpecial("Oranges", 1, 1, .5, -1);

            _checkoutOrderUnderTest.ScanItem("Oranges", 1);
            _checkoutOrderUnderTest.ScanItem("Wheaties");
            _checkoutOrderUnderTest.ScanItem("Wheaties");
            _checkoutOrderUnderTest.ScanItem("Oranges", 1);
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            _checkoutOrderUnderTest.ScanItem("Tomato Soup");
            _checkoutOrderUnderTest.ScanItem("Wheaties");

            Assert.Equal(9, _checkoutOrderUnderTest.TotalGroceryBill, 2);
        }
    }
}
