/***********************************************************
 * Credits:
 * 
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 * 
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 * 
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 * 
 * http://blogs.msdn.com/mattwar/default.aspx
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinqToTwitter
{
    internal static class TypeSystem
    {
//#if NETFX_CORE
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GenericTypeArguments[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            TypeInfo seqTypeInfo = seqType.GetTypeInfo();
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqTypeInfo.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqTypeInfo.GetElementType());

            if (seqTypeInfo.IsGenericType)
            {
                foreach (Type arg in seqTypeInfo.GenericTypeArguments)
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.GetTypeInfo().IsAssignableFrom(seqTypeInfo))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqTypeInfo.ImplementedInterfaces.ToArray();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqTypeInfo.BaseType != null && seqTypeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(seqTypeInfo.BaseType);
            }

            return null;
        }
//#else
//        internal static Type GetElementType(Type seqType)
//        {
//            Type ienum = FindIEnumerable(seqType);
//            if (ienum == null) return seqType;
//            return ienum.GetGenericArguments()[0];
//        }

//        private static Type FindIEnumerable(Type seqType)
//        {
//            if (seqType == null || seqType == typeof(string))
//                return null;

//            if (seqType.IsArray)
//                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

//            if (seqType.IsGenericType)
//            {
//                foreach (Type arg in seqType.GetGenericArguments())
//                {
//                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
//                    if (ienum.IsAssignableFrom(seqType))
//                    {
//                        return ienum;
//                    }
//                }
//            }

//            Type[] ifaces = seqType.GetInterfaces();
//            if (ifaces != null && ifaces.Length > 0)
//            {
//                foreach (Type iface in ifaces)
//                {
//                    Type ienum = FindIEnumerable(iface);
//                    if (ienum != null) return ienum;
//                }
//            }

//            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
//            {
//                return FindIEnumerable(seqType.BaseType);
//            }

//            return null;
//        }
//#endif
    }
}
