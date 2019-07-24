using System;

/*
 * 參考自 https://jermdavis.wordpress.com/2016/10/03/an-alternative-approach-to-pipelines/
 */

namespace SynchronousPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OK, Synchronous Pipeline!");
            new Program().Run();
        }

        void Run()
        {
            // Test 1
            int no1 = 12345;
            string msg1 = no1
                .Step(new IntToStringStep())
                .Step(new SayHelloStep());
            Console.WriteLine($"test 1 => message: {msg1}");

            // Test 2
            int no2 = 54321;
            string msg2 = new IntSayHelloPipeline().Process(no2);
            Console.WriteLine($"test 2 => message: {msg2}");
        }
    }

    /// <summary>
    /// 1) Pipeline 中的步驟
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IPipelineStep<TInput, TOutput>
    {
        TOutput Process(TInput input);
    }

    /// <summary>
    /// 2) 步驟實作: 第1步
    /// </summary>
    public class IntToStringStep : IPipelineStep<int, string>
    {
        public string Process(int input)
        {
            return input.ToString();
        }
    }

    /// <summary>
    /// 2) 步驟實作: 第2步
    /// </summary>
    public class SayHelloStep : IPipelineStep<string, string>
    {
        public string Process(string input)
        {
            return $"Hello World, I'm {input}";
        }
    }

    /// <summary>
    /// 3) 定義 Extension method, 讓任意型別都可以掛上型別符合的 Step, 並執行
    /// </summary>
    public static class PipelineStepExtensions
    {
        public static TOutput Step<TInput, TOutput>(this TInput input, IPipelineStep<TInput, TOutput> step)
        {
            return step.Process(input);
        }
    }

    /// <summary>
    /// 4) 功能包裹, 串接一系列 Step 的實作, 成為特定功能的執行體.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class Pipeline<TInput, TOutput>
    {
        public Func<TInput, TOutput> PipelineSteps { get; protected set; }

        public TOutput Process(TInput input)
        {
            return PipelineSteps(input);
        }
    }

    /// <summary>
    /// 5) Pipeline 功能實作
    /// </summary>
    public class IntSayHelloPipeline : Pipeline<int, string>
    {
        public IntSayHelloPipeline()
        {
            PipelineSteps = input => input
                .Step(new IntToStringStep())
                .Step(new SayHelloStep());
        }
    }
}
