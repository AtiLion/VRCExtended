using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.Events;

namespace VRCExtended.Obfuscation
{
    internal class UnityActionInternal<T>
    {
        #region Reflection Properties
        private static MethodInfo MethodAdd { get; set; }
        private static MethodInfo MethodExecute { get; set; }
        private static MethodInfo MethodRemove { get; set; }

        private object Instance { get; set; }
        #endregion

        public UnityActionInternal(Type type, object instance)
        {
            Instance = instance;

            if (MethodAdd != null && MethodExecute != null && MethodRemove != null)
                return;
            MethodInfo[] addRemoveMethods = type.GetMethods().Where(a => a.GetParameters().Length > 0 && a.GetParameters()[0].ParameterType == typeof(UnityAction<T>)).ToArray();
            if (addRemoveMethods.Length < 2)
            {
                ExtendedLogger.LogError("Failed to find required UnityActionInternal functions for type: " + type.Name + "!");
                return;
            }

            if (addRemoveMethods[0].GetMethodBody().GetILAsByteArray().Length > addRemoveMethods[1].GetMethodBody().GetILAsByteArray().Length)
            {
                MethodAdd = addRemoveMethods[0];
                MethodRemove = addRemoveMethods[1];
            }
            else
            {
                MethodAdd = addRemoveMethods[1];
                MethodRemove = addRemoveMethods[0];
            }
            MethodExecute = type.GetMethods().First(a => a.GetParameters()[0].ParameterType == typeof(T));

            ExtendedLogger.Log("Found Execute method in " + type.Name + " with name: " + MethodExecute.Name + "!");
            ExtendedLogger.Log("Found Add method in " + type.Name + " with name: " + MethodAdd.Name + "!");
            ExtendedLogger.Log("Found Remove method in " + type.Name + " with name: " + MethodRemove.Name + "!");
        }

        #region UnityActionInternal Functions
        public void Add(UnityAction<T> action)
        {
            if (Instance == null)
                return;

            MethodAdd?.Invoke(Instance, new object[] { action });
        }
        public void Remove(UnityAction<T> action)
        {
            if (Instance == null)
                return;

            MethodRemove?.Invoke(Instance, new object[] { action });
        }

        public void Execute(T val)
        {
            if (Instance == null)
                return;

            MethodExecute?.Invoke(Instance, new object[] { val });
        }
        #endregion
    }
}
