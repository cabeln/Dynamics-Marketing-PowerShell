//--------------------------------------------------------------------------
//  <copyright file="GetMDMResponseCommand.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands
{
    using System;
    using System.Management.Automation;
    using Client;
    using Helpers;
    using SDK.Common;
    using ServiceBus.Messaging;

    /// <summary>
    /// Command to get a response from the Dynamics Marketing response queue.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "MDMAnyResponse")]
    public class GetMDMAnyResponse : Cmdlet
    {
        /// <summary>
        /// BeginProcessing method.
        /// </summary>
        protected override void BeginProcessing()
        {
            ClientHelpers.EnsureClientIsInitialized();
        }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            var client = Client.GetClient();
            do
            {
                var response = client.ReceiveAny();
                if (response == null)
                {
                    this.WriteVerbose("no response!");
                    Console.WriteLine("no response!");
                    break;
                }

                var log = response.ToString() + " [" + response.EnqueuedTimeUtc.ToLongTimeString() + "]";
                Console.WriteLine(log);
                //response.
                var sdkResponse = SdkResponse.FromBrokeredMessage(response);
                if (sdkResponse != null)
                {
                    Console.WriteLine(sdkResponse.Message);
                }
                Console.WriteLine("");
            } while (true);

        }

        /// <summary>
        /// EndProcessing method.
        /// </summary>
        protected override void EndProcessing()
        {
        }
    }
}