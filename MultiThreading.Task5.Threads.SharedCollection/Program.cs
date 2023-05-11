/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection;

class Program
{
    private const int CollectionLength = 10;
    private static readonly ManualResetEventSlim _producerAddedNewValue = new(false);
    private static readonly ManualResetEventSlim _producerFinishedWork = new(false);
    private static readonly ManualResetEventSlim _consumerHandledValue = new(false);
    private static readonly List<int> _collection = new();

    static void Main(string[] args)
    {
        Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
        Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
        Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
        Console.WriteLine();

        // feel free to add your code
        Task producerTask = Task.Factory.StartNew(FillCollection);
        Task consumerTask = Task.Factory.StartNew(ReadCollection);

        Task.WaitAll(producerTask, consumerTask);

        Console.ReadLine();
    }

    private static void FillCollection()
    {
        for (var i = 1; i <= CollectionLength; i++)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Producer thread #{Environment.CurrentManagedThreadId} is adding value {i}...");
            Console.WriteLine();

            _collection.Add(i);

            _producerAddedNewValue.Set();

            _consumerHandledValue.Reset();
            _consumerHandledValue.Wait();
        }

        _producerAddedNewValue.Set();
        _producerFinishedWork.Set();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Producer thread #{Environment.CurrentManagedThreadId} has finished work");
        Console.WriteLine();
    }

    private static void ReadCollection()
    {
        while (true)
        {
            _producerAddedNewValue.Wait();
            if (_producerFinishedWork.IsSet)
                break;
            
            _producerAddedNewValue.Reset();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Consumer thread #{Environment.CurrentManagedThreadId} is here!");
            Console.WriteLine("New value added! Here is the list of items:");

            foreach (int i in _collection)
            {
                Console.Write($"{i} ");
            }

            Console.WriteLine();
            Console.WriteLine();
            _consumerHandledValue.Set();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Consumer thread #{Environment.CurrentManagedThreadId} has finished work");
        Console.WriteLine();
    }
}