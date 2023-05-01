/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining;

class Program
{
    private static readonly Random _random = new();
    static void Main(string[] args)
    {
        Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
        Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
        Console.WriteLine("First Task – creates an array of 10 _random integer.");
        Console.WriteLine("Second Task – multiplies this array with another _random integer.");
        Console.WriteLine("Third Task – sorts this array by ascending.");
        Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
        Console.WriteLine();

        // feel free to add your code
        Console.WriteLine($"Main thread number: {Environment.CurrentManagedThreadId}");

        Task.Factory.StartNew(CreateArray)
            .ContinueWith(task => MultiplyArray(task.Result))
            .ContinueWith(task => SortArray(task.Result))
            .ContinueWith(task => CalculateAverage(task.Result))
            .Wait();

        Console.ReadLine();
    }

    private static int[] CreateArray()
    {
        Console.WriteLine($"Current thread number: {Environment.CurrentManagedThreadId}");
        Console.WriteLine("Creating array:");
        int[] array = Enumerable.Repeat(1, 10).Select(x => _random.Next()).ToArray();
        foreach (int val in array)
        {
            Console.WriteLine(val);
        }
        return array;
    }

    private static int[] MultiplyArray(int[] input)
    {
        Console.WriteLine();
        Console.WriteLine($"Current thread number: {Environment.CurrentManagedThreadId}");
        Console.WriteLine("Multiply array:");
        int[] output = input.Select(x => x * _random.Next()).ToArray();
        foreach (int val in output)
        {
            Console.WriteLine(val);
        }
        return output;
    }

    private static int[] SortArray(int[] input)
    {
        Console.WriteLine();
        Console.WriteLine($"Current thread number: { Environment.CurrentManagedThreadId }");
        Console.WriteLine("Sorting array:");
        int[] output = input.OrderBy(x => x).ToArray();
        foreach (int val in output)
        {
            Console.WriteLine(val);
        }
        return output;
    }
    private static double CalculateAverage(int[] input)
    {
        Console.WriteLine();
        Console.WriteLine($"Current thread number: {Environment.CurrentManagedThreadId}");
        Console.WriteLine("Calculating average:");
        double average = input.Average();
        Console.WriteLine($"AVG = {average}");
        return average;
    }
}