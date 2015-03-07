//--------------------------------------------------------------------------
//  <copyright file="ODataOperators.cs" company="Microsoft">
//      Copyright (c) 2015 Microsoft Corporation.
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//      THE SOFTWARE.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.Marketing.Powershell.OData.Client
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The o data operators.
    /// </summary>
    public static class ODataOperators
    {
        /// <summary>
        /// The OData operators.
        /// </summary>
        public enum FilterOperatorType
        {
            /// <summary>
            /// The Equals operator (or ‘eq’) evaluates to true if the left operand is equal to the right operand, otherwise if evaluates to false.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            Equals = 0,

            /// <summary>
            /// The Not Equals operator (or ‘ne’) evaluates to true if the left operand is not equal to the right operand, otherwise if evaluates to false.
            /// </summary>
            NotEquals = 1,

            /// <summary>
            /// The Greater Than operator (or ‘gt’) evaluates to true if the left operand is greater than the right operand, otherwise if evaluates to false.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            GreaterThan = 2,

            /// <summary>
            /// The Greater Than or Equal operator (or ‘ge’) evaluates to true if the left operand is greater than or equal to the right operand, otherwise if evaluates to false.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            GreaterThanOrEquals = 3,

            /// <summary>
            /// The Less Than operator (or ‘lt’) evaluates to true if the left operand is less than the right operand, otherwise if evaluates to false.
            /// </summary>
            LessThan = 4,

            /// <summary>
            /// The Less Than operator (or ‘le’) evaluates to true if the left operand is less than or equal to the right operand, otherwise 
            /// </summary>
            LessThanOrEquals = 5,
        }

        /// <summary>Converts OData filter operator to string.</summary>
        /// <param name="filterOperator">The operator.</param>
        /// <returns>Operator tag for query request</returns>
        public static string GetODataFilterOperatorString(FilterOperatorType filterOperator)
        {
            return FilterOperatorTags[(int)filterOperator];
        }

        private static readonly string[] FilterOperatorTags = { "eq", "ne", "gt", "ge", "lt", "le" };

        /// <summary>
        /// Gets the "And" operator between fields
        /// </summary>
        public static string AND 
        {
            get
            {
                return "and";
            } 
        }
    }
}
