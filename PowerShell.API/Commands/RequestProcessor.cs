//--------------------------------------------------------------------------
//  <copyright file="RequestProcessor.cs" company="Microsoft">
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
    using System.Globalization;
    using System.Management.Automation;
    using Microsoft.Dynamics.Marketing.Powershell.API.Client;
    using Microsoft.Dynamics.Marketing.Powershell.API.Exceptions;
    using Microsoft.Dynamics.Marketing.SDK.Common;
    using Microsoft.Dynamics.Marketing.SDK.Messages;

    /// <summary>
    /// Typed request processor class for any MDM command let processing MDM typed SDK requests and receiving responses
    /// </summary>
    /// <typeparam name="TReq">Type of the SDK request</typeparam>
    /// <typeparam name="TResp">Type of the SDK response</typeparam>
    internal class RequestProcessor<TReq, TResp> : object
        where TReq : SdkRequest, new()
        where TResp : SdkResponse
    {
        private readonly Cmdlet cmdlet;

        /// <summary>
        /// The maximum time to wait for a response
        /// </summary>
        public TimeSpan MaxResponseWaitTime
        {
            get; 
            set;            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessor{TReq,TResp}"/> class. 
        /// </summary>
        /// <param name="cmdlet">
        /// The command let to process requests for.
        /// </param>
        public RequestProcessor(Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");                
            }

            this.cmdlet = cmdlet;                    
            this.MaxResponseWaitTime = new TimeSpan(0, 0, 20);
        }

        /// <summary>
        /// The get command name.
        /// </summary>
        /// <returns>Name of the command for generated messages</returns>
        private string GetCommandName()
        {
            var attributes = this.cmdlet.GetType().GetCustomAttributes(typeof(CmdletAttribute), true);
            if (attributes.Length <= 0)
            {
                return string.Empty;
            }

            var att = (CmdletAttribute)attributes[0];
            return string.Format(CultureInfo.CurrentUICulture, "{0}-{1}", att.VerbName, att.NounName);
        }

        /// <summary>
        /// Creates a new instance of a request object
        /// </summary>
        /// <returns>The <see cref="TReq"/>.</returns>
        public TReq NewRequest()
        {
            return new TReq();
        }

        /// <summary>
        /// Fires a request to the MDM endpoint and receives and validates the response.
        /// </summary>
        /// <param name="request">The message id.</param>
        /// <returns>The response in the expected type.</returns>
        public TResp ProcessRequest(TReq request)
        {
            var client = Client.Instance;
            var messageId = client.Send(request.ToBrokeredMessage(Client.SessionId));
            this.cmdlet.WriteVerbose(string.Format(CultureInfo.CurrentUICulture, "{0}, MessageId:{1}", this.GetCommandName(), messageId));

            bool timedOut;
            var response = client.Receive(messageId, this.MaxResponseWaitTime, out timedOut);
            if (response == null)
            {
                this.cmdlet.WriteVerbose("no response!");
                if (timedOut)
                {
                    var errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} has timed out", this.GetCommandName());
                    this.cmdlet.WriteError(new ErrorRecord(new SdkError(errorMessage), string.Empty, ErrorCategory.OperationTimeout, null));
                }
                else
                {
                    var errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} has NO response", this.GetCommandName());
                    this.cmdlet.WriteError(new ErrorRecord(new SdkError(errorMessage), string.Empty, ErrorCategory.InvalidResult, null));
                }

                return null;
            }

            this.cmdlet.WriteVerbose("Received response as " + response.MessageId);

            var sdkResponse = SdkResponse.FromBrokeredMessage(response);

            var errorResponse = sdkResponse as SdkErrorResponse;
            if (errorResponse != null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0}, {1}", sdkResponse.Message, errorResponse.MessageDetails);
                this.cmdlet.WriteError(new ErrorRecord(new SdkError(errorMessage), string.Empty, ErrorCategory.NotSpecified, null));
            }

            var typedResponse = sdkResponse as TResp;
            if (typedResponse != null)
            {
                return typedResponse;
            }

            var exception =
                new InvalidPowerShellStateException(string.Format(CultureInfo.CurrentUICulture, "{0} has unexpected response type:{1}", this.GetCommandName(), sdkResponse.GetType().FullName));
            this.cmdlet.WriteError(new ErrorRecord(exception, string.Empty, ErrorCategory.InvalidResult, null));
            return null;
        }
    }
}
