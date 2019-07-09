using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestUtilities
{
    public class AssertResult
    {
        /// <summary>
        /// Generic function that takes in an object and an enumerable of that object and checks to see if the item is found in the collection
        /// </summary>
        /// <typeparam name="T">The generic Type of the parameters being passed in</typeparam>
        /// <param name="expected">Single instance of T with expected values</param>
        /// <param name="actual">Enumerable list of T containing result set</param>
        /// <returns>Boolean indicating if the collection contains the item</returns>
        public static bool AssertResultContains<T>(T expected, IEnumerable<T> actual) where T : class
        {
            //Contains should function but requires a implementation of IEquatable
            return actual.Contains(expected);
        }

        /// <summary>
        /// Generic function that takes in an object and an enumerable of that object and checks to see if all properties of the initial object match one instance of the enumerable
        /// </summary>
        /// <typeparam name="T">The generic Type of the parameters being passed in</typeparam>
        /// <param name="expected">Single instance of T with expected values</param>
        /// <param name="actual">Enumerable list of T containing result set</param>
        /// <param name="excludedProps">List of string containing properties to exclude from the check, defaults to null</param>
        /// <returns>Boolean: true if all properties of the expected object match all properties of at least one item in the collection</returns>
        [Obsolete("Only use this version if you cannot implement IEquatable")]
        public static bool AssertResultContains<T>(T expected, IEnumerable<T> actual, List<string> excludedProps = null) where T : class
        {
            //Get the type of the object
            Type type = typeof(T);

            //Return false if any of the object is null
            if (expected == null || actual == null)
            {
                return false;
            }

            bool propsMatch = false;
            bool hasMatch = true;
            //Loop through each item in the result list
            foreach (var item in actual)
            {
                //Loop through each property inside class definition and get values for the property from both of the objects
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.Name != "ExtensionData" && (excludedProps == null || !excludedProps.Contains(property.Name)))
                    {
                        var expectedValue = type.GetProperty(property.Name).GetValue(expected);
                        var currentValue = type.GetProperty(property.Name).GetValue(item);
                        if (currentValue != expectedValue)
                        {
                            hasMatch = false;
                            break;
                        }
                    }
                }
                if (!hasMatch) continue;
                else
                {
                    propsMatch = hasMatch;
                    break;
                }
            }

            return propsMatch;
        }
    }
}
