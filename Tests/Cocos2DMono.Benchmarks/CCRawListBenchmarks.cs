using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Cocos2D;

namespace Cocos2DMono.Benchmarks;

// Defends the planned CCRawList<T> -> List<T> swap (Phase 2.2). Compares the render
// hot-path iteration - CCRawList.Elements direct array access - against
// List<T> + CollectionsMarshal.AsSpan and the plain List indexer, plus Add throughput.
// The plan's "+-5%" bar can be checked against these numbers before swapping.
[MemoryDiagnoser]
public class CCRawListBenchmarks
{
    [Params(1000)]
    public int N;

    private CCRawList<int> _rawList = null!;
    private List<int> _list = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rawList = new CCRawList<int>();
        _list = new List<int>();
        for (int i = 0; i < N; i++)
        {
            _rawList.Add(i);
            _list.Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public long Iterate_CCRawList_Elements()
    {
        long sum = 0;
        int[] elements = _rawList.Elements;
        int count = _rawList.count;
        for (int i = 0; i < count; i++)
            sum += elements[i];
        return sum;
    }

    [Benchmark]
    public long Iterate_List_AsSpan()
    {
        long sum = 0;
        var span = CollectionsMarshal.AsSpan(_list);
        for (int i = 0; i < span.Length; i++)
            sum += span[i];
        return sum;
    }

    [Benchmark]
    public long Iterate_List_Indexer()
    {
        long sum = 0;
        int count = _list.Count;
        for (int i = 0; i < count; i++)
            sum += _list[i];
        return sum;
    }

    [Benchmark]
    public CCRawList<int> Add_CCRawList()
    {
        var list = new CCRawList<int>();
        for (int i = 0; i < N; i++)
            list.Add(i);
        return list;
    }

    [Benchmark]
    public List<int> Add_List()
    {
        var list = new List<int>();
        for (int i = 0; i < N; i++)
            list.Add(i);
        return list;
    }
}
