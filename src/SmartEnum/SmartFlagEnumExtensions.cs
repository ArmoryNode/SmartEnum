using System;
using System.Collections.Generic;
using System.Linq;

namespace Ardalis.SmartEnum
{
    /// <summary>
    /// 
    /// </summary>
    public static class SmartFlagEnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSmartFlagEnum(this Type type) =>
            IsSmartFlagEnum(type, out var _);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericArguments"></param>
        /// <returns></returns>
        public static bool IsSmartFlagEnum(this Type type, out Type[] genericArguments)
        {
            if (type is null || type.IsAbstract || type.IsGenericTypeDefinition)
            {
                genericArguments = null;
                return false;
            }

            do
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(SmartFlagEnum<,>))
                {
                    genericArguments = type.GetGenericArguments();
                    return true;
                }

                type = type.BaseType;
            }
            while (type is not null);

            genericArguments = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="names"></param>
        /// <param name="outputEnums"></param>
        /// <returns></returns>
        public static bool TryGetFlagEnumValuesByName<TEnum, TValue>(this Dictionary<string, TEnum> dictionary, string names, out IEnumerable<TEnum> outputEnums)
            where TEnum : SmartFlagEnum<TEnum, TValue>
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            var outputList = new List<TEnum>(dictionary.Count);

            var commaSplitNameList = names.Replace(" ", "").Trim().Split(',');
            Array.Sort(commaSplitNameList);

            foreach (var enumValue in dictionary.Values)
            {
                var result = Array.BinarySearch(commaSplitNameList, enumValue.Name);
                if (result >= 0)
                {
                    outputList.Add(enumValue);
                }
            }

            if (!outputList.Any())
            {
                outputEnums = null;
                return false;
            }

            outputEnums = outputList.ToList();
            return true;
        }
    }
}
