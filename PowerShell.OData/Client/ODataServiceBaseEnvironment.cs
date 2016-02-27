// =====================================================================
//  <copyright file="ODataServiceBaseEnvironment.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.OData.Client
{
    using System;
    using IdentityModel.Clients.ActiveDirectory;
    /// <summary>
    /// The o data service base environment.
    /// </summary>
    public abstract class ODataServiceBaseEnvironment
    {
        // MDM Online OAuth URL.
        private const string _OAuthUrl = "https://login.windows.net/common/";

        /// <summary>
        /// MDM authentication resource name.
        /// </summary>
        private const string _OAuthTokenResourceName = "https://marketing-infra.dynamics.com/";

        //private const string _OAuthTokenResourceName = "https://AUTHREDIR-US1.MARKETING-INFRA.DYNAMICS.COM/default.aspx";

        private const string _PowerBiAppId = "a672d62c-fc7b-4e81-a576-e60dc46e951d";
        private const string _PowerBiRedirectUrl = "https://de-users-preview.sqlazurelabs.com/account/reply/";

        /// <summary>
        /// The before sign in event.
        /// </summary>
        public event EventHandler BeforeSignIn;

        /// <summary>
        /// The signed in event.
        /// </summary>
        public event EventHandler SignedIn;

        /// <summary>
        /// The signed out event.
        /// </summary>
        public event EventHandler SignedOut;

        private AuthenticationContext authenticationContext;
        private AuthenticationResult currentAuthenticationResult;

        private string odataClientAppId;
        private string mdmServiceUrl;

        private Uri redirectUrl;

        public ODataServiceBaseEnvironment()
        {
            this.OAuthUrl = _OAuthUrl;
            this.OAuthTokenResourceName = _OAuthTokenResourceName;
            this.AzureClientAppId = _PowerBiAppId;
            this.RedirectUrl = new Uri(_PowerBiRedirectUrl);
        }

        /// <summary>
        /// Gets or sets the App Id of the Azure app that is permitted to access the MDM OData service of an org
        /// </summary>
        public string AzureClientAppId
        {
            get { return this.odataClientAppId; }
            set
            {
                if (string.Equals(this.AzureClientAppId, value))
                {
                    return;
                }

                this.SignOut();
                this.odataClientAppId = value;
            }
        }

        /// <summary>
        /// Gets or sets the Service Url of an MDM org
        /// </summary>
        public string ServiceUrl
        {
            get { return this.mdmServiceUrl; }
            set
            {
                var newUrl = this.CleanseServiceUrl(value);
                if (string.Equals(this.ServiceUrl, newUrl))
                {
                    return;
                }

                this.SignOut();
                this.mdmServiceUrl = newUrl;
            }
        }

        /// <summary>
        /// Gets the Analytics Url based on a MDM Service Url 
        /// </summary>
        public string ODataServiceUrl
        {
            get
            {
                return this.ServiceUrl + "/analytics";
            }
        }

        /// <summary>
        /// Gets the Analytics Url based on a MDM Service Url 
        /// </summary>
        public string ODataServiceMetadataUrl
        {
            get
            {
                return this.ServiceUrl + "/analytics/$metadata";
            }
        }

        /// <summary>
        /// Gets or sets the  redirect URI that has been specified in the Azure app 
        /// </summary>
        public Uri RedirectUrl
        {
            get { return this.redirectUrl; }
            set
            {
                if (string.Equals(this.RedirectUrl, value))
                {
                    return;
                }

                this.SignOut();
                this.redirectUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the  current authentication token, while user is signed in
        /// </summary>
        public string AuthenticationToken { get; protected set; }

        /// <summary>
        /// Resource that is protected
        /// </summary>
        public string OAuthUrl { get; set; }

        /// <summary>
        /// Resource that is protected
        /// </summary>
        public string OAuthTokenResourceName { get; set; }

        /// <summary>
        /// Gets or sets the  current refresh token, while user is signed in
        /// </summary>
        public string RefreshToken { get; protected set; }

        /// <summary>
        ///  Forget about any authorization token that has been made
        /// </summary>
        public virtual void SignOut()
        {
            if (!this.IsSignedIn)
            {
                return;
            }

            this.authenticationContext = null;
            this.AuthenticationToken = null;
            this.currentAuthenticationResult = null;

            if (this.SignedOut != null)
            {
                this.SignedOut(this, null);
            }
        }

        /// <summary>
        /// Perform any required app initialization.
        /// This is where authentication with Active Directory is performed.        
        /// </summary>
        /// <param name="userId">The userId for prefilling login form.</param>
        /// <param name="secureSecret">Secure string with the password wor direct sign in.</param>
        /// <returns>The access token.</returns>
        public AuthenticationResult SignIn(string userId = "", string secureSecret = "")
        {
            if (this.BeforeSignIn != null)
            {
                this.BeforeSignIn(this, null);
            }

            // Obtain an authentication token to access the web service. 
            try
            {
                UserIdentifier user = UserIdentifier.AnyUser;
                if (!string.IsNullOrEmpty(userId))
                {
                    new UserIdentifier(userId, UserIdentifierType.OptionalDisplayableId);
                }

                this.authenticationContext = new AuthenticationContext(this.OAuthUrl, true);
                this.currentAuthenticationResult = this.authenticationContext.AcquireToken(
                    this.OAuthTokenResourceName,
                    this.AzureClientAppId,
                    this.RedirectUrl,
                    PromptBehavior.RefreshSession,
                    user);


                // Verify that an access token was successfully acquired.
                if (String.IsNullOrEmpty(currentAuthenticationResult.AccessToken))
                {
                    this.SignOut();
                }
                else
                {
                    this.AuthenticationToken = currentAuthenticationResult.AccessToken;
                    this.RefreshToken = currentAuthenticationResult.RefreshToken;

                    if (this.SignedIn != null)
                    {
                        this.SignedIn(this, null);
                    }
                }

                return currentAuthenticationResult;
            }
            finally
            {
            }
        }

        public bool RefreshSignIn()
        {
            if (!this.IsSignedIn)
            {
                return false;
            }

            if (this.currentAuthenticationResult.ExpiresOn.AddMinutes(10) <= DateTimeOffset.Now)
            {
                this.currentAuthenticationResult = this.authenticationContext.AcquireTokenByRefreshToken(this.currentAuthenticationResult.RefreshToken, this.odataClientAppId);
                this.AuthenticationToken = currentAuthenticationResult.AccessToken;
                this.RefreshToken = currentAuthenticationResult.RefreshToken;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the OData service is signed in
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return !string.IsNullOrEmpty(this.AuthenticationToken);
            }
        }

        /// <summary>
        /// Make sure we have not the Analytics in the service Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string CleanseServiceUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            var slash = new Char[]{ '/' };
            string cleansed = url.TrimEnd(slash);
            var tag = "analytics";
            if (cleansed.ToLowerInvariant().EndsWith(tag))
            {
                cleansed = cleansed.Remove(cleansed.Length - tag.Length);
                cleansed = url.TrimEnd(slash);
            }

            return cleansed;
        }
    }
}