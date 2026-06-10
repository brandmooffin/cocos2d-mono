using Xunit;

// The CCNode / CCAction / CCScheduler tests exercise the global CCDirector.SharedDirector
// singleton, whose lazy initializer is not thread-safe: it publishes the static instance
// (CCDirector.SharedDirector) before Init() populates ActionManager/Scheduler. Under
// xUnit's default per-class parallelism, one thread could observe the half-initialized
// director, so the cached m_pActionManager on a freshly constructed CCNode was null and
// CCNode.RunAction threw an intermittent NullReferenceException.
//
// The engine is single-threaded by design (one game loop owns the director), so we
// serialize the test run rather than the engine. A thread-safe lazy init in the library
// is handled as a separate change.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
