using System;
using SmartFridge;
using Xunit;

namespace SmartFridgeTests
{
    public class SmartFridgeManagerTests
    {
        [Fact]
        public void AnAddedItemCanBeRemoved()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemType = 123;
            var itemUUID = "8976";
            var itemName = "Orange Juice";
            double fillFactor = 1.0d;

            // Act
            sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor);
            sfm.HandleItemRemoved(itemUUID);

            // Assert
            // If we got here without exception that's good enough for this test
        }

        [Fact]
        public void AnUnaddedItemCanNotBeRemoved()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemUUID = "8976";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sfm.HandleItemRemoved(itemUUID));
        }

        [Fact]
        public void SameItemCannotBeRemovedTwice()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemUUID = "8976";
            var itemType = 123;
            var itemName = "Orange Juice";
            double fillFactor = 1.0d;
            sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor);
            sfm.HandleItemRemoved(itemUUID);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sfm.HandleItemRemoved(itemUUID));
        }

        [Fact]
        public void SameItemCannotBeAddedTwice()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemUUID = "8976";
            var itemType = 123;
            var itemName = "Orange Juice";
            double fillFactor = 1.0d;
            sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor));
        }

        [Fact]
        public void ItemCannotHaveAFillFactorLessThanZero()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemUUID = "8976";
            var itemType = 123;
            var itemName = "Orange Juice";
            double fillFactor = -1.0d;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor));
        }

        [Fact]
        public void ItemCannotHaveAFillFactorGreaterThanOne()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemUUID = "8976";
            var itemType = 123;
            var itemName = "Orange Juice";
            double fillFactor = 2.0d;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sfm.HandleItemAdded(itemType, itemUUID, itemName, fillFactor));
        }

        [Fact]
        public void GetItemsForShoppingListReturnsCorrectItems_WhenFridgeHasOneItemAlmostEmpty()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "18976";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.25d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            double thresholdFillFactor = 0.5;
            object[] expectedResult = new object[] { itemType, item1FillFactor };

            // Act
            var items = sfm.GetItems(thresholdFillFactor);

            // Assert
            Assert.Single(items);
            Assert.Contains(expectedResult, items);
        }

        [Fact]
        public void GetItemsForShoppingListReturnsCorrectItems_WhenFridgeHasNoAlmostEmptyItems()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "18976";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.75d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            double thresholdFillFactor = 0.5;

            // Act
            var items = sfm.GetItems(thresholdFillFactor);

            // Assert
            Assert.Empty(items);
        }

        [Fact]
        public void GetItemsForShoppingListReturnsCorrectItems_WhenAnItemIsFullyConsumed()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "18976";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.75d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            sfm.HandleItemRemoved(item1UUID);
            double thresholdFillFactor = 0.5;
            object[] expectedResult = new object[] { itemType, 0d };

            // Act
            var items = sfm.GetItems(thresholdFillFactor);

            // Assert
            Assert.Single(items);
            Assert.Contains(expectedResult, items);
        }

        [Fact]
        public void GetItemsForShoppingListReturnsCorrectItems_WhenFridgeHasOneItemAlmostEmptyButSameItemFull()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.25d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            var item2UUID = "2897";
            double item2FillFactor = 1.0d;
            sfm.HandleItemAdded(itemType, item2UUID, itemName, item2FillFactor);
            double thresholdFillFactor = 0.5;

            // Act
            var items = sfm.GetItems(thresholdFillFactor);

            // Assert
            Assert.Empty(items);
        }

        [Fact]
        public void GetItemsForShoppingListReturnsCorrectItems_WhenAnItemIsForgottenItShouldNotBeReturned()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 1.0d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            double thresholdFillFactor = 0.5;

            // Act
            sfm.ForgetItem(itemType);
            var items = sfm.GetItems(thresholdFillFactor);

            // Assert
            Assert.Empty(items);
        }

        [Fact]
        public void ForgettingAnItemNotAlreadyStocked_ThrowsAnException()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var itemType = 123L;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sfm.ForgetItem(itemType));
        }

        [Fact]
        public void GetFillFactor_WhenFridgeHasOneItemOfGivenType()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.25d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);

            // Act
            var actualFillFactor = sfm.GetFillFactor(itemType);

            // Assert
            Assert.Equal(item1FillFactor, actualFillFactor);
        }

        [Fact]
        public void GetFillFactor_WhenFridgeHasTwoItemsOfGivenType()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var itemType = 123L;
            var itemName = "Orange Juice";
            double item1FillFactor = 0.25d;
            sfm.HandleItemAdded(itemType, item1UUID, itemName, item1FillFactor);
            var item2UUID = "2897";
            double item2FillFactor = 1.0d;
            sfm.HandleItemAdded(itemType, item2UUID, itemName, item2FillFactor);
            var expectedFillFactor = item1FillFactor + item2FillFactor;

            // Act
            var actualFillFactor = sfm.GetFillFactor(itemType);

            // Assert
            Assert.Equal(expectedFillFactor, actualFillFactor);
        }

        [Fact]
        public void GetFillFactor_WhenFridgeHasUnrelatedItems()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var ojItemType = 123L;
            var itemName = "Orange Juice";
            double ojFillFactor = 0.25d;
            sfm.HandleItemAdded(ojItemType, item1UUID, itemName, ojFillFactor);
            var item2UUID = "2897";
            double item2FillFactor = 1.0d;
            var otherItemType = 333L;
            sfm.HandleItemAdded(otherItemType, item2UUID, itemName, item2FillFactor);

            // Act
            var actualFillFactor = sfm.GetFillFactor(ojItemType);

            // Assert
            Assert.Equal(ojFillFactor, actualFillFactor);
        }
        [Fact]
        public void GetItems_NegativeThresholdNotAllowed()
        {
            // Arrange
            var sfm = new SmartFridgeManager();
            var item1UUID = "1897";
            var ojItemType = 123L;
            var itemName = "Orange Juice";
            double ojFillFactor = 0.25d;
            sfm.HandleItemAdded(ojItemType, item1UUID, itemName, ojFillFactor);
            var item2UUID = "2897";
            double item2FillFactor = 1.0d;
            var otherItemType = 333L;
            sfm.HandleItemAdded(otherItemType, item2UUID, itemName, item2FillFactor);
            var threshold = -1.0d;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sfm.GetItems(threshold));
        }
    }
}
