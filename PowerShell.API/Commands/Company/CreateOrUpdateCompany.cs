//--------------------------------------------------------------------------
//  <copyright file="CreateOrUpdateCompany.cs" company="Microsoft">
//     Copyright (c) 2015 Microsoft Corporation.
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
namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.Company
{
    using System.Management.Automation;

    using Microsoft.Dynamics.Marketing.Powershell.API.Commands.Validators;

    using SDK.Messages.Company;

    /// <summary>
    /// Command to setup the Azure namespace and authentication used by all other commands.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "MDMCompany")]
    public class CreateOrUpdateCompany : TypedCmdlet<CreateOrUpdateCompanyRequest, CreateOrUpdateCompanyResponse>
    {
        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Dynamics.Marketing.SDK.Model.Company"/>.
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, 
            HelpMessage = "Company SDK object that contains values for creation or update of a compoany.")]
        [ValidateNotNullOrEmpty]
        public SDK.Model.Company Company
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether concurrent updates should be ignored
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("IgnoreChanges", "Overwrite")]
        public bool DisableConcurrencyValidation { get; set; }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.Company.Validate();

            var request = this.NewRequest();
            request.Company = this.Company;
            request.DisableConcurrencyValidation = this.DisableConcurrencyValidation;

            var response = this.ProcessRequest(request);
            if (response == null)
            {
                return;
            }

            if (!response.Succeeded)
            {
                this.WriteVerbose("Command has failed:" + response.Message);
            }

            this.WriteObject(response.Company);
        }
    }
}