using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCExtended.Obfuscation
{
    internal static class MethodInfoExtensions
    {
        public static bool HasPattern(this MethodInfo method, string pattern)
        {
            string[] patternSegments = pattern.Split(',');
            byte[] bytecode = method.GetMethodBody().GetILAsByteArray();

            if (patternSegments.Length > bytecode.Length)
                return false;
            for (int i = 0; i < patternSegments.Length; i++)
            {
                if (patternSegments[i] == "?")
                    continue;

                if (Convert.ToByte(patternSegments[i]) != bytecode[i])
                    return false;
            }
            return true;
        }
    }
}