//--------------------------------------------------------------------------
//  <copyright file="QueueClientProvider.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.API.Client
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using ServiceBus;
    using ServiceBus.Messaging;

    /// <summary>
    /// Provides the QueueClients for the SDK request and response queues.
    /// </summary>
    public class QueueClientProvider
    {
        private readonly MessagingFactory messagingFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueClientProvider"/> class.
        /// </summary>
        /// <param name="requestQueueName">The name of the SDK request queue.</param>
        /// <param name="responseQueueName">The name of the SDK response queue.</param>
        /// <param name="serviceBusNamespace">The Service Bus namespace.</param>
        /// <param name="serviceBusIssuerName">The Service Bus issuer name.</param>
        /// <param name="serviceBusIssuerKey">The Service Bus issuer key.</param>
        public QueueClientProvider(string requestQueueName, string responseQueueName, string serviceBusNamespace, string serviceBusIssuerName, SecureString serviceBusIssuerKey)
        {
            var runtimeUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, string.Empty);
            this.messagingFactory = MessagingFactory.Create(runtimeUri, TokenProvider.CreateSharedSecretTokenProvider(serviceBusIssuerName, SecureStringToString(serviceBusIssuerKey)));

            this.RequestQueueClient = this.CreateQueueClient(requestQueueName);
            this.ResponseQueueClient = this.CreateQueueClient(responseQueueName);

            this.TestQueues();
        }

        /// <summary>
        /// Creates the QueueClient for sending and receiving over the queue.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>The QueueClient for the given queue.</returns>
        private QueueClient CreateQueueClient(string queueName)
        {
            if (this.messagingFactory == null)
            {
                throw new InvalidOperationException("Messaging Factory not initialized.");
            }

            return this.messagingFactory.CreateQueueClient(queueName);
        }

        /// <summary>
        /// Converts the content of <see cref="value"/> to it's <see cref="string" /> representation.
        /// </summary>
        /// <param name="value">An instance of <see cref="SecureString"/></param>
        /// <returns><see cref="string"/> representation of <paramref name="value"/>.</returns>
        private static string SecureStringToString(SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        /// <summary>
        /// Tests that the QueueClients are usable.
        /// </summary>
        private void TestQueues()
        {
            this.RequestQueueClient.Peek();
            this.ResponseQueueClient.Peek();
        }

        /// <summary>
        /// Closes the messaging objects
        /// </summary>
        public void Close()
        {
            this.messagingFactory.Close();
        }

        /// <summary>
        /// Gets the QueueClient for the SDK request queue.
        /// </summary>
        public QueueClient RequestQueueClient { get; private set; }

        /// <summary>
        /// Gets the QueueClient for the SDK response queue.
        /// </summary>
        public QueueClient ResponseQueueClient { get; private set; }
    }
}
