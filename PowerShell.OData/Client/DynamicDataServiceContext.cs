//--------------------------------------------------------------------------
//  <copyright file="DynamicDataServiceContext.cs" company="Microsoft">
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

    /// <summary>
    /// The dynamic data service context.
    /// </summary>
    public partial class DynamicDataServiceContext : DataServiceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataServiceContext"/> class.
        /// </summary>
        /// <param name="serviceRoot">
        /// The service root.
        /// </param>
        public DynamicDataServiceContext(System.Uri serviceRoot) :
            base(serviceRoot, System.Data.Services.Common.DataServiceProtocolVersion.V3)
        {
            this.OnContextCreated();
        }

        partial void OnContextCreated();
    }
}
