//--------------------------------------------------------------------------
//  <copyright file="ModelValidator.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.API.Commands.Validators
{
    using System;
    using System.Management.Automation;

    using Microsoft.Dynamics.Marketing.SDK.Model;

    /// <summary>
    /// Extensions for SDK models to capture most typical situations with incomplete data.
    /// </summary>
    public static class ModelValidator
    {
        /// <summary>Validates a SDK Contact entity for create or update</summary>
        /// <param name="contact">The contact to validate.</param>
        /// <exception cref="PSArgumentNullException"> Exception thrown upon failed validation</exception>
        public static void Validate(this Contact contact)
        {
            if (contact == null)
            {
                throw new PSArgumentNullException("Contact");
            }

            // Uppon creation of a lead (Id = null) we need a belongs to company
            if ((contact.Id == null) && ((contact.BelongsToCompany == null) || (contact.BelongsToCompany.Id == null)))
            {
                throw new PSArgumentNullException("Contact.BelongsToCompany");
            }

            // contact type
            if (!contact.IsClient && !contact.IsMarketing && !contact.IsStaff && !contact.IsVendor)
            {
                throw new PSArgumentNullException(
                    "One contact.IsClient, contact.IsMarketing, contact.IsStaff, contact.IsVendor");
            }

            // uppon creation of new company implicitely
            if ((contact.Company != null) && (contact.Company.Id == null))
            {
                if (string.IsNullOrEmpty(contact.Company.Name))
                {
                    throw new PSArgumentNullException("Contact.Company.Name");
                }

                if (!contact.Company.IsClient && !contact.Company.IsMarketing && !contact.Company.IsVendor)
                {
                    throw new PSArgumentNullException(
                        "One Contact.IsClient, contact.IsMarketing, contact.IsStaff, contact.IsVendor");
                }

                contact.Company.BelongsToCompany = contact.BelongsToCompany;
            }

            // Mandatory name properties
            if (string.IsNullOrEmpty(contact.FirstName) && string.IsNullOrEmpty(contact.LastName) && (contact.Company == null))
            {
                throw new PSArgumentNullException("At least one of: Contact.FirstName, Contact.LastName, Contact.Company");
            }
        }

        /// <summary>Validates a SDK Company entity for create or update</summary>
        /// <param name="company">The company to validate.</param>
        /// <exception cref="PSArgumentNullException"> Exception thrown upon failed validation</exception>
        public static void Validate(this Company company)
        {
            if (company == null)
            {
                throw new PSArgumentNullException("Contact");
            }

            // Uppon creation of a lead (Id = null) we need a belongs to company
            if ((company.Id == null) && ((company.BelongsToCompany == null) || (company.BelongsToCompany.Id == null)))
            {
                throw new PSArgumentNullException("Company.BelongsToCompany");
            }

            // contact type
            if (!company.IsClient && !company.IsMarketing && !company.IsVendor)
            {
                throw new PSArgumentNullException(
                    "One contact.IsClient, contact.IsMarketing, contact.IsVendor");
            }

            // Name is mandatory
            if (string.IsNullOrEmpty(company.Name))
            {
                throw new PSArgumentNullException("Name is mandatory");
            }
        }

        /// <summary>Validates a SDK Lead entity for create or update</summary>
        /// <param name="lead">The lead to validate.</param>
        /// <exception cref="PSArgumentNullException"> Exception thrown upon failed validation</exception>
        public static void Validate(this Lead lead)
        {
            if (lead == null)
            {
                throw new PSArgumentNullException("Lead");
            }

            // Lead.Name is mandatory
            if (string.IsNullOrEmpty(lead.Name))
            {
                throw new PSArgumentNullException("Lead.Name");
            }

            // Lead.Date is mandatory
            if ((lead.Date == null) || (DateTime.MinValue.Equals(lead.Date)))
            {
                throw new PSArgumentNullException("Lead.Date");
            }

            // Uppon creation of a lead (Id = null) we need a belongs to company
            if ((lead.Id == null) && ((lead.BelongsToCompany == null) || (lead.BelongsToCompany.Id == null)))
            {
                throw new PSArgumentNullException("Lead.BelongsToCompany");
            }

            // At least one of Company or Contact is mandatory
            if ((lead.Company == null) && (lead.Contact == null))
            {
                throw new PSArgumentNullException("at least one is needed: Lead.Contact or Lead.Company");
            }

            // Correct the lead company if we have information about the contact.Company
            if ((lead.Contact != null) && (lead.Contact.Company != null))
            {
                lead.Company = lead.Contact.Company;
            }

            // check belongs to for Markewting Contact and Marketing Company
            if ((lead.Contact != null) && (lead.Contact.BelongsToCompany != null) && (!lead.BelongsToCompany.Id.Equals(lead.Contact.BelongsToCompany.Id)))
            {
                throw new PSArgumentNullException("Lead.Contact does not belong to same company as lead.");
            }

            if ((lead.Company != null) && (lead.Company.BelongsToCompany != null) && (!lead.BelongsToCompany.Id.Equals(lead.Company.BelongsToCompany.Id)))
            {
                throw new PSArgumentNullException("Lead.Company does not belong to same company as lead.");
            }

            // Not possible to check for BelongsTo for Program & Campaign
        }
    }
}
