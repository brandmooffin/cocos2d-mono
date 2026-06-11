using BenchmarkDotNet.Attributes;
using Cocos2D;

namespace Cocos2DMono.Benchmarks;

// Validates CCRawList's opt-in array pooling (UseArrayPool), now backed by
// System.Buffers.ArrayPool<T>. A build-up-then-Clear(true) churn returns the backing
// buffer to the shared pool, so the pooled path should allocate far less than the default
// new-array path once the pool is warm.
[MemoryDiagnoser]
public class CCRawListPoolingBenchmarks
{
    [Params(1000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int Unpooled()
    {
        var list = new CCRawList<int>(useArrayPool: false);
        for (int i = 0; i < N; i++)
            list.Add(i);
        int c = list.count;
        list.Clear(true);
        return c;
    }

    [Benchmark]
    public int Pooled()
    {
        var list = new CCRawList<int>(useArrayPool: true);
        for (int i = 0; i < N; i++)
            list.Add(i);
        int c = list.count;
        list.Clear(true); // Clear(true) -> Free() returns the buffer to the shared pool
        return c;
    }
}
