//--------------------------------------------------------------------------
//  <copyright file="FindLeads.cs" company="Microsoft">
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

namespace Microsoft.Dynamics.Marketing.Powershell.OData.Commands
{
    using System;
    using System.Dynamic;
    using System.Management.Automation;

    using Client;

    using Microsoft.ServiceBus;

    /// <summary>
    /// The find contacts by email.
    /// </summary>
    [Cmdlet(VerbsCommon.Find, "MDMLeads")]
    public class FindLeads : ODataCmdletBase
    {
        /// <summary>
        /// Gets or sets the id of the belongs to company.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField(FieldType = typeof(Guid))]
        [Alias("BelongsToId")]
        public string BelongsToCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the id of the belongs to company.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField(FieldType = typeof(Guid))]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField]
        public bool? SalesReady { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataFilterField(Name = "Score", Operator = ODataOperators.FilterOperatorType.GreaterThanOrEquals)]
        public double? MinScore { get; set; }

        /// <summary>
        /// Gets or sets the fields for the request.
        /// </summary>
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ODataQueryParam(Name = "select")]
        public string Fields { get; set; }

        /// <summary>
        /// ProcessRecord method.
        /// </summary>
        protected override void ProcessRecord()
        {
            // deserialize into object with template
            var genericTemplate = new { metadata = string.Empty, value = new[] { new ExpandoObject() } };

            var entityTemplate =
                new
                    {
                        metadata = string.Empty,
                        value =
                            new[]
                                {
                                    new
                                        {
                                            Id = string.Empty,
                                            Name = string.Empty,
                                            BelongsToCompanyId = string.Empty,
                                            LeadDate = DateTime.MinValue,
                                            Status = string.Empty,
                                            SalesReady = false,
                                            Score = (double)0.0,
                                            CampaignName = string.Empty,
                                            ProgramName = string.Empty,
                                        }
                                }
                    };

            var requestBuilder = this.RequestBuilder;
            requestBuilder.TableName = "Leads";


            requestBuilder.Fields =
                "Id,Name,BelongsToCompanyId,LeadDate,Status,SalesReady,Score,CampaignName,ProgramName";

            requestBuilder.AddCustomParameters(this);
            requestBuilder.AddFieldFilterConditions(this);

            this.WriteVerbose(requestBuilder.Query);

            try
            {
                if (this.Fields == null)
                {
                    var dataObj = requestBuilder.RequestOData(entityTemplate);
                    this.WriteObject(dataObj.value);
                }
                else
                {
                    var dataObj = requestBuilder.RequestOData(genericTemplate);
                    this.WriteObject(dataObj.value);
                }
            }
            catch (InvalidRequestException ex)
            {
                this.WriteVerbose("Request error:" + ex.Message);
            }
        }
    }
}
