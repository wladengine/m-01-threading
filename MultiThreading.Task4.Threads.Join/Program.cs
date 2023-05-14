/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join;

class Program
{
    private static readonly Semaphore _mainSemaphore = new(initialCount: 0, maximumCount: 1);
    private static readonly Semaphore _innerSemaphore = new(initialCount: 1, maximumCount: 1);
    private const int MaxThreads = 10;
    static void Main(string[] args)
    {
        Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
        Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
        Console.WriteLine("Implement all of the following options:");
        Console.WriteLine();
        Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
        Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

        Console.WriteLine();

        // feel free to add your code
        RunWithThreadJoin();
        RunWithThreadPool();

        Console.ReadLine();
    }

    private static void RunWithThreadJoin()
    {
        Console.WriteLine("Doing work with Thread.Join()...");
        Console.WriteLine($"Current thread is {Environment.CurrentManagedThreadId}");
        var t = new Thread(DoWork);
        t.Start(MaxThreads);
        t.Join();
        Console.WriteLine("Done.");
        Console.WriteLine();
    }
    private static void DoWork(object state)
    {
        Console.WriteLine($"Current state is {state}; Current thread number is {Environment.CurrentManagedThreadId}");
        state = (int)state - 1;
        if ((int)state <= 0) 
            return;

        var t = new Thread(DoWork);
        t.Start(state);
        t.Join();
    }

    private static void RunWithThreadPool()
    {
        Console.WriteLine("Doing work with ThreadPool...");
        Console.WriteLine($"Current thread is {Environment.CurrentManagedThreadId:D2}");
        ThreadPool.QueueUserWorkItem(DoWorkWithPool, MaxThreads);
        _mainSemaphore.WaitOne();
        Console.WriteLine("Done.");
        Console.WriteLine();
    }
    private static void DoWorkWithPool(object state)
    {
        _innerSemaphore.WaitOne();
        Console.WriteLine($"Current state is {state:D2}; Current thread number is {Environment.CurrentManagedThreadId:D2}");
        state = (int)state - 1;
        if ((int)state <= 0)
        {
            _innerSemaphore.Release();
            _mainSemaphore.Release();
            return;
        }

        ThreadPool.QueueUserWorkItem(DoWorkWithPool, state);
        _innerSemaphore.Release();
    }
}