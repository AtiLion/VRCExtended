using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;

namespace VRCExtended.Modules.ModuleExtensions
{
    internal static class MethodInfoExtensions
    {
        public static DynamicMethod Patch(this MethodInfo original, MethodInfo prefix) =>
            ManagerModule.Harmony.Patch(original, new HarmonyMethod(prefix));
    }
}
