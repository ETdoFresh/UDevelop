using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace CommandSystem
{
    public static class CSharp
    {
        public static object Execute(string cSharpCode, Dictionary<string, ArgData> argMemory)
        {
            if (string.IsNullOrWhiteSpace(cSharpCode)) return null;
            
            // New Example
            // new UnityEngine.GameObject({GameObject Name}, {Component Types})
            // Looks up {GameObject Name} in localRunValues and cast as localRunType lookup
            // Looks up {Component Types} in localRunValues and cast as localRunType lookup
            if (cSharpCode.StartsWith("new"))
            {
                var withoutNew = cSharpCode[3..];
                var split = withoutNew.Split('(');
                var splitClosingIndex = split[1].LastIndexOf(')');
                var typeString = split[0];
                var argsString = split[1][..splitClosingIndex];
                var argStrings = argsString.Split(',').Select(x => x.Trim()).ToArray();
                var type = StringToTypeUtility.Get(typeString);
                var argObjects = new List<object>();
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (!argMemory.TryGetValue(argString, out var arg)) continue;
                    if (arg.Required || arg.Value != null)
                        argObjects.Add(arg.Value);
                }

                return Activator.CreateInstance(type, argObjects.ToArray());
            }
            else
            {
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
                // Type: UnityEngine.GameObject
                // Function: Find
                // Args: {GameObject Name}
                var split = cSharpCode.Split('(');
                var splitClosingIndex = split[1].LastIndexOf(')');

                if (split.Length == 1)
                    return cSharpCode;

                var methodSplit = split[0].Split('.');
                var methodName = methodSplit[^1];
                var fullTypeName = methodSplit[..^1];
                var fullTypeString = string.Join('.', fullTypeName);
                var type = StringToTypeUtility.Get(fullTypeString);

                // Else run method
                var argsString = split[1][..splitClosingIndex];
                var argStrings = string.IsNullOrEmpty(argsString)
                    ? Array.Empty<string>()
                    : argsString.Split(',').Select(x => x.Trim()).ToArray();

                var self = (ArgData)null;
                if (argStrings.Length > 0 && argStrings[0].StartsWith("this"))
                {
                    var selfString = argStrings[0][4..].Trim();
                    self = argMemory[selfString];
                    argStrings = argStrings.Skip(1).ToArray();
                }

                var args = new ArgData[argStrings.Length];
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (argMemory.TryGetValue(argString, out var arg))
                        args[i] = arg;
                    else if (argString.StartsWith("\"") && argString.EndsWith("\""))
                        args[i] = new ArgData(argString[1..^1], typeof(string), argString[1..^1]);
                    else if (argString.StartsWith("'") && argString.EndsWith("'"))
                        args[i] = new ArgData(argString[1..^1], typeof(string), argString[1..^1]);
                    else if (argString == "null")
                        args[i] = new ArgData(argString, typeof(object), null);
                    else if (int.TryParse(argString, out var intResult))
                        args[i] = new ArgData(argString, typeof(int), intResult);
                    else if (float.TryParse(argString, out var floatResult))
                        args[i] = new ArgData(argString, typeof(float), floatResult);
                    else if (bool.TryParse(argString, out var boolResult))
                        args[i] = new ArgData(argString, typeof(bool), boolResult);
                    else
                        args[i] = new ArgData(argString, typeof(string), argString);
                }

                var argTypes = args.Select(x => x?.Type ?? typeof(object)).ToArray();
                var argObjects = args.Select(x => x?.Value).ToArray();
                var bindingFlags = Public | NonPublic | Instance | Static;
                // The following works pretty well, but does not work very well for generics
                // var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);

                // To make generics work, we first try above method, then try MakeGenericMethod
                var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);
                if (method == null)
                {
                    var isLinq = type.FullName.StartsWith("System.Linq");
                    if (isLinq)
                    {
                        var genericMethod = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                        var typeFromArray = (Type)null;
                        if (self != null)
                            typeFromArray = self.GetType().IsArray ? self.GetType().GetElementType() : self.GetType();
                        else if (argObjects.Length > 0)
                            typeFromArray = argTypes[0].IsArray ? argTypes[0].GetElementType() : argTypes[0];
                        method = genericMethod?.MakeGenericMethod(typeFromArray);
                    }
                    else
                    {
                        method = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                    }
                }

                if (method == null) ThrowException("Method not found!", cSharpCode, argMemory);

                try
                {
                    return method.Invoke(self?.Value, argObjects);
                }
                catch (TargetInvocationException ex)
                {
                    ThrowException(ex.InnerException, cSharpCode, argMemory);
                }
                catch (Exception ex)
                {
                    ThrowException(ex, cSharpCode, argMemory);
                }
                return null;
            }
        }
        
        private static void ThrowException(string message, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            ThrowException(new ArgumentException(message), commandString, localArgs, args);
        }
        
        private static void ThrowException(Exception ex, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            var message = ex.Message; //ex.ToString();
            if (commandString != null) message += $"\nCommand: {commandString}";

            if (localArgs != null)
            {
                message += "\n\nArg Memory:";
                foreach (var localArg in localArgs)
                    message += $"\n{localArg.Value}";
            }

            if (args != null)
            {
                message += "\n\nArgs:";
                foreach (var arg in args)
                    message += $"\n{arg}";
            }

            message += "\n\nCommand Call Trace:";
            
            throw new ArgumentException(message);
        }
    }
}