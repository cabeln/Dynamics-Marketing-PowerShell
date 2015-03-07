//--------------------------------------------------------------------------
//  <copyright file="TypedCmdlet.cs" company="Microsoft">
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

    using Microsoft.Dynamics.Marketing.SDK.Common;

    /// <summary>
    /// Client class for communicating with Dynamics Marketing through PowerShell.
    /// </summary>
    /// <typeparam name="TReq">Type of request expected for this command let</typeparam>
    /// <typeparam name="TResp">Type of response expected for this command let</typeparam>
    public class TypedCmdlet<TReq, TResp> : BaseCmdlet where TReq : SdkRequest, new() where TResp : SdkResponse
    {
        private readonly RequestProcessor<TReq, TResp> requestProcessor; 

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
        /// Initializes a new instance of the <see cref="TypedCmdlet{TReq,TResp}"/> class. 
        /// </summary>
        public TypedCmdlet()
        {
            this.requestProcessor = new RequestProcessor<TReq, TResp>(this);
        }

        /// <summary>
        /// Creates a new instance of a request object
        /// </summary>
        /// <returns>The <see cref="TReq"/>.</returns>
        public TReq NewRequest()
        {
            return this.requestProcessor.NewRequest();
        }

        /// <summary>
        /// Fires a request to the MDM endpoint and receives and validates the response.
        /// </summary>
        /// <param name="request">The message id.</param>
        /// <returns>The response in the expected type.</returns>
        public TResp ProcessRequest(TReq request)
        {
            if (this.MaxResponseWaitTime > 0)
            {
                this.requestProcessor.MaxResponseWaitTime = new TimeSpan(0, 0, this.MaxResponseWaitTime);
            }

            return this.requestProcessor.ProcessRequest(request);
        }
    }
}
