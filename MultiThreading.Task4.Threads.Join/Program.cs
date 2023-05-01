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
    private static readonly Semaphore _semaphore = new(0, 10);
    private const int MaxThreads = 15;
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

    private static void RunWithThreadPool()
    {
        Console.WriteLine("Doing work with ThreadPool...");
        Console.WriteLine($"Current thread is {Environment.CurrentManagedThreadId}");
        ThreadPool.QueueUserWorkItem(DoWorkWithPool, MaxThreads);
        for (var i = 0; i < MaxThreads; i++) // Wait for all threads to complete
        {
            _semaphore.WaitOne();
        }
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

    private static void DoWorkWithPool(object state)
    {
        Console.WriteLine($"Current state is {state}; Current thread number is {Environment.CurrentManagedThreadId}");
        state = (int)state - 1;
        _semaphore.Release();
        if ((int)state <= 0) 
            return;
        
        ThreadPool.QueueUserWorkItem(DoWorkWithPool, state);
    }
}