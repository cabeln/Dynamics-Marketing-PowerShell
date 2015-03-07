//--------------------------------------------------------------------------
//  <copyright file="Client.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Security;
    using ServiceBus.Messaging;

    /// <summary>
    /// Client class for communicating with Dynamics Marketing through PowerShell.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Object for locking the dictionary writes.
        /// </summary>
        private static readonly object MessagesLock = new object();

        /// <summary>
        /// Holds all received messages for any instance of the PowerShell script.
        /// </summary>
        private static readonly Dictionary<string, BrokeredMessage> ReceivedMessages = new Dictionary<string, BrokeredMessage>();

        /// <summary>
        /// Gets the currently used SessionId
        /// </summary>
        public static string SessionId { get; private set; }

        /// <summary>
        /// Gets the current client instance.
        /// </summary>
        /// <returns>The currently used instance of <see cref="Client"/>. If none have been configured, returns null.</returns>
        /// <remarks>To set the current instance, create a new instance of <see cref="Client"/>. 
        /// </remarks>
        public static Client Instance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client is initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return (Client.Instance != null);
            }
        }

        /// <summary>
        /// The used instance of <see cref="QueueClientProvider"/>
        /// </summary>
        private readonly QueueClientProvider queueClientProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class and assigns it as the current instance.
        /// </summary>
        /// <param name="sessionId">Session id used when communicating with Dynamics Marketing.</param>
        /// <param name="requestQueueName">Queue name used for SDK requests.</param>
        /// <param name="responseQueueName">Queue name used for SDK responses.</param>
        /// <param name="serviceBusNamespace">Azure service bus Namespace.</param>
        /// <param name="serviceBusIssuerName">Issuer name of the Azure service bus (for instance: Owner).</param>
        /// <param name="serviceBusIssuerKey">Issuer key of the Azure service bus corresponding to the <paramref name="serviceBusIssuerKey"/>.</param>
        public static void Initialize(
            string sessionId,
            string requestQueueName,
            string responseQueueName,
            string serviceBusNamespace,
            string serviceBusIssuerName,
            SecureString serviceBusIssuerKey)
        {
            if (Client.IsInitialized)
            {
                Client.Close();
            }

            var client = new Client(
                requestQueueName,
                responseQueueName,
                serviceBusNamespace,
                serviceBusIssuerName,
                serviceBusIssuerKey);
            Client.Instance = client;
            Client.SessionId = sessionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class and assigns it as the current instance.
        /// </summary>
        /// <param name="requestQueueName">Queue name used for SDK requests.</param>
        /// <param name="responseQueueName">Queue name used for SDK responses.</param>
        /// <param name="serviceBusNamespace">Azure service bus Namespace.</param>
        /// <param name="serviceBusIssuerName">Issuer name of the Azure service bus (for instance: Owner).</param>
        /// <param name="serviceBusIssuerKey">Issuer key of the Azure service bus corresponding to the <paramref name="serviceBusIssuerKey"/>.</param>
        private Client(string requestQueueName, string responseQueueName, string serviceBusNamespace, string serviceBusIssuerName, SecureString serviceBusIssuerKey)
        {
            this.queueClientProvider = new QueueClientProvider(requestQueueName, responseQueueName, serviceBusNamespace, serviceBusIssuerName, serviceBusIssuerKey);
        }

        /// <summary>
        /// Closes the current queue client.
        /// </summary>
        public static void Close()
        {
            lock (Client.Instance)
            {
                if (!IsInitialized)
                {
                    return;
                }

                Client.Instance.queueClientProvider.Close();
                Client.SessionId = null;
                Client.Instance = null;
            }
        }

        /// <summary>
        /// Sends a <see cref="BrokeredMessage"/> to the Azure service bus.
        /// </summary>
        /// <param name="brokeredMessage">An instance of <see cref="BrokeredMessage"/>.</param>
        /// <returns>The message Id of the sent message.</returns>
        public string Send(BrokeredMessage brokeredMessage)
        {
            this.queueClientProvider.RequestQueueClient.Send(brokeredMessage);
            return brokeredMessage.MessageId;
        }

        /// <summary>
        /// Retrieves a message from the Azure service bus request queue.
        /// </summary>
        /// <returns>An instance of <see cref="BrokeredMessage"/> or null if no messages are in the queue</returns>
        public BrokeredMessage ReceiveAny()
        {
            var receiver = this.queueClientProvider.ResponseQueueClient.AcceptMessageSession();
            var message = receiver.Receive();
            receiver.Close();

            return message;
        }

        /// <summary>
        /// Retrieve a message based on its message id.
        /// </summary>
        /// <param name="messageId">ID of the message to retrieve</param>
        /// <param name="maxResponseWaitTime">The maximum wait time for a response</param>
        /// <param name="timedOut">The operation has timed Out. </param>
        /// <returns>Message from service bus</returns>
        public BrokeredMessage Receive(string messageId, TimeSpan maxResponseWaitTime, out bool timedOut)
        {
            timedOut = false;
            var receivedMessage = GetReceivedMessage(messageId);
            if (receivedMessage != null)
            {
                return receivedMessage;
            }

            lock (this)
            {
                var receiver = this.queueClientProvider.ResponseQueueClient.AcceptMessageSession(Client.SessionId);
                while (true)
                {
                    receivedMessage = receiver.Receive(maxResponseWaitTime);
                    if (receivedMessage == null)
                    {
                        timedOut = true;
                        break;
                    }

                    if (receivedMessage.MessageId.Equals(messageId))
                    {
                        receivedMessage.Complete();
                        break;
                    }

                    receivedMessage.Complete();
                    AddReceivedMessage(receivedMessage);
                }

                receiver.Close();
            }

            return receivedMessage;
        }

        /// <summary>
        /// Returns all remaining messages that have been received and clears the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="BrokeredMessage"/>.
        /// </returns>
        public static BrokeredMessage[] GetAllRemainingreceivedMessages()
        {
            var remaining = new BrokeredMessage[ReceivedMessages.Count];
            ReceivedMessages.Values.CopyTo(remaining, 0);
            ReceivedMessages.Clear();
            return remaining;
        }

        /// <summary>
        /// Adds a received message to the dictionary for pick up
        /// </summary>
        /// <param name="brokeredMessage">Message that has been received</param>
        private static void AddReceivedMessage(BrokeredMessage brokeredMessage)
        {
            lock (Client.MessagesLock)
            {
                // it can happen that the message with that ID has already been received before!
                if (Client.ReceivedMessages.ContainsKey(brokeredMessage.MessageId))
                {
                    Client.ReceivedMessages.Remove(brokeredMessage.MessageId);
                }

                Client.ReceivedMessages.Add(brokeredMessage.MessageId, brokeredMessage);
            }
        }

        /// <summary>
        /// Tries to receive a message for a sent message ID and also looks in the dictionary of already received messages
        /// </summary>
        /// <param name="messageId">Message ID for which a response message should be received</param>
        /// <returns>The received message if found</returns>
        private static BrokeredMessage GetReceivedMessage(string messageId)
        {
            if (!Client.ReceivedMessages.ContainsKey(messageId))
            {
                return null;
            }

            lock (Client.MessagesLock)
            {
                var obj = Client.ReceivedMessages[messageId];
                Client.ReceivedMessages.Remove(messageId);
                return obj;
            }
        }
    }
}
