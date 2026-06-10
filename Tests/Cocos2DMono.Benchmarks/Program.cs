using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(System.Reflection.Assembly.GetExecutingAssembly()).Run(args);
