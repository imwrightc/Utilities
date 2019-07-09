using System;
using System.Collections.Generic;
using System.Text;

namespace TestUtilities
{
    public class GraphQLBuilder
    {
        /// <summary>
        /// Build a GraphQL query with defined parameters and containing all fields for return value
        /// </summary>
        /// <typeparam name="T">Base object type for which to build GraphQL query</typeparam>
        /// <param name="request">Reference to string object which will contain formatted GraphQL query</param>
        /// <param name="dataObject">Generic object to use with reflection to derive all parameter names to return</param>
        /// <param name="requestType">Name of GraphQL query to perform</param>
        /// <param name="id">KeyValuePair where key is name of GraphQL query argument and value is unique identifier to be used in query</param>
        /// <param name="includeInactive">Boolean value which denotes whether to return only active records</param>
        public static void BuildGraphRequest<T>(ref string request, T dataObject, string requestType, KeyValuePair<string, string> id, bool includeInactive = false) where T : class
        {
            //Define Type object for passed in generic
            Type type = typeof(T);

            //Validate that a query name was passed in
            if (String.IsNullOrEmpty(requestType)) throw new ArgumentNullException();

            //Instantiate StringBuilder to contain GraphQL query object
            var sbRequest = new StringBuilder();

            //Append base query object
            sbRequest.Append("{ \"query\": \"query {");

            //Append query name to execute
            sbRequest.Append(requestType);

            //Construct and append the parameter list
            sbRequest.Append(GraphQLUtilities.BuildParameterList(id, includeInactive));

            //Construct and append the property list
            sbRequest.Append(GraphQLUtilities.BuildGraphPropreties(dataObject));

            //Close query object
            sbRequest.Append("}\" }");

            //Update reference parameter
            request = sbRequest.ToString();
        }

        /// <summary>
        /// Build a GraphQL mutation with defined parameters and containing all fields for return value
        /// </summary>
        /// <typeparam name="T">Base object type for which to build GraphQL mutation</typeparam>
        /// <param name="request">Reference to string object which will contain formatted GraphQL mutation</param>
        /// <param name="dataObject">Generic object to use with reflection to derive all parameter names to return</param>
        /// <param name="mutationType">Name of GraphQL mutation to perform</param>
        /// <param name="objectType">Name of object to pass within GraphQL mutation</param>
        public static void BuildGraphMutation<T>(ref string request, T dataObject, string mutationType, string objectType) where T : class
        {
            //Define Type object for passed in generic
            Type type = typeof(T);

            //Validate that a query name was passed in
            if (String.IsNullOrEmpty(mutationType)) throw new ArgumentNullException();

            //Instantiate StringBuilder to contain GraphQL query object
            var sbMutation = new StringBuilder();

            //Append base query object
            sbMutation.Append("{ \"query\": \"mutation {");

            //Append query name to execute
            sbMutation.Append(mutationType);
            sbMutation.Append(GraphQLUtilities.BuildParameterList(dataObject, objectType));

            sbMutation.Append(GraphQLUtilities.BuildGraphPropreties(dataObject));

            //Close query object
            sbMutation.Append("}\" }");

            //Update reference parameter
            request = sbMutation.ToString();
        }
    }
}
