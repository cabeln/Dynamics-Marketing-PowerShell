//--------------------------------------------------------------------------
//  <copyright file="SendCommercialEmailMessageById.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.EmailMessage
{
    using System;
    using System.Management.Automation;
    using SDK.Messages.EmailMessage;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommunications.Send, "MDMCommercialEmailMessage")]
    public class SendCommercialEmailMessageById : Cmdlet
    {
        /// <summary>
        /// Gets or sets the ID of the email to send out
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Guid EmailMessageId { get; set; }

        /// <summary>
        /// Gets or sets the importance of the email when send out. Optional, default = Normal
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public SDK.Model.EmailMessageImportance Importance { get; set; }

        /// <summary>
        /// Gets or sets data to be send with the email. Optional.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string MessageData { get; set; }

        /// <summary>
        /// Gets or sets the sender contact ID. Mandatory.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Guid SenderId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailMessage"/> class. 
        /// Constructor
        /// </summary>
        public SendCommercialEmailMessageById()
        {
            this.Importance = SDK.Model.EmailMessageImportance.Normal;
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if ((this.EmailMessageId == null) || (this.EmailMessageId == Guid.Empty))
            {
                throw new PSArgumentNullException("EmailMessageId");
            }

            if ((this.SenderId == null) || (this.SenderId == Guid.Empty))
            {
                throw new PSArgumentNullException("SenderId");
            }

            var requestProcessor = new RequestProcessor<SendCommercialEmailMessageByIdRequest, SendCommercialEmailMessageByIdResponse>(this);
            var request = requestProcessor.NewRequest();
            request.EmailMessageId = this.EmailMessageId;
            request.SenderId = this.SenderId;
            request.MessageData = this.MessageData;
            request.Importance = this.Importance;
            this.WriteObject(requestProcessor.ProcessRequest(request));
        }

        /// <summary>
        /// EndProcessing method.
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}
