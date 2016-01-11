namespace Redmine.Net.Api.Logging
{
    public static class Logger
    {
        static readonly object locker = new object();
        private static ILogger logger;

        public static ILogger Current
        {
            get { return logger ?? (logger = new ConsoleLogger()); }
            private set { logger = value; }
        }

        public static void UseLogger(ILogger logger)
        {
            lock (locker)
            {
                Current = logger;
            }
        }
    }
}