//--------------------------------------------------------------------------
//  <copyright file="DynamicsMarketingSnapIn.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.OData
{
    using System.ComponentModel;
    using System.Management.Automation;

    /// <summary>
    /// Create this sample as an PowerShell snap-in
    /// </summary>
    [RunInstaller(true)]
    public class DynamicsMarketingSnapIn : PSSnapIn
    {
        /// <summary>
        /// Get a name for this PowerShell snap-in. This name will be used in registering
        /// this PowerShell snap-in.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Microsoft.Dynamics.Marketing.OData";
            }
        }

        /// <summary>
        /// Vendor information for this PowerShell snap-in.
        /// </summary>
        public override string Vendor
        {
            get
            {
                return "Microsoft";
            }
        }

        /// <summary>
        /// Gets resource information for vendor. This is a string of format: 
        /// resourceBaseName,resourceName. 
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return string.Format("{0},{1}", this.Name, this.Vendor);
            }
        }

        /// <summary>
        /// Description of this PowerShell snap-in.
        /// </summary>
        public override string Description
        {
            get
            {
                return "PowerShell snap-in for Microsoft Dynamics Marketing OData endpoint.";
            }
        }

        /// <summary>
        /// Gets the description resource.
        /// </summary>
        public override string DescriptionResource
        {
            get
            {
                return string.Format("{0},{1}", this.Name, this.Description);
            }
        }
    }
}