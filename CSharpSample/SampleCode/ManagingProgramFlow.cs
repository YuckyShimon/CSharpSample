﻿//-----------------------------------------------------------------------
// <copyright file="ManagingProgramFlow.cs" company="CVLab">
//      Copyright(c) cv-lab.com.All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CSharpSample.SampleCode
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using CVL.Extentions;
    using DesignPattern.Factory;

    /// <summary>
    /// サブスレッド
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "For Japanese support")]
    public class ManagingProgramFlow : ISamplePractitioner
{
        /// <summary>
        /// スレッド内のLocal Field
        /// </summary>
        [ThreadStatic]
        private static int field;

        /// <summary>
        /// スレッド内のLocal Field
        /// </summary>
        private static ThreadLocal<int> threadLocalField = new ThreadLocal<int>(() => { return Thread.CurrentThread.ManagedThreadId; });

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ManagingProgramFlow() { }

        /// <summary>
        /// 唯一のインスタンス
        /// </summary>
        private static ManagingProgramFlow Instance { get; set; } = new ManagingProgramFlow();

        /// <summary>
        /// Instatnce取得
        /// </summary>
        /// <returns>ManagingProgramFlowクラスのインスタンス</returns>
        public static ManagingProgramFlow GetInstance() => Instance;

        /// <summary>
        /// ドキュメントを非同期で取得
        /// </summary>
        /// <returns>指定したサイトの文字列</returns>
        public static async Task<string> DownloadContent()
        {
            using (HttpClient client = new HttpClient())
            {
                string result = await client.GetStringAsync("http://www.micorsoft.com");
                return result;
            }
        }

        /// <summary>
        /// 数値の偶数判定
        /// </summary>
        /// <param name="i">数値</param>
        /// <returns>偶数の場合true。10の倍数とその他はfalse</returns>
        public static bool IsEven(int i)
        {
            if (i % 10 == 0) { throw new ArgumentException("i"); }
            return i % 2 == 0;
        }

        /// <summary>
        /// Example 1.1 マルチスレッドの例
        /// </summary>
        public void MultithreadingAndAsynchronuous()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start();

            Enumerable.Range(0, 30).ForEach(x =>
            {
                Console.WriteLine($"Main Thread Do Some Work.{x}");
                Thread.Sleep(0);
            });

