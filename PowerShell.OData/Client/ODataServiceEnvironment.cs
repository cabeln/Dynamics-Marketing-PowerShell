// =====================================================================
//  <copyright file="ODataServiceEnvironment.cs" company="Microsoft">
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
    using System.Data.Services.Client;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Manages authentication with the organization web service.
    /// </summary>
    /// <typeparam name="T">
    /// Type of OData container class
    /// </typeparam>
    public class ODataServiceEnvironment<T> : ODataServiceBaseEnvironment where T : DataServiceContext
    {
        private static ODataServiceEnvironment<T> currentEnvironment;

        private ODataRequestBuilder<T> requestBuilder;
        private T odataContainer;

        /// <summary>
        /// Gets the Helper object for building the correct http request with authentication header 
        /// </summary>
        private ODataRequestBuilder<T> RequestBuilder
        {
            get { return this.requestBuilder ?? (this.requestBuilder = new ODataRequestBuilder<T>(this)); }
        }

        /// <summary>
        /// Gets the  OData container representing the OData for a specific MDM version 
        /// </summary>
        public T ODataContainer
        {
            get { return this.odataContainer ?? (this.odataContainer = this.LoadODataContainer()); }
        }

        /// <summary>
        /// Fire the request and load data asynchronously
        /// </summary>
        /// <returns>The container class for OData</returns>
        private T LoadODataContainer()
        {
            return this.IsSignedIn ? this.RequestBuilder.RetrieveODataContainer() : null;
        }

        /// <summary>
        /// The static construction method that delivers the one and only OData environment 
        /// and provides access to the MDM OData endpoint 
        /// </summary>
        /// <returns>The OData environment object</returns>
        public static ODataServiceEnvironment<T> GetEnvironment()
        {
            return currentEnvironment ?? (currentEnvironment = new ODataServiceEnvironment<T>());
        }

        /// <summary>
        /// Prevents a default instance of the class from being created. 
        /// Private constructor in order to enforce static instance from GetEnvironment 
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1642:ConstructorSummaryDocumentationMustBeginWithStandardText", Justification = "Reviewed. Suppression is OK here.")]
        private ODataServiceEnvironment()
        {
        }
    }
}