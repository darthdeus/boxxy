using System;
using System.Threading;

namespace Boxxy.Screens
{
    class TimeoutedConsoleReader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static TimeoutedConsoleReader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                return null;
        }
    }
}