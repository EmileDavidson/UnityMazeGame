using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Toolbox.MethodExtensions
{
    public static class TypeExtentions
    {
        /// <summary>
        /// Get list of all inherited classes 
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static List<Type> GetInheritedClassesList(this Type myType)
        {
            return Assembly.GetAssembly(myType).GetTypes().Where(theType => theType.IsClass && !theType.IsAbstract && theType.IsSubclassOf(myType)).ToList();
        }

        /// <summary>
        /// Get list of all inherited classes and the base class
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static List<Type> GetInheritedClassesAndBaseClassList(this Type myType)
        {
            List<Type> list = new List<Type>();
            list.Add(myType);
            list.AddList(GetInheritedClassesList(myType));
            return list;
        }
        
        /// <summary>
        /// Get Dictonary of all inherited classes in the form (namespace + classname, type) 
        /// </summary>
        /// <param name="myType"></param>
        /// <returns>Dictonary (namespace + name, type)</returns>
        public static Dictionary<string, Type> GetInheritedClassesDict(this Type myType)
        {
            Dictionary<string, Type> typeDict = new Dictionary<string, Type>();
            List<Type> types = GetInheritedClassesList(myType);

            foreach (var type in types)
            {
                typeDict.Add(type.ToString(), type);
            }

            return typeDict;
        }
        
        /// <summary>
        /// Get Dictonary of all inherited classes and the base class in the form (namespace + classname, type) 
        /// </summary>
        /// <param name="myType"></param>
        /// <returns>Dictonary (namespace + name, type)</returns>
        public static Dictionary<string, Type> GetInheritedClassesAndBaseClassDict(this Type myType)
        {
            Dictionary<string, Type> typeDict = new Dictionary<string, Type>();
            List<Type> types = GetInheritedClassesAndBaseClassList(myType);
            
            foreach (var type in types)
            {
                typeDict.Add(type.ToString(), type);
            }
            
            return typeDict;
        }
        
        /// <summary>
        /// Get derrived classes from current class
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static List<Type> GetDerrivedClasses(this Type myType)
        {
            return (
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where myType.IsAssignableFrom(assemblyType) && assemblyType != myType
                select assemblyType).ToList();
        }
        
        public static List<string> GetInheritedClassesNames(this Type type)
        {
            List<Type> types = type.GetInheritedClassesList();
            List<string> names = new List<string>();
            foreach (var VARIABLE in types)
            {
                names.Add(VARIABLE.Name);
            }

            return names;
        } 
    }
}