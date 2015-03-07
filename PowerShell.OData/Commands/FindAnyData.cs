//--------------------------------------------------------------------------
//  <copyright file="FindAnyData.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.OData.Commands
{
    using System;
    using System.Dynamic;
    using System.Management.Automation;
    using Client;

    using Microsoft.ServiceBus;

    using Newtonsoft.Json;

    /// <summary>
    /// The request data.
    /// </summary>
    [Cmdlet(VerbsCommon.Find, "MDMAnyData")]
    public class FindAnyData : ODataCmdletBase
    {
        /// <summary>
        /// Gets or sets the table name for request.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("Table", "Entity")]
        [ValidateNotNullOrEmpty]
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the fields for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "select")]
        public string Fields
        {
            get;
            set;
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            var requestBuilder = this.RequestBuilder;
            requestBuilder.TableName = this.TableName;
            
            requestBuilder.AddCustomParameters(this);
            this.WriteVerbose(requestBuilder.Query);

            // deserialize into object with template
            var template =
                new
                {
                    metadata = string.Empty,
                    value =
                        new[]
                                {
                                    new ExpandoObject()
                                }
                };

            try
            {
                var dataObj = requestBuilder.RequestOData(template);
                this.WriteObject(dataObj.value);
            }
            catch (InvalidRequestException ex)
            {
                this.WriteVerbose("Request error:" + ex.Message);
            }
        }
    }
}
