using System;

namespace ProjectWebAPI.TestClass
{
    public class LearnDelegate
    {
        private TestBoolDelegate TestBoolDelegateFunction;

        private TestDelegate TestDelegateFunction;

        private TestStringDelegate TestStringDelegateFunction;

        // 下面的委托可被用于引用任何一个带有一个单一的 int 参数的方法，并返回一个 bool 类型变量。
        public delegate bool TestBoolDelegate(int i);
        public delegate void TestDelegate();
        public delegate void TestStringDelegate(string str);
        public void Start()
        {
            // when TestBoolDelegateFunction(int) and TestDelegateFunction() run first,
            // whoever is the first to output;

            Func<int, int, int> constant = delegate (int _, int _) { return 42; };
            Console.WriteLine(constant(3, 4));  // output: 42

            static int sum(int a, int b) { return a + b; }
            // Func<int, int, int> sum = (a, b) => a + b;
            Console.WriteLine(sum(3, 4));  // output: 7

            TestDelegateFunction = delegate () { Console.WriteLine("Anonymous method"); };
            TestDelegateFunction += () => { Console.WriteLine("This is the simplest Anonymous method"); };

            TestDelegateFunction += MyTestDelegateFunction;
            TestDelegateFunction += MySecondTestDelegateFunction;
            TestDelegateFunction(); // Multicasting of a Delegate

            TestBoolDelegateFunction = MyTestBoolDelegateFunction;
            Console.WriteLine(TestBoolDelegateFunction(3));

            TestStringDelegateFunction = MyTestStringDelegateFunction;
            TryMethod(TestStringDelegateFunction);
        }

        private void MySecondTestDelegateFunction()
        {
            Console.WriteLine("MySecondTestDelegateFunction");
        }

        private bool MyTestBoolDelegateFunction(int i)
        {
            return i < 7;
        }

        private void MyTestDelegateFunction()
        {
            Console.WriteLine("MyTestDelegateFunction");
        }
        private void MyTestStringDelegateFunction(string str)
        {
            Console.WriteLine($"{str} out MyTestStringDelegateFunction");
        }

        // 该方法把委托作为参数，并使用它调用方法
        private void TryMethod(TestStringDelegate cannot)
        {
            cannot("I cannot believe");
        }
    }
}
