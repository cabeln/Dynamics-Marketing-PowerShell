//--------------------------------------------------------------------------
//  <copyright file="ODataCmdletBase.cs" company="Microsoft">
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
    using System.Management.Automation;

    /// <summary>
    /// Base command let for finding records over OData.
    /// </summary>
    public class ODataCmdletBase : StopwatchCmdlet
    {
        /// <summary>
        /// Gets or sets the filter for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "filter")]
        public string Filter
        {
            get;
            set;
        }

        // 'Search' not supported yet by MDM OData endpoint
        // <summary>
        // Gets or sets the filter for the request.
        // </summary>
        // [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        // [ODataQueryParam(Name = "search")]
        // public string Search
        // {
        //     get;
        //     set;
        // }

        // 'Count' not supported yet by MDM OData endpoint
        // <summary>
        // Gets or sets the filter for the request.
        // </summary>
        // [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        // [ODataQueryParam(Name = "count")]
        // public bool? Count
        // {
        //     get;
        //     set;
        // }

        /// <summary>
        /// Gets or sets the filter for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "orderby")]
        [Alias("Order", "Sort")]
        public string OrderBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "expand")]
        public string Expand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top number of contacts to get.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "top")]
        public int? Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top number of contacts to get.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "skip")]
        public int? Skip
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataCmdletBase"/> class.
        /// </summary>
        public ODataCmdletBase()
        {
            this.Top = 10;
        }

        private ODataRequestBuilder<DynamicDataServiceContext> requestBuilder;

        /// <summary>
        /// Gets the request builder.
        /// </summary>
        protected ODataRequestBuilder<DynamicDataServiceContext> RequestBuilder
        {
            get
            {
                return this.requestBuilder
                       ?? (this.requestBuilder = new ODataRequestBuilder<DynamicDataServiceContext>());
            }
        }
    }
}