            t.Join();
        }

        /// <summary>
        /// Example 1.2 バックグラウンドスレッドの例
        /// </summary>
        public void BackgroundThread()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));

            // IsBackground = trueの場合、スレッドは、バックグランドで処理される
            // IsBackground = falseの場合、スレッドは、フォアグランドで処理される
            t.IsBackground = false;
            t.Start();
        }

        /// <summary>
        /// Example 1.3 スレッドにパラメータを渡す例
        /// </summary>
        public void ParameterizedThreadStart()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadMethod));
            t.Start(10);
            t.Join();
        }

        /// <summary>
        /// Example 1.4 スレッドの停止
        /// </summary>
        public void StoppingAThread()
        {
            bool stopped = false;

            Thread t = new Thread(new ThreadStart(() => 
            {
                while (!stopped)
                {
                    Console.WriteLine("Running...");
                    Thread.Sleep(500);
                }
            }));

            t.Start();

            Console.WriteLine("Please any key to exit");
            Console.ReadKey();

            stopped = true;
            t.Join();
        }

        /// <summary>
        /// Example 1.5 Static Attributeの使用例
        /// ラムダ式を使ったスレッドで、内部で利用するフィールドが共有されない設定
        /// </summary>
        public void StaticAttribute()
        {
            new Thread(() => 
            {
                Enumerable.Range(0, 10).ForEach(x => 
                {
                    field++;
                    Console.WriteLine($"Thread A : field={field}");
                });
            }).Start();

            new Thread(() => 
            {
                Enumerable.Range(0, 10).ForEach(x => 
                {
                    field++;
                    Console.WriteLine($"Thread B : field={field}");
                });
            }).Start();

            Console.ReadKey();
        }

        /// <summary>
        /// Example 1.6 ThreadLocalの使用例
        /// </summary>
        public void ThreadLocal()
        {
            new Thread(() => 
            {
                Enumerable.Range(0, threadLocalField.Value).ForEach(x => 
                {
                    Console.WriteLine($"Thread A : field={x}");
                });
            }).Start();

            new Thread(() => 
            {
                Enumerable.Range(0, threadLocalField.Value).ForEach(x => 
                {
                    Console.WriteLine($"Thread B : field={x}");
                });
            }).Start();

            Console.ReadKey();
        }

        /// <summary>
        /// Example 1.7 ThreadPoolに処理を登録する例
        /// </summary>
        public void ThreadPools()
        {
            ThreadPool.QueueUserWorkItem((s) => 
            {
                Console.WriteLine("Working on a thread from threadpool");
            });

            Console.ReadLine();
        }

        /// <summary>
        /// Example 1.8 Taskの実行例
        /// </summary>
        public void NewTask()
        {
            Task t = Task.Run(() => 
            {
                Enumerable.Range(0, 40).ForEach(x => 
                {
                    Console.Write("*");
                });
                Console.WriteLine(string.Empty);
            });

            t.Wait();
        }

        /// <summary>
        /// Example 1.9 戻り値を持つTaskクラスの使用例
        /// </summary>
        public void TaskThatReturnValue()
        {
            Task<int> t = Task.Run(() => { return 1024; });
            Console.WriteLine($"結果:{t.Result}");
        }

        /// <summary>
        /// Example 1.10-11 Taskを継続する(Taskが成功した場合、失敗した場合の例)
        /// </summary>
        public void TaskContinueWith()
        {
            // 前段のTaskが成功したとこのみ実行される
            Task<int> t = Task.Run(() => { return 1024; });
            t.ContinueWith(
                _t => { Console.WriteLine($"Result:{_t.Result}"); },
                TaskContinuationOptions.OnlyOnRanToCompletion);

            // 前段のTaskでハンドルされない例外が発生した時のみ実行される
            Task t2 = Task.Run(() => { throw new Exception(); });
            t2.ContinueWith(
                _t => { Console.WriteLine($"Occured Exception"); },
               TaskContinuationOptions.OnlyOnFaulted);

            Console.ReadKey();
        }

        /// <summary>
        /// Example 1.12 子タスクを親タスクに関連付け
        /// </summary>
        public void AttachingChildTasksToParentTask()
        {
            Task<int[]> parent = Task.Run(() => 
            {
                var results = new int[3];

                new Task(() => { results[0] = 0; }, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => { results[1] = 1; }, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => { results[2] = 2; }, TaskCreationOptions.AttachedToParent).Start();
                return results;
            });

            var finalTask = parent.ContinueWith(parentTask => 
            {
                parentTask.Result.ForEach(x => Console.WriteLine($"Parent Task Result is {x}"));
            });

            finalTask.Wait();
        }

        /// <summary>
        /// Example 1.13 Task Factoryの利用例
        /// </summary>
        public void TaskFactoryClass()
        {
            Task<int[]> parent = Task.Run(() =>
            {
                var results = new int[3];

                TaskFactory tf = new TaskFactory(
                    TaskCreationOptions.AttachedToParent,
                    TaskContinuationOptions.ExecuteSynchronously);

                tf.StartNew(() => results[0] = 0);
                tf.StartNew(() => results[1] = 1);
                tf.StartNew(() => results[2] = 2);

                return results;
            });

            var finalTask = parent.ContinueWith(parentTask => 
            {
                parentTask.Result.ForEach(x => Console.WriteLine($"Parent Task Result is {x}"));
            });

            finalTask.Wait();
        }

        /// <summary>
        /// Example 1.14 Task.WaitAllの利用例
        /// </summary>
        public void TaskWaitAll()
        {
            Task[] tasks = new Task[3];

            tasks[0] = Task.Run(() => 
            {
                Thread.Sleep(500);
                Console.WriteLine("Thread A:1");
                return 1;
            });
            tasks[1] = Task.Run(() => 
            {
                Thread.Sleep(500);
                Console.WriteLine("Thread B:2");
                return 2;
            });
            tasks[2] = Task.Run(() => 
            {
                Thread.Sleep(500);
                Console.WriteLine("Thread C:3");
                return 3;
            });

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Example 1.15 Task.WaitAnyの利用例
        /// </summary>
        public void TaskWaitAny()
        {
            Task<int>[] tasks = new Task<int>[3];

            tasks[0] = Task.Run(() => { Thread.Sleep(2000); return 1; });
            tasks[1] = Task.Run(() => { Thread.Sleep(1000); return 2; });
            tasks[2] = Task.Run(() => { Thread.Sleep(3000); return 3; });

            while (tasks.Length > 0)
            {
                // WaitAnyは完了したTaskのindexを戻す
                int i = Task.WaitAny(tasks);
                Task<int> completedTask = tasks[i];

                Console.WriteLine($"Completed Task Result:{completedTask.Result}");

                var tmp = tasks.ToList();
                tmp.RemoveAt(i);
                tasks = tmp.ToArray();
            }
        }

        /// <summary>
        /// Example 1.16 Parallel.ForとForeachの利用例
        /// </summary>
        public void ParallelForAndForeach()
        {
            Console.WriteLine("Example Parallel.For()");
            Parallel.For(0, 10, x => { Console.WriteLine($"Running {x}"); });

            Console.WriteLine("Example Parallel.ForEach()");
            var range = Enumerable.Range(0, 10);
            Parallel.ForEach(range, x => { Console.WriteLine($"Running {x}"); });
        }

        /// <summary>
        /// Example 1.17 Parallel.Breakの利用例
        /// </summary>
        public void ParallelBreak()
        {
            ParallelLoopResult result = Parallel.For(
                0,
                100,
                (int x, ParallelLoopState loopState) =>
                {
                    if (x == 50)
                    {
                        Console.WriteLine($"Parallel Loop will break at {x}");
                        loopState.Break();
                    }
                });
        }

        /// <summary>
        /// Example 1.18 簡単な非同期メソッド(async)の利用例
        /// </summary>
        public void SimpleExampleOfAsynchronousMethod()
        {
            string result = DownloadContent().Result;
            Console.WriteLine(result);
        }

        /// <summary>
        /// Example 1.22/23 Linqを並列動作させる例
        /// </summary>
        public void AsParallel()
        {
            var numbers = Enumerable.Range(0, 1000);
            var parallelResult = numbers.AsParallel()
                .Where(x => x % 2 == 0)
                .ToArray();

            // 表示順番は保証されている訳ではない
            parallelResult.ForEach(x =>
            {
                Console.WriteLine($"Result : {x}");
            });
        }

        /// <summary>
        /// Example 1.24 Linqを並列動作させ結果をソートする例
        /// </summary>
        public void AsParallelAsOrdered()
        {
            var numbers = Enumerable.Range(0, 1000);
            var parallelResult = numbers.AsParallel().AsOrdered()
                .Where(x => x % 2 == 0)
                .ToArray();

            // 表示順番は保証されている訳ではない
            parallelResult.ForEach(x =>
            {
                Console.WriteLine($"Result : {x}");
            });
        }

        /// <summary>
        /// Example 1.26,27 PLING時の例外
        /// </summary>
        public void ForAllAggregateException()
        {
            var numbers = Enumerable.Range(0, 20);

            try
            {
                var parallelResult = numbers
                    .AsParallel()
                    .Where(x => IsEven(x));

                parallelResult.ForAll(x => Console.WriteLine(x));
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"There were {e.InnerExceptions.Count()} Exception");
            }
        }

        /// <summary>
        /// Example 1.28 BlockingCollectionの使用例
        /// </summary>
        public void BlockingCollection()
        {
            var blockingCollection = new BlockingCollection<string>();

            Task read = Task.Run(() => 
            {
                while (true)
                {
                    Console.WriteLine(blockingCollection.Take());
                }
            });

            Task write = Task.Run(() => 
            {
                while (true)
                {
                    var s = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(s)) { break; }
                    blockingCollection.Add(s);
                }
            });
            write.Wait();            
        }

        /// <summary>
        /// Example 1.30 ConcurrentBagの使用例
        /// </summary>
        public void ConcurrentBag()
        {
            var bag = new ConcurrentBag<int>();

            bag.Add(23);
            bag.Add(41);

            int result;
            if (bag.TryTake(out result))
            {
                Console.WriteLine(result);
            }

            if (bag.TryPeek(out result))
            {
                Console.WriteLine($"There is next item : {result}");
            }
        }

        /// <summary>
        /// Example 1.32 ConcurrentStackの使用例
        /// </summary>
        public void ConcurrentStack()
        {
            var concurrentStack = new ConcurrentStack<int>();

            // Push-Pop
            concurrentStack.Push(32);

            int result;
            if (concurrentStack.TryPop(out result))
            {
                Console.WriteLine($"Poped : {result}");
            }

            // PushRange-PopRange
            concurrentStack.PushRange(new[] { 1, 2, 3 });

            int[] results = new int[3];
            concurrentStack.TryPopRange(results);
            foreach (int item in results)
            {
                Console.WriteLine($"PopRange : {item}");
            }
        }

        /// <summary>
        /// Example 1.33 ConcurrentQueueの使用例
        /// </summary>
        public void ConcurrentQueue()
        {
            var concurrentQueue = new ConcurrentQueue<int>();
            concurrentQueue.Enqueue(45);

            int result;
            if (concurrentQueue.TryDequeue(out result))
            {
                Console.WriteLine($"Dequeue : {result}");
            }
        }

        /// <summary>
        /// Example 1.34 ConcurrentDictionaryの使用例
        /// </summary>
        public void ConcurrentDictionary()
        {
            var dict = new ConcurrentDictionary<string, int>();

            if (dict.TryAdd("k1", 42))
            {
                Console.WriteLine("Added");
            }

            if (dict.TryUpdate("k1", 51, 42))
            {
                Console.WriteLine("42 Update to 51");
            }

            int r1 = dict.AddOrUpdate("k1", 51, (k, v) => v * 2);
            Console.WriteLine($"dict[k1] = {dict["k1"]}");

            int r2_1 = dict.GetOrAdd("k1", 103);
            int r2_2 = dict.GetOrAdd("k2", 103);
            Console.WriteLine($"dict[k1] = {dict["k1"]}");
            Console.WriteLine($"dict[k2] = {dict["k2"]}");
        }

        /// <summary>
        /// Sampleの実行
        /// </summary>
        /// <param name="exampleNo">例番号</param>
        /// <returns>実行結果</returns>
        public bool Do(int exampleNo)
        {
            switch (exampleNo)
            {
                case 1:
                    // Exp 1.1
                    MultithreadingAndAsynchronuous();
                    break;

                case 2:
                    // Exp 1.2
                    BackgroundThread();
                    break;

                case 3:
                    // Exp 1.3
                    ParameterizedThreadStart();
                    break;

                case 4:
                    // Exp 1.4
                    StoppingAThread();
                    break;

                case 5:
                    // Exp 1.5
                    StaticAttribute();
                    break;

                case 6:
                    // Exp 1.6
                    ThreadLocal();
                    break;

                case 7:
                    // Exp 1.7
                    ThreadPools();
                    break;

                case 8:
                    // Exp 1.8
                    NewTask();
                    break;

                case 9:
                    // Exp 1.9
                    TaskThatReturnValue();
                    break;

                case 10:
                case 11:
                    // Exp 1.10-11
                    TaskContinueWith();
                    break;

                case 12:
                    // Exp 1.12
                    AttachingChildTasksToParentTask();
                    break;

                case 13:
                    // Exp 1.13
                    TaskFactoryClass();
                    break;

                case 14:
                    // Exp 1.14
                    TaskWaitAll();
                    break;

                case 15:
                    // Exp 1.15
                    TaskWaitAny();
                    break;

                case 16:
                    // Exp 1.16
                    ParallelForAndForeach();
                    break;

                case 17:
                    // Exp 1.17
                    ParallelBreak();
                    break;

                case 18:
                    // Exp 1.18
                    SimpleExampleOfAsynchronousMethod();
                    break;

                case 22:
                case 23:
                    // Exp 1.22-23
                    AsParallel();
                    break;

                case 24:
                    // Exp 1.24
                    AsParallelAsOrdered();
                    break;

                case 26:
                case 27:
                    // Exp 1.26-27
                    ForAllAggregateException();
                    break;

                case 28:
                    // Exp 1.28
                    BlockingCollection();
                    break;

                case 30:
                    // Exp 1.30
                    ConcurrentBag();
                    break;

                case 32:
                    // Exp 1.32
                    ConcurrentStack();
                    break;

                case 33:
                    // Exp 1.33
                    ConcurrentQueue();
                    break;

                case 34:
                    // Exp 1.34
                    ConcurrentDictionary();
                    break;

                case 19:
                case 20:
                case 21:
                case 25:
                case 29:
                case 31:
                default:
                    // Exp 1.19-21 is skipped
                    // Exp 1.25 is skipped
                    // Exp 1.29 is skipped
                    // Exp 1.31 is skipped
                    throw new NotImplementedException();
            }

            return true;
        }

        /// <summary>
        /// 30回コンソールに文字列を表示するサブスレッド
        /// </summary>
        private void ThreadMethod()
        {
            Enumerable.Range(0, 30).ForEach(x =>
            {
                Console.WriteLine($"ThreadProc{x}");
                Thread.Sleep(0);
            });
        }

        /// <summary>
        /// 指定回数コンソールに文字列を表示するサブスレッド
        /// </summary>
        /// <param name="numLoop">コンソールに表示する回数</param>
        private void ThreadMethod(object numLoop)
        {
            int numWriteLine = (int)numLoop;
            Enumerable.Range(0, numWriteLine).ForEach(x =>
            {
                Console.WriteLine($"ThreadProc{x}");
                Thread.Sleep(0);
            });
        }
    }
}
