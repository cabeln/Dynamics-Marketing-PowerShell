//--------------------------------------------------------------------------
//  <copyright file="ConnectToApi.cs" company="Microsoft">
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
    using System.Security;
    using Client;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "MDMApi")]
    public class ConnectToApi : Cmdlet
    {
        /// <summary>
        /// Gets or sets the Azure namespace.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the issuer name used to authenticate with Azure.
        /// </summary>
        [Parameter(Position = 1, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the Issuer key used to authenticate with Azure.
        /// </summary>
        [Parameter(Position = 2, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public SecureString IssuerKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the request queue.
        /// </summary>
        [Parameter(Position = 3, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string RequestQueueName { get; set; }

        /// <summary>
        /// Gets or sets the name of the response queue.
        /// </summary>
        [Parameter(Position = 4, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string ResponseQueueName { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        [Parameter(Position = 5, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string SessionId { get; set; }

        /// <summary>
        /// BeginProcessing method.
        /// </summary>
        protected override void BeginProcessing()
        {
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(this.SessionId))
            {
                this.SessionId = Guid.NewGuid().ToString("N");
            }

            try
            {
                Client.Initialize(this.SessionId, this.RequestQueueName, this.ResponseQueueName, this.Namespace, this.IssuerName, this.IssuerKey);
                this.WriteVerbose("Connected to queues. Session: " + this.SessionId);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    this.WriteVerbose(ex.InnerException.Message);
                }

                this.WriteVerbose(ex.Message);
                throw;
            }

            this.WriteObject(Client.IsInitialized);
        }

        /// <summary>
        /// EndProcessing method.
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}