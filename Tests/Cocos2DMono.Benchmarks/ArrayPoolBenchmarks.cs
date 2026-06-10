using BenchmarkDotNet.Attributes;

namespace Cocos2DMono.Benchmarks;

// Defends the planned Cocos2D.ArrayPool<T> -> System.Buffers.ArrayPool<T> swap (Phase 2.2).
// Compares a rent/return cycle of the custom pool against the BCL shared pool.
// (Types are fully qualified to disambiguate Cocos2D.ArrayPool from System.Buffers.ArrayPool.)
[MemoryDiagnoser]
public class ArrayPoolBenchmarks
{
    [Params(256)]
    public int Size;

    [Benchmark(Baseline = true)]
    public int Custom_CreateFree()
    {
        int[] array = Cocos2D.ArrayPool<int>.Create(Size);
        Cocos2D.ArrayPool<int>.Free(array);
        return array.Length;
    }

    [Benchmark]
    public int Shared_RentReturn()
    {
        var pool = System.Buffers.ArrayPool<int>.Shared;
        int[] array = pool.Rent(Size);
        pool.Return(array);
        return array.Length;
    }
}
