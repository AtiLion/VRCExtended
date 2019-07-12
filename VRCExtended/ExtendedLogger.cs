using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCModLogger = VRCModLoader.VRCModLogger;

namespace VRCExtended
{
    internal static class ExtendedLogger
    {
        public static void Log(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            VRCModLogger.Log("[VRCExtended] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void Log(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            VRCModLogger.Log("[VRCExtended] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogWarning(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            VRCModLogger.Log("[VRCExtended] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void LogWarning(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            VRCModLogger.Log("[VRCExtended] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogError(string text, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            VRCModLogger.LogError("[VRCExtended] " + text);
            if (exception != null)
                VRCModLogger.LogError(exception.ToString());
            Console.ForegroundColor = oldColor;
        }
        public static void LogError(object obj, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            VRCModLogger.LogError("[VRCExtended] " + obj.ToString());
            if (exception != null)
                VRCModLogger.LogError(exception.ToString());
            Console.ForegroundColor = oldColor;
        }
    }
}
