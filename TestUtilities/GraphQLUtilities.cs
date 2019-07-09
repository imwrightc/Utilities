using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestUtilities
{
    public sealed class GraphQLUtilities
    {
        /// <summary>
        /// Construct a unique PropertyInfo array from the base object and all implemented interfaces
        /// </summary>
        /// <typeparam name="T">Base object type from which to retrieve properties</typeparam>
        /// <param name="dataObject">Generic object to use with reflection to derive all parameter names to return</param>
        /// <returns>Array of PropertyInfo object</returns>
        public static PropertyInfo[] BuildPropertyList<T>(T dataObject) where T : class
        {
            List<PropertyInfo> pis = new List<PropertyInfo>();
            Type type = typeof(T);
            pis.AddRange(type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).ToList());
            foreach (var implementation in type.GetInterfaces())
            {
                pis.AddRange(implementation.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));
            }
            return pis.Distinct().ToArray();
        }

        /// <summary>
        /// Construct a property formatted JSON object for GraphQL query return properties
        /// </summary>
        /// <typeparam name="T">Base object type from which to retrieve properties</typeparam>
        /// <param name="dataObject">Generic object to use with reflection to derive all parameter names to return</param>
        /// <returns>String value containing camelCased property names for full object</returns>
        public static string BuildGraphPropreties<T>(T dataObject) where T : class
        {
            StringBuilder sbProperties = new StringBuilder();
            sbProperties.Append("{");

            PropertyInfo[] properties = BuildPropertyList(dataObject);
            foreach (PropertyInfo property in properties)
            {
                sbProperties.Append(property.Name.ToCamelCase());
                sbProperties.Append(" ");
            }

            //Close query object
            sbProperties.Append("}");

            return sbProperties.ToString();
        }

        /// <summary>
        /// Construct a property formatted JSON object for GraphQL query parameters
        /// </summary>
        /// <param name="idPair">KeyValuePair where key is name of GraphQL query argument and value is unique identifier to be used in query</param>
        /// <param name="includeInactive">Boolean value which denotes whether to return only active records</param>
        /// <returns>String value containing
        ///     1) Empty
        ///     2) Id parameter name and value
        ///     3) Id parameter name and value, includeInactive parameter
        /// </returns>
        public static string BuildParameterList(KeyValuePair<string, string> idPair, bool includeInactive = false)
        {
            StringBuilder sbParameters = new StringBuilder();
            //Determine if value for id is null or if inactive items should be returned
            if (!String.IsNullOrEmpty(idPair.Value) || includeInactive)
            {
                //Construct the input parameters object
                sbParameters.Append("(");
                if (!String.IsNullOrEmpty(idPair.Value))
                {
                    //Append the parameter name
                    sbParameters.Append(idPair.Key);

                    //Append a JSON evaluable escaped quotation
                    sbParameters.Append(": \\\"");

                    //Append the parameter value
                    sbParameters.Append(idPair.Value);

                    //Append a JSON evaluable escaped quotation
                    sbParameters.Append("\\\"");
                }
                if (includeInactive)
                {
                    sbParameters.Append(" includeInactive: ");
                    sbParameters.Append(includeInactive.ToString().ToLower());
                }

                sbParameters.Append(")");
            }

            return sbParameters.ToString();
        }

        /// <summary>
        /// Construct a property formatted JSON object for GraphQL query parameters
        /// </summary>
        /// <typeparam name="T">Base object type from which to retrieve properties</typeparam>
        /// <param name="dataObject">Generic object to use with reflection to derive all parameter names to return</param>
        /// <param name="objectType"></param>
        /// <returns>String value containing camelCased property names associated and for non-null object properties</returns>
        public static string BuildParameterList<T>(T dataObject, string objectType) where T : class
        {
            //Assure that passed object class and GraphQL object name are not null
            if (dataObject == null || string.IsNullOrEmpty(objectType)) throw new ArgumentNullException();

            //Construct the list of all object properties
            PropertyInfo[] properties = BuildPropertyList(dataObject);

            StringBuilder sbParameters = new StringBuilder();
            sbParameters.Append("(");
            sbParameters.Append(objectType);
            sbParameters.Append(": {");

            //Iterate over the property list
            foreach (var property in properties)
            {
                //Assure that the value is not null, if so try to exclude it
                if (property.GetValue(dataObject) != null)
                {
                    //Convert the name to camelCase
                    sbParameters.Append(property.Name.ToCamelCase());
                    sbParameters.Append(": ");

                    //Determine the data type of the property value
                    switch (Type.GetTypeCode(property.GetValue(dataObject).GetType()))
                    {
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            //For raw types append the value itself
                            sbParameters.Append(property.GetValue(dataObject));
                            break;

                        case TypeCode.Boolean:
                            //Booleans are a special case, they should not be enclosed in quotes, they must also be lower case (true/false)
                            sbParameters.Append(property.GetValue(dataObject).ToString().ToLower());
                            break;

                        default:
                            //Enclose all string-like properties in escaped quotes
                            //These include: GUID, DateTime, and String
                            sbParameters.Append("\\\"");
                            sbParameters.Append(property.GetValue(dataObject));
                            sbParameters.Append("\\\"");
                            break;
                    }
                    //Make sure there is space between value and the next name
                    sbParameters.Append(" ");
                }
            }
            sbParameters.Append("})");

            return sbParameters.ToString();
        }
    }
}
