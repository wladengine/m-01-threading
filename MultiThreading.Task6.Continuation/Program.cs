/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation;

class Program
{
    private enum OptionsToStart
    {
        Normal,
        ThrowException,
        Cancel
    }

    private static CancellationTokenSource _cancellationTokenSource = new();
    static void Main(string[] args)
    {
        Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
        Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
        Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
        Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
        Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
        Console.WriteLine("Demonstrate the work of the each case with console utility.");
        Console.WriteLine();

        // feel free to add your code
        Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
        const TaskContinuationOptions regardlessOptions = 
            TaskContinuationOptions.None | TaskContinuationOptions.AttachedToParent;

        Task t1 = Task.Run(() => { DoSomeWork(OptionsToStart.Normal); })
            .ContinueWith(_ => { ContinueSomeWork("with regardless AttachedToParent options"); }, regardlessOptions);
            
        t1.Wait();

        Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
        const TaskContinuationOptions nonSucceedOptions = TaskContinuationOptions.OnlyOnFaulted;
        Task t2 = Task.Factory.StartNew(() => { DoSomeWork(OptionsToStart.ThrowException); })
            .ContinueWith(_ => { ContinueSomeWork("with OnlyOnFaulted options"); }, nonSucceedOptions);
        
        t2.Wait();

        Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
        const TaskContinuationOptions nonSucceedAttachedOptions = 
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously;
        Task t3 = Task.Factory.StartNew(() => { DoSomeWork(OptionsToStart.ThrowException); })
            .ContinueWith(_ => { ContinueSomeWork("in OnlyOnFaulted | ExecuteSynchronously options"); }, nonSucceedAttachedOptions);
        
        t3.Wait();

        Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
        const TaskContinuationOptions nonSucceedOutsideThreadPoolOptions = 
            TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning;
        Task t4 = Task.Run(() =>
        {
            DoSomeWork(OptionsToStart.Cancel);
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }, _cancellationTokenSource.Token);
        Task t4Child = t4.ContinueWith(_ => { ContinueSomeWork("with OnlyOnCanceled and LongRunning options"); }, nonSucceedOutsideThreadPoolOptions);
        
        _cancellationTokenSource.Cancel();
        Thread.Sleep(1000);

        t4Child.Wait();

        Console.WriteLine("Done!");

        Console.ReadLine();
    }

    private static void DoSomeWork(OptionsToStart options)
    {
        Console.WriteLine($"Starting work in thread #{Environment.CurrentManagedThreadId}");

        switch (options)
        {
            case OptionsToStart.Normal:
            case OptionsToStart.Cancel:
                Thread.Sleep(1000);
                break;
            case OptionsToStart.ThrowException:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception in work thread #{Environment.CurrentManagedThreadId}!");
                Console.ResetColor();
                throw new ArgumentException("....");
            default:
                return;
        }
    }

    private static void ContinueSomeWork(object state)
    {
        Console.WriteLine($"Continue in thread #{Environment.CurrentManagedThreadId}");
        if (state != null)
        {
            Console.WriteLine($"Message: {state}");
        }

        Console.WriteLine();
    }
}