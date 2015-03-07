//--------------------------------------------------------------------------
//  <copyright file="CopyContactsFromMarketingList.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.MarketingList
{
    using System;
    using System.Management.Automation;
    using SDK.Messages.MarketingList;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommon.Copy, "MDMListContacts")]
    public class CopyContactsFromMarketingList : TypedCmdlet<CopyContactsFromMarketingListRequest, CopyContactsFromMarketingListResponse>
    {
        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("SourceId")]
        [ValidateNotNullOrEmpty]
        public Guid SourceListId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("TargetId")]
        [ValidateNotNullOrEmpty]
        public Guid TargetListId
        {
            get;
            set;
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.SourceListId == null || this.SourceListId == Guid.Empty)
            {
                throw new PSArgumentNullException("SourceListId");
            }

            if (this.TargetListId == null || this.TargetListId == Guid.Empty)
            {
                throw new PSArgumentNullException("TargetListId");
            }

            var request = this.NewRequest();
            request.SourceMarketingListId = this.SourceListId;
            request.TargetMarketingListId = this.TargetListId;

            var response = this.ProcessRequest(request);
            if (response == null)
            {
                return;
            }

            // !!! MDM API Design issue: Response type is untyped
            this.WriteObject(response.Message);
        }
    }
}