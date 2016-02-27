// =====================================================================
//  <copyright file="ODataRequestBuilder.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Microsoft.ServiceBus;

    using Newtonsoft.Json;
    using Microsoft.OData.Client;
    /// <summary>
    /// Http request builder used to request data from the MDM OData endpoint
    /// </summary>
    /// <typeparam name="TD">Type of data model class</typeparam>
    public class ODataRequestBuilder<TD>
        where TD : DataServiceContext
    {
        private readonly ODataServiceEnvironment<TD> environment;

        private readonly List<FilterItem> filterItems;

        private readonly Dictionary<string, string> customParameters;

        private string filter, query;

        /// <summary>
        /// Filter item struct that records a field filter
        /// </summary>
        private struct FilterItem
        {
            public string Field;

            public ODataOperators.FilterOperatorType Operator;

            public object Value;
        }

        /// <summary>
        /// Gets or sets the OData table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets fields for the Select query parameter
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        public string Filter
        {
            get
            {
                if (string.IsNullOrEmpty(this.filter) && (this.filterItems.Count > 0))
                {
                    this.filter = this.BuildFilter();
                }

                return this.filter;
            }

            set
            {
                this.filter = value;
            }
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public string Query
        {
            get
            {
                if (string.IsNullOrEmpty(this.query))
                {
                    this.query = this.BuildQueryString();
                }

                return this.query;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataRequestBuilder{TD}"/> class. 
        /// Public constructor
        /// </summary>
        public ODataRequestBuilder()
        {
            this.customParameters = new Dictionary<string, string>();
            this.filterItems = new List<FilterItem>();
            this.environment = ODataServiceEnvironment<TD>.GetEnvironment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataRequestBuilder{TD}"/> class. 
        /// Public constructor
        /// </summary>
        /// <param name="environment">
        /// The environment.
        /// </param>
        public ODataRequestBuilder(ODataServiceEnvironment<TD> environment)
        {
            if (environment == null)
            {
                throw new ArgumentException("Authentication Environment missing for RequestBuilder");
            }

            this.environment = environment;
        }

        /// <summary>
        /// Retrieve entity record data from the organization web service. 
        /// </summary>
        /// <returns>Response from the web service.</returns>
        /// <remarks>Builds an OData HTTP request using passed parameters and sends the request to the server.</remarks>
        public TD RetrieveODataContainer()
        {
            if (!this.environment.RefreshSignIn())
            {
                this.environment.SignIn();
            }

            var container = Activator.CreateInstance(typeof(TD), new Uri(this.environment.ODataServiceUrl)) as TD;
            if (container == null)
            {
                return null;
            }

            container.BuildingRequest += (sender, args) =>
                {
                    // transmit the access token we have received earlier
                    var headerVal = new AuthenticationHeaderValue("Bearer", this.environment.AuthenticationToken);
                    args.Headers.Add("Authorization", headerVal.ToString());
                };

            return container;
        }

        /// <summary>
        /// Adds filter conditions that are represented by command let property values
        /// </summary>
        /// <param name="cmdlet">The command let to scan for filter condition properties</param>
        public void AddFieldFilterConditions(Cmdlet cmdlet)
        {
            this.ResetQuery();

            if (cmdlet == null)
            {
                return;
            }

            // properties with ODataFilterFieldAttribute
            var props = cmdlet.GetType().GetProperties();
            foreach (var prop in props)
            {
                var att =
                    prop.GetCustomAttributes(typeof(ODataFilterFieldAttribute), true).FirstOrDefault() as
                    ODataFilterFieldAttribute;
                if (att == null)
                {
                    continue;
                }

                var value = prop.GetValue(cmdlet);
                if (value == null)
                {
                    continue;
                }

                var name = prop.Name;
                if (!string.IsNullOrEmpty(att.Name))
                {
                    name = att.Name;
                }

                var boolValue = value as bool?;
                if (boolValue != null)
                {
                    this.AddFilterCondition(name, att.Operator, boolValue.Value);
                    return;
                }

                var intValue = value as int?;
                if (intValue != null)
                {
                    this.AddFilterCondition(name, att.Operator, intValue.Value);
                    return;
                }

                var doubleValue = value as double?;
                if (doubleValue != null)
                {
                    this.AddFilterCondition(name, att.Operator, doubleValue.Value);
                    return;
                }

                var dateTime = value as DateTime?;
                if (dateTime != null)
                {
                    var utc = dateTime.Value.ToUniversalTime();
                    this.AddFilterCondition(name, att.Operator, utc);
                    return;
                }
                
                
                var stringValue = value.ToString();

                if (att.FieldType == typeof(Guid))
                {
                    var guidValue = Guid.Parse(stringValue);
                    this.AddFilterCondition(name, att.Operator, guidValue);
                    return;
                }
                
                this.AddFilterCondition(name, att.Operator, stringValue);
            }
        }

        /// <summary>
        /// Add filter items for the type of Contacts and Companies
        /// </summary>
        /// <param name="cmdlet">The command let to scan for filter condition properties</param>
        public void AddTypeFilterConditions(Cmdlet cmdlet)
        {
            // From known Type properties IsMarketing, IsClient, ...
            var typeProp = cmdlet.GetType().GetProperty("IsMarketing");
            if (typeProp != null)
            {
                var isMarketing = typeProp.GetValue(cmdlet) as bool?;
                if ((isMarketing != null) && (isMarketing.Value))
                {
                    this.AddFilterCondition(
                        "substringof('Marketing', Type)",
                        ODataOperators.FilterOperatorType.Equals,
                        true);
                }
            }

            typeProp = cmdlet.GetType().GetProperty("IsClient");
            if (typeProp != null)
            {
                var isMarketing = typeProp.GetValue(cmdlet) as bool?;
                if ((isMarketing != null) && (isMarketing.Value))
                {
                    this.AddFilterCondition(
                        "substringof('Client', Type)",
                        ODataOperators.FilterOperatorType.Equals,
                        true);
                }
            }

            typeProp = cmdlet.GetType().GetProperty("IsVendor");
            if (typeProp != null)
            {
                var isMarketing = typeProp.GetValue(cmdlet) as bool?;
                if ((isMarketing != null) && (isMarketing.Value))
                {
                    this.AddFilterCondition(
                        "substringof('Vendor', Type)",
                        ODataOperators.FilterOperatorType.Equals,
                        true);
                }
            }

            typeProp = cmdlet.GetType().GetProperty("IsSiteCompany");
            if (typeProp != null)
            {
                var isMarketing = typeProp.GetValue(cmdlet) as bool?;
                if ((isMarketing != null) && (isMarketing.Value))
                {
                    this.AddFilterCondition(
                        "substringof('Site', Type)",
                        ODataOperators.FilterOperatorType.Equals,
                        true);
                }
            }

            typeProp = cmdlet.GetType().GetProperty("IsStaff");
            if (typeProp != null)
            {
                var isMarketing = typeProp.GetValue(cmdlet) as bool?;
                if ((isMarketing != null) && (isMarketing.Value))
                {
                    this.AddFilterCondition(
                        "substringof('Staff', Type)",
                        ODataOperators.FilterOperatorType.Equals,
                        true);
                }
            }
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, string value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, Guid value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, bool value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, int value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, double value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Add a new filter condition
        /// </summary>
        /// <param name="field">Field in the condition</param>
        /// <param name="valueOperator">Operator in the condition</param>
        /// <param name="value">Value in the condition</param>
        public void AddFilterCondition(string field, ODataOperators.FilterOperatorType valueOperator, DateTime value)
        {
            this.ResetQuery();
            this.filterItems.Add(new FilterItem { Field = field, Operator = valueOperator, Value = value });
        }

        /// <summary>
        /// Adds custom parameters that are represented by command let property values
        /// </summary>
        /// <param name="cmdlet">The command let to scan for filter conditions</param>
        public void AddCustomParameters(Cmdlet cmdlet)
        {
            this.ResetQuery();

            if (cmdlet == null)
            {
                return;
            }

            var props = cmdlet.GetType().GetProperties();
            foreach (var prop in props)
            {
                var att = prop.GetCustomAttributes(typeof(ODataQueryParamAttribute), true).FirstOrDefault() as ODataQueryParamAttribute;
                if (att == null)
                {
                    continue;
                }

                var value = prop.GetValue(cmdlet);
                if (value == null)
                {
                    continue;
                }

                var name = prop.Name;
                if (!string.IsNullOrEmpty(att.Name))
                {
                    name = att.Name;
                }

                if (value is int?)
                {
                    value = ((int?)value).Value;
                }
                else if (value is bool?)
                {
                    value = ((bool?)value).Value;
                }

                this.AddCustomParameter(name, value.ToString());
            }            
        }

        /// <summary>
        /// Add custom parameters to the query
        /// </summary>
        /// <param name="param">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void AddCustomParameter(string param, string value)
        {
            this.ResetQuery();
            this.customParameters.Add(param, value);
        }

        /// <summary>
        /// Reset the last created query
        /// </summary>
        private void ResetQuery()
        {
            this.filter = null;
            this.query = null;
        }

        /// <summary>
        /// Build the filter string from the set of registered conditions
        /// </summary>
        /// <returns>Filter query string</returns>
        private string BuildFilter()
        {
            var filters = string.Empty;
            foreach (var filterItem in this.filterItems)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    filters = string.Format("{0} {1} ", filters, ODataOperators.AND);
                }

                var operatorTag = ODataOperators.GetODataFilterOperatorString(filterItem.Operator);
                if (filterItem.Value is double)
                {
                    filters += string.Format(
                        "({0} {1} {2})",
                        filterItem.Field,
                        operatorTag,
                        ((double)filterItem.Value).ToString(CultureInfo.InvariantCulture));
                }
                else if (filterItem.Value is int)
                {
                    filters += string.Format(
                        "({0} {1} {2})",
                        filterItem.Field,
                        operatorTag,
                        ((int)filterItem.Value).ToString(CultureInfo.InvariantCulture));
                }
                else if (filterItem.Value is bool)
                {
                    filters += string.Format(
                        "({0} {1} {2})",
                        filterItem.Field,
                        operatorTag,
                        filterItem.Value.ToString().ToLower());
                }
                else if (filterItem.Value is DateTime)
                {
                    var utc = ((DateTime)filterItem.Value).ToUniversalTime();
                    filters += string.Format(
                        "({0} {1} datetime'{2}')",
                        filterItem.Field,
                        operatorTag,
                        string.Format("{0:yyyy-MM-dd}T{0:HH:mm:ss}Z", utc));
                }
                else if (filterItem.Value is Guid)
                {
                    filters += string.Format(
                        "({0} {1} guid'{2}')",
                        filterItem.Field,
                        operatorTag,
                        ((Guid)filterItem.Value).ToString());
                }
                else
                {
                    filters += string.Format("({0} {1} '{2}')", filterItem.Field, operatorTag, filterItem.Value);
                }
            }

            return filters;
        }

        /// <summary>
        /// Requests OData from MDM OData Service
        /// </summary>
        /// <returns>OData response</returns>
        public string BuildQueryString()
        {
            var queryString = string.Format("/{0}", this.TableName);

            var parameters = new Dictionary<string, string>();
            if (!this.customParameters.ContainsKey("select") && (!string.IsNullOrEmpty(this.Fields)))
            {
                parameters.Add("select", this.Fields);
            }

            if (!string.IsNullOrEmpty(this.Filter))
            {
                parameters.Add("filter", this.Filter);
            }

            if ((parameters.Count + this.customParameters.Count) > 0)
            {
                queryString += '?';
            }

            var conc = string.Empty;
            foreach (var param in parameters)
            {
                queryString += conc + "$" + param.Key + "=" + param.Value;
                conc = "&";
            }

            foreach (var param in this.customParameters)
            {
                queryString += conc + "$" + param.Key + "=" + param.Value;
                conc = "&";
            }

            return queryString;
        }

        /// <summary>Request OData and return result deserialized into desired format.</summary>
        /// <param name="template">The template data object.</param>
        /// <typeparam name="T">Type of template</typeparam>
        /// <returns>Deserialized data object</returns>
        public T RequestOData<T>(T template)
        {
            var json = this.RequestOData();
            try
            {
                var dataObj = JsonConvert.DeserializeAnonymousType(json, template);
                var prop = dataObj.GetType().GetProperty("value");
                if ((prop == null) || (prop.GetValue(dataObj) == null))
                {
                    throw new InvalidRequestException(json, null);
                }

                return dataObj;
            }
            catch (JsonReaderException ex)
            {
                var error = ex.Message;
                if (json != null)
                {
                    error += "\r\n" + json;
                }
                
                throw new InvalidRequestException(error, ex);
            }
        }

        /// <summary>
        /// Requests OData from MDM OData Service
        /// </summary>
        /// <returns>OData response</returns>
        public string RequestOData()
        {
            return this.RequestOData(this.Query);
        }

        /// <summary>
        /// Requests OData from MDM OData Service
        /// </summary>
        /// <param name="localQuery">Query parameter string for the OData request</param>
        /// <returns>OData response</returns>
        public string RequestOData(string localQuery)
        {
            if (!this.environment.IsSignedIn)
            {
                throw new InvalidPowerShellStateException("Not signed in to OData endpoint.");
            }

            if (!this.environment.RefreshSignIn())
            {
                throw new Exception("Refresh sign in to OData endpoint failed.");
            }

            if (string.IsNullOrEmpty(localQuery))
            {
                localQuery = this.Query;
            }

            var uri = new Uri(this.environment.ODataServiceUrl + localQuery);
            var client = new HttpClient
            {
                BaseAddress = uri,
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json;odata.metadata=minimal;q=1.0,application/json;odata=minimalmetadata;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("User-Agent", "Microsoft.Data.Mashup (http://go.microsoft.com/fwlink/?LinkID=304225)");

            var msg = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = uri };
            msg.Headers.Add("MaxDataSericeVersion", "3.0");
            msg.Headers.Add("OData-MaxVersion", "4.0");
            var headerVal = new AuthenticationHeaderValue("Bearer", this.environment.AuthenticationToken);
            msg.Headers.Add("Authorization", headerVal.ToString());

            var sendTask = client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            sendTask.Wait(5000);
            var response = sendTask.Result;
            var readTask = response.Content.ReadAsStringAsync();
            readTask.Wait(5000);
            var json = readTask.Result;
            
            return json;
        }
    }
}
