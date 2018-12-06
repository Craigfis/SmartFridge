using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartFridge
{
    public class SmartFridgeManager : ISmartFridgeManager
    {
        class FridgeItem
        {
            public string ItemUUID;
            public long ItemType;
            public string Name;
            public double FillFactor;
        }

        private Dictionary<string, FridgeItem> CurrentItems = new Dictionary<string, FridgeItem>();
        private Dictionary<long,string> StockedItems = new Dictionary<long, string>(); // List of items we stock in the fridge (itemtype,name) pairs

        public void HandleItemRemoved(string itemUUID)
        {
            if (!CurrentItems.Remove(itemUUID))
                throw new InvalidOperationException("Item not present in fridge");
        }

        // This method is called every time an item is stored in the fridge
        public void HandleItemAdded(long itemType, string itemUUID, string name, double fillFactor)
        {
            if (CurrentItems.ContainsKey(itemUUID))
                throw new InvalidOperationException("Item is already in the fridge");
            if (fillFactor < 0d || fillFactor > 1.0d)
                throw new ArgumentOutOfRangeException(nameof(fillFactor));

            CurrentItems.Add(itemUUID, new FridgeItem
            {
                ItemUUID = itemUUID,
                ItemType = itemType,
                Name = name,
                FillFactor = fillFactor
            });
            if (!StockedItems.ContainsKey(itemType))
            {
                StockedItems.Add(itemType, name);
            }
        }

        // Returns a list of items based on their fill factor. This method is used by the
        // fridge to display items that are running low and need to be replenished.
        public object[] GetItems(double fillFactor)
        {
            var itemLevels = StockedItems
                .Select(s =>
                    new object[]
                    {
                        s.Key,
                        CurrentItems.Where(c => c.Value.ItemType == s.Key).Sum(c => c.Value.FillFactor)
                    });

            return itemLevels.Where(i => (double)i[1] <= fillFactor).ToArray();
        }

        public double GetFillFactor(long itemType)
        {
            return CurrentItems.Where(c => c.Value.ItemType == itemType).Sum(c => c.Value.FillFactor);
        }

        public void ForgetItem(long itemType)
        {
            if (!StockedItems.Remove(itemType))
                throw new InvalidOperationException("Requested item type is already not stocked");
        }
    }
}
