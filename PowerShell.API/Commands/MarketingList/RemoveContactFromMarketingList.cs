//--------------------------------------------------------------------------
//  <copyright file="RemoveContactFromMarketingList.cs" company="Microsoft">
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
    [Cmdlet(VerbsCommon.Remove, "MDMContactFromList")]
    public class RemoveContactFromMarketingList : BaseCmdlet
    {
        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Guid ListId { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public Guid ContactId { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("Address", "EmailAddress")]
        public string Email { get; set; }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if ((this.ListId == null) || (this.ListId == Guid.Empty))
            {
                throw new PSArgumentNullException("ListId");
            }

            if (((this.ContactId == null) || (this.ContactId == Guid.Empty)) &&
                string.IsNullOrEmpty(this.Email))
            {
                throw new PSArgumentNullException("ContactId or Email");
            }

            if (this.ContactId != Guid.Empty)
            {
                var requestProcessor = new RequestProcessor<RemoveContactFromMarketingListByIdRequest, RemoveContactFromMarketingListByIdResponse>(this);
                var request = requestProcessor.NewRequest();
                request.ContactId = this.ContactId;
                var response = requestProcessor.ProcessRequest(request);
                if (response == null)
                {
                    return;
                }

                this.WriteObject(response.SuccessfullyRemoved);
            }
            else
            {
                var requestProcessor = new RequestProcessor<RemoveContactFromMarketingListByEmailRequest, RemoveContactFromMarketingListByEmailResponse>(this);
                var request = requestProcessor.NewRequest();
                request.ContactEmail = this.Email;
                var response = requestProcessor.ProcessRequest(request);
                if (response == null)
                {
                    return;
                }

                this.WriteObject(response.SuccessfullyRemoved);
            }
        }
    }
}