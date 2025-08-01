namespace FarmHealthReport_ScheduleJob.Helpers
{
    public class ConsoleLogger
    {
        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + message);
            Console.ResetColor();
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARNING] " + message);
            Console.ResetColor();
        }

        public static void LogInfo(string message)
        {
            //Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO] " + message);
            //Console.ResetColor();
        }

        public static void LogStep(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[STEP] " + message);
            Console.ResetColor();
        }

        public static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[SUCCESS] " + message);
            Console.ResetColor();
        }

        public static void LogTitle(string title)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(title.ToUpper());
            Console.WriteLine("--------------------------------------------------");
        }
    }
}