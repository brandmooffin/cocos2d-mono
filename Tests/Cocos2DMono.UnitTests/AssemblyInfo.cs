using Xunit;

// These tests share process-global singleton state - the CCDirector.SharedDirector instance
// (with its ActionManager / Scheduler) and static caches. The director's lazy init is now
// thread-safe (backed by Lazy<CCDirector>), but the shared mutable state it hands out is not:
// under xUnit's default per-class parallelism, tests in different classes would mutate the
// same director / action manager concurrently. The engine is single-threaded by design (one
// game loop owns the director), so we serialize the test run rather than the engine.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
