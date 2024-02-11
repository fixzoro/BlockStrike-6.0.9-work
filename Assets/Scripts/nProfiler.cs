using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

public static class nProfiler
{
    private static Stopwatch watch;

    public static void BeginSample(string name)
	{
        #if UNITY_EDITOR
        Profiler.BeginSample(name);
        #endif
    }
    
	public static void EndSample()
	{
        #if UNITY_EDITOR
        Profiler.EndSample();
        #endif
    }
    
    public static void StartWatch()
	{

	}
    
	public static long StopWatch()
	{
		return 0L;
	}
}
