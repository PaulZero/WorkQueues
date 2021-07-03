# What is this?
A .NET Standard 2.1 library for queueing things up and processing them in parallel
(if you want to anyway).

## WorkQueue
This is the only class of any value here.

```csharp
using PaulZero.WorkQueues;
using system;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class Program
{
    public static void Main(string[] args)
    {
        // Make a default work queue that will take ints and generate strings,
        // passing in the "WorkOnAge" method that'll be executed for each int
        var ageQueue = new WorkQueue<int, string>(WorkOnAge);

        // Listen to the "ResultReady" event so we know when, erm, a result
        // is ready? You can also use the "Chain" method to have the queue
        // dump the results straight into another queue.
        ageQueue.ResultReady += HandleResult;

        // Fill the queue up with garbage...
        Task.Run(() =>
        {
            foreach (var age in GenerateAges())
            {
                ageQueue.Enqueue(age, CancellationToken.None);
            }
        });

        // Just quit because this is a terrible example...
    }

    private static void HandleResult(IWorkQueueResult<string> result)
    {
        if (result.Success)
        {
            // Do something with result.Value
        }
        else
        {
            // Wallow in your failure with result.Exception
        }
    }

    private static Task<string> WorkOnAge(int age)
    {
        if (age == 18)
        {
            throw new Exception("Weird age...");
        }

        return Task.FromResult($"Your age is {age}, apparently.");
    }

    private static IEnumerable<int> GenerateAges()
    {
        for (var i = 1; i < 120; i++)
        {
            yield return i;
        }
    }
}
```