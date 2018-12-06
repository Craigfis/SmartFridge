# Craig's SmartFridge Creation Process:
	- Study interface.
		○ Hmmm, what does fillFactor mean? E.g. How full a bottle of OJ is?
		○ Each item in the Fridge has a UUID
	- Convert to C#
	- Implement stub class. (Note, I don't like the suffix "Manager" on a class, but I'll use it here to reflect the interface naming)
	- Create dictionary container to hold items inside  Manager
	- What should be first test?
		○ Add an item and remove it
			§ Forces me to implement the Add and Remove methods
	- Next test - Remove an item that hasn't been added should throw
			§ Forces me to actually store the result of the add call
	- Test: Try removing an item twice - should throw
		○ Forces me to modify the collection when an item  is removed
	- Next test: GetItems should return items that need replenishing
		○ Time to implement GetItems
		○ The interface says to return an array of arrays of objects. I don't like that structure (I'd prefer something strongly typed) but will roll with it
		○ Hmmm…interface says it should include items that are depleted - presumably when an item is depleted it is removed from the fridge and not re-added. Aha! So we need to keep track of items that have been in the fridge (unless they've been "forgotten" - hence the forgetItem method).
		○ OK, first test a simple case - we added a 25% full bottle of OJ
	- Got that working. Next a test to make sure nothing comes back when there are no near empty items. OK, that passes.
	- Next a test for when an item was removed and not returned (presumably because it was all used up). This forces me to implement the tracking of items that have been added to the fridge at some point and not forgotten. 
	- OK, got that working
	- Next test: don't return items if there is one container with less than the fillfactor amount and another with more
	- OK, that's working. Next test: forgetting an item should remove it from the stock list and not return it from getItems
	- OK, next to test (& implement) the GetFillFactor method…
	- Error scenarios…
		○ What should be the behavior if an item is added to the fridge when that item (UUID) is already recorded as being in the fridge? Throw an exception? Overwrite? I'm going to go with throw an Exception (because this suggests likely an error on behalf of the caller that they should know about), but this would be a requirement I would get clarified.
		○ What should happen if the user tries to forget an item that was not already stocked?
			§ Again I'll opt for throwing an exception by the same reasoning above.
		○ An item not already in the fridge cannot be removed. Throw an exception.
		○ Negative fill factors should not be allowed. Throw.
    
	- Other thoughts
		○ Persistence of state. My implementation is purely memory based. A more real application would probably need some kind of more permanent backing store such that it could survive power outages for example. (Who wants to tell their fridge the contents after each blackout!?)
		○ Performance. Given the likely frequency of fridge operations, this doesn't seem like a particularly performance critical application and I haven't tried to do any performance optimization. If this was running on a standalone device, performance is probably not critical. If however this was a web service supporting millions of fridge devices, performance would indeed become critical and justify profiling.
		Scalability. I imagine the capacity of a fridge is generally just a few dozen items so I don't think the scalability of the SmartFridgeManager is a concern. 
