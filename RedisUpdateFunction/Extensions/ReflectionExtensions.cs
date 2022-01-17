using System;
using System.Reflection;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.Extensions
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Get the PropertyInfo of a desired Property
        /// </summary>
        /// <typeparam name="T">The Object type</typeparam>
        /// <param name="obj">The Object containing the Property</param>
        /// <param name="propertyName">The desired Property</param>
        /// <returns>The PropertyInfo</returns>
        private static PropertyInfo GetPropertyInfo<T>(this T obj, string propertyName) => obj.GetType().GetProperty(propertyName);

        /// <summary>
        /// Get the Value from an Object's property through Reflection
        /// </summary>
        /// <typeparam name="T">The Object type</typeparam>
        /// <param name="obj">The Object containing the Property</param>
        /// <param name="propertyName">The desired Property</param>
        /// <returns>The value of the Object's property. Null otherwise</returns>
        public static object GetValue<T>(this T obj, string propertyName) => obj.GetPropertyInfo(propertyName)?.GetValue(obj);

        /// <summary>
        /// Set value to an Object's property through Reflection
        /// </summary>
        /// <typeparam name="T">The Object type</typeparam>
        /// <param name="obj">The Object containing the property</param>
        /// <param name="propertyName">The desired Property</param>
        /// <param name="value">The value to set to the Property</param>
        public static void SetValue<T>(this T obj, string propertyName, object value) => obj.GetPropertyInfo(propertyName)?.SetValue(obj, value);

        /// <summary>
        /// Gets the Value of an Object's property by executing a Generic Method
        /// </summary>
        /// <typeparam name="T">The Object Type</typeparam>
        /// <param name="obj">The Object containing the property</param>
        /// <param name="methodName">The name of the method to execute</param>
        /// <param name="typeArguments">The Type arguments</param>
        /// <param name="parameters">The method parameters</param>
        /// <returns>The value of the Object's property.</returns>
        public static object GetValueFromMethod<T>(this T obj, string methodName, Type[] typeArguments, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName).MakeGenericMethod(typeArguments);
            return method.Invoke(obj, parameters);
        }
    }
}
