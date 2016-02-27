//--------------------------------------------------------------------------
//  <copyright file="PartialContactsUpdate.cs" company="Microsoft">
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

using System.Collections.Generic;

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.Contact
{
    using System;
    using System.Management.Automation;
    using SDK.Messages.MarketingResult;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommon.Enter, "MDMMarketingResultsUpdate")]
    public class PartialMarketingResultsUpdate : TypedCmdlet<PartialUpdateMarketingResultsRequest, PartialUpdateMarketingResultsResponse>
    {
        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Dynamics.Marketing.SDK.Model.Contact"/>.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("MarketingResultsUpdate")]
        public IEnumerable<SDK.Model.PartialUpdateMarketingResultItem> PartialMarketingResultUpdateItems { get; set; }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.PartialMarketingResultUpdateItems == null)
            {
                throw new PSArgumentNullException("PartialMarketingResultUpdateItems");
            }
            
            var request = this.NewRequest();
            request.PartialUpdateMarketingResultItems = this.PartialMarketingResultUpdateItems;

            var response = this.ProcessRequest(request);
            if (response == null)
            {
                return;
            }

            this.WriteObject(response.PartialUpdateMarketingResultResponses);
        }
    }
}