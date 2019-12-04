using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgrammingInCSharp {
    public static class ImplementMultithreadingAndAsynchronousProcessing {
        public static void Start() {
            //ParallelInvoke.Start();
            //ParallelForEach.Start();
            //ParallelFor.Start();
            //ParallelForWithParallelLoopResult.Start();
            //ParallelLinqQuery.Start();
            //Tasks.Start();
            //ContinuationTasks.Start();
            ChildTasks.Start();
        }
    }

    public static class ParallelInvoke {
        public static void Start() {
            Console.WriteLine("Parallel.Invoke started");

            Parallel.Invoke(Task1, Task2);

            Console.WriteLine("Parallel.Invoke ended\r\n");
        }

        private static void Task1() {
            Console.WriteLine("Task 1 started");
            Thread.Sleep(2000);
            Console.WriteLine("Task 1 ended");
        }

        private static void Task2() {
            Console.WriteLine("Task 2 started");
            Thread.Sleep(1000);
            Console.WriteLine("Task 2 ended");
        }
    }

    public static class ParallelForEach {
        public static void Start() {
            Console.WriteLine("Parallel.ForEach started");

            var list = Enumerable.Range(0, 15);
            Parallel.ForEach(list, item => PrintIndex(item));

            Console.WriteLine("Parallel.ForEach ended\r\n");
        }

        private static void PrintIndex(object index) {
            Console.WriteLine($"Task {index} started");
            Thread.Sleep(100);
            Console.WriteLine($"Task {index} ended");
        }
    }

    public static class ParallelFor {
        public static void Start() {
            Console.WriteLine("Parallel.For started");

            var list = Enumerable.Range(0, 15).ToArray();
            Parallel.For(0, list.Length, index => PrintIndex(list[index]));

            Console.WriteLine("Parallel.For ended\r\n");
        }

        private static void PrintIndex(object index) {
            Console.WriteLine($"Task {index} started");
            Thread.Sleep(100);
            Console.WriteLine($"Task {index} ended");
        }
    }

    public static class ParallelForWithParallelLoopResult {
        public static void Start() {
            Console.WriteLine("ParallelForWithParallelLoopResult started");

            var list = Enumerable.Range(0, 15).ToArray();

            Console.WriteLine("With Stop:");
            ParallelLoopResult result = Parallel.For(0, list.Length, (int index, ParallelLoopState state) => {
                if (index == 10) state.Stop();

                PrintIndex(list[index]);
            });

            Console.WriteLine($"\r\nIsCompleted = {result.IsCompleted}, Items: {result.LowestBreakIteration}");

            Console.WriteLine("With Break:");
            result = Parallel.For(0, list.Length, (int index, ParallelLoopState state) => {
                if (index == 10) state.Break();

                PrintIndex(list[index]);
            });
            Console.WriteLine($"\r\nIsCompleted = {result.IsCompleted}, Items: {result.LowestBreakIteration}");

            Console.WriteLine("ParallelForWithParallelLoopResult ended\r\n");
        }

        private static void PrintIndex(object index) {
            Console.WriteLine($"Task {index} started");
            Thread.Sleep(100);
            Console.WriteLine($"Task {index} ended");
        }
    }

    public static class ParallelLinqQuery {
        class Person { public string Name { get; set; } public string City { get; set; } }

        public static void Start() {
            Console.WriteLine("# ParallelLinqQuery started");

            Person[] people = new Person[] { new Person { Name = "Alan", City = "Hull" }, new Person { Name = "Beryl", City = "Seattle" }, new Person { Name = "Charles", City = "London" }, new Person { Name = "David", City = "Seattle" }, new Person { Name = "Eddy", City = "Paris" }, new Person { Name = "Fred", City = "Berlin" }, new Person { Name = "Gordon", City = "Hull" }, new Person { Name = "Henry", City = "Seattle" }, new Person { Name = "Isaac", City = "Seattle" }, new Person { Name = "James", City = "London" } };

            Console.WriteLine("- Simple foreach");
            foreach (var person in people) Console.WriteLine(person.Name);

            Console.WriteLine("- Parallel LINQ query");

            var result = from person in people.AsParallel()
                         .WithDegreeOfParallelism(4)
                         .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         where person.Name.ToLower().Contains("a")
                         select person.Name;

            foreach (var personName in result) Console.WriteLine(personName);

            Console.WriteLine("- Ordered Parallel LINQ query");

            result = from person in people.AsParallel()
                         .AsOrdered()
                         .WithDegreeOfParallelism(4)
                         .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                     where person.Name.ToLower().Contains("a")
                     select person.Name;

            foreach (var personName in result) Console.WriteLine(personName);

            Console.WriteLine("- Parallel LINQ query with AsSequential");

            var listResult = (from person in people.AsParallel()
                         .AsOrdered()
                         .WithDegreeOfParallelism(4)
                         .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                              where person.Name.ToLower().Contains("a")
                              orderby person.Name
                              select person.Name)
                     .AsSequential()
                     .Take(2);

            foreach (var personName in listResult) Console.WriteLine(personName);

            Console.WriteLine("- Parallel LINQ query with ForAll on the result");

            result = from person in people.AsParallel()
                         .AsOrdered()
                         .WithDegreeOfParallelism(4)
                         .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                      where person.Name.ToLower().Contains("a")
                      orderby person.Name
                      select person.Name;

            result.ForAll(personName => Console.WriteLine(personName));

            Console.WriteLine("# ParallelLinqQuery ended");
        }
    }

    public static class Tasks {
        public static void Start() {
            Console.WriteLine("# Tasks started");

            Console.WriteLine("- With start and wait");
            Task task = new Task(() => Task1());
            task.Start();
            task.Wait();

            Console.WriteLine("- With .Run");
            task = Task.Run(() => Task1());
            task.Wait();

            Console.WriteLine("- With return result");
            Task<int> taskWithReturn = Task.Run(() => TaskWithReturn(66));
            Console.WriteLine($"Result: {taskWithReturn.Result}");

            Console.WriteLine("- With .WaitAll");
            var tasks = new Task[15];
            foreach (var taskIndex in Enumerable.Range(0, 15)) {
                int indexOfTask = taskIndex;

                tasks[indexOfTask] = Task.Run(() => TaskWithReturn(indexOfTask));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("# Tasks ended\r\n");
        }

        private static void Task1() {
            Console.WriteLine("Task 1 started");
            Thread.Sleep(2000);
            Console.WriteLine("Task 1 ended");
        }

        private static int TaskWithReturn(int ret) {
            Console.WriteLine($"TaskWithReturn [{ret}] started");
            Thread.Sleep(2000);
            Console.WriteLine($"TaskWithReturn [{ret}] is returning");
            return ret;
        }
    }

    public static class ContinuationTasks {
        public static void Start() {
            Console.WriteLine("# ContinuationTasks started\r\n");

            var task = Task.Run(() => Task1(3));
            var secondTask = task.ContinueWith((previousTask) => Task2(previousTask.Result));

            Console.WriteLine($"Result: {secondTask.Result}");

            Console.WriteLine("- With OnlyOnFaulted");
            task = Task.Run(() => Task1(0));
            secondTask = task.ContinueWith((previousTask) => Task2(previousTask.Result), TaskContinuationOptions.OnlyOnFaulted);
            task.Wait();
            Console.WriteLine($"SecondTask IsCanceled: {secondTask.IsCanceled}");

            Console.WriteLine("- With OnlyOnRanToCompletion");
            task = Task.Run(() => Task1(1));
            secondTask = task.ContinueWith((previousTask) => Task2(previousTask.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

            Console.WriteLine($"Result: {secondTask.Result}");
            Console.WriteLine($"SecondTask status: {secondTask.Status}");

            Console.WriteLine("# ContinuationTasks ended\r\n");
        }

        private static int Task1(int ret) {
            Console.WriteLine("Task 1 started");
            Thread.Sleep(2000);
            Console.WriteLine("Task 1 ended");
            return ret;
        }

        private static int Task2(int prev) {
            Console.WriteLine("Task 2 started");
            Thread.Sleep(2000);
            Console.WriteLine("Task 2 ended");
            return 18 / prev;
        }
    }

    public static class ChildTasks {
        public static void Start() {
            Console.WriteLine("# ChildTasks started\r\n");

            var parentTask = Task.Factory.StartNew(() => {
                Console.WriteLine("Main task started");

                for (var i = 0; i < 15; ++i) {
                    var taskIndex = i;

                    Task.Factory.StartNew(x => PrintIndex(x), taskIndex, TaskCreationOptions.AttachedToParent);
                }

                Console.WriteLine("Main task finished");
            });

            parentTask.Wait();

            Console.WriteLine("# ChildTasks ended\r\n");
        }

        private static void PrintIndex(object index) {
            Console.WriteLine($"Task {index} started");
            Thread.Sleep(100);
            Console.WriteLine($"Task {index} ended");
        }
    }
}
