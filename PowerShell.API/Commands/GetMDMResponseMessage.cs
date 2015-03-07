//--------------------------------------------------------------------------
//  <copyright file="GetMDMResponseMessage.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands
{
    using System;
    using System.Management.Automation;
    using Client;
    using SDK.Common;

    /// <summary>
    /// Command to get a response from the Dynamics Marketing response queue.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "MDMApiResponse")]
    public class GetMDMResponseMessage : BaseCmdlet
    {
        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string MessageId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("Timeout")]
        public int MaxResponseWaitTime
        {
            get;
            set;
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            var client = Client.Instance;

            var timeout = new TimeSpan(0, 0, 20);
            if (this.MaxResponseWaitTime > 0)
            {
                timeout = new TimeSpan(0, 0, this.MaxResponseWaitTime);
            }

            bool timedOut;
            var response = client.Receive(this.MessageId, timeout, out timedOut);
            if (response == null)
            {
                this.WriteVerbose("no response!");
                return;
            }

            var sdkResponse = SdkResponse.FromBrokeredMessage(response);
            this.WriteObject(sdkResponse);
        }
    }
}