using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCRawListTests
{
    [Fact]
    public void PackToCount_OnEmptyList_ThenAdd_DoesNotThrow()
    {
        var list = new CCRawList<int>();
        list.Add(1);
        list.Add(2);
        list.Clear();        // count = 0, backing buffer retained
        list.PackToCount();  // must not leave a zero-length backing array...

        list.Add(42);        // ...or this throws IndexOutOfRange (Capacity stuck at 0)

        Assert.Equal(1, list.count);
        Assert.Equal(42, list.Elements[0]);
    }

    [Fact]
    public void Pooled_AddAndGrow_StoresValuesAndFrees()
    {
        var list = new CCRawList<int>(useArrayPool: true);  // backing buffer rented from the shared pool
        for (int i = 0; i < 100; i++)
            list.Add(i);                                    // forces several pooled grows

        Assert.Equal(100, list.count);
        Assert.Equal(50, list.Elements[50]);

        list.Clear(true);                                   // returns the rented buffer to the shared pool
        Assert.Equal(0, list.count);
    }

    [Fact]
    public void Pooled_PackToCount_NeverGrowsCapacity()
    {
        var list = new CCRawList<int>(useArrayPool: true);
        for (int i = 0; i < 100; i++)
            list.Add(i);                 // grows to a large pooled bucket
        int before = list.Elements.Length;

        list.RemoveAt(50, 50);           // count: 100 -> 50, values 0..49 retained
        list.PackToCount();

        Assert.Equal(50, list.count);
        Assert.True(list.Elements.Length <= before);      // packing must never increase capacity
        Assert.True(list.Elements.Length >= list.count);  // ...but must still fit the elements
        Assert.Equal(49, list.Elements[49]);
    }
}
