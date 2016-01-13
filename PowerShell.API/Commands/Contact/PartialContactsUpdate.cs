//--------------------------------------------------------------------------
//  <copyright file="PartialContactsUpdate.cs" company="Microsoft">
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

using System.Collections.Generic;

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.Contact
{
    using System;
    using System.Management.Automation;
    using SDK.Messages.Contact;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommon.Enter, "MDMContactsUpdate")]
    public class PartialContactsUpdate : TypedCmdlet<PartialContactsUpdateRequest, PartialContactsUpdateResponse>
    {

        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Dynamics.Marketing.SDK.Model.Contact"/>.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("ContactUpdate")]
        public IEnumerable<SDK.Model.PartialContactUpdateItem> PartialContactUpdateItems { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("AllowUpdateCompany", "UpdateCompany")]
        public bool EnableCreationOrUpdateOfCompany { get; set; }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.PartialContactUpdateItems == null)
            {
                throw new PSArgumentNullException("PartialContactUpdateItems");
            }
            
            var request = this.NewRequest();
            request.PartialContactUpdateItems = this.PartialContactUpdateItems;
            request.EnableCreationOrUpdateOfCompany = this.EnableCreationOrUpdateOfCompany;

            var response = this.ProcessRequest(request);
            if (response == null)
            {
                return;
            }

            this.WriteObject(response.Responses);
        }
    }
}