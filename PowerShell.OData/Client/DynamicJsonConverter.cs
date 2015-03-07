//--------------------------------------------------------------------------
//  <copyright file="DynamicJsonConverter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.Marketing.Powershell.OData.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;

    /// <summary>
    /// The dynamic json converter.
    /// </summary>
    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        /// <summary>The deserialize.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The <see cref="object"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        /// <summary>The serialize.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The <see cref="IDictionary"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) }));
            }
        }

        #region Nested type: DynamicJsonObject

        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }

                this.dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                this.ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in this.dictionary)
                {
                    if (!firstInDictionary)
                    {
                        sb.Append(",");
                    }

                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else
                    {
                        var objects = value as IDictionary<string, object>;
                        if (objects != null)
                        {
                            new DynamicJsonObject(objects).ToString(sb);
                        }
                        else
                        {
                            var list = value as ArrayList;
                            if (list != null)
                            {
                                sb.Append(name + ":[");
                                var firstInArray = true;
                                foreach (var arrayValue in list)
                                {
                                    if (!firstInArray)
                                    {
                                        sb.Append(",");
                                    }

                                    firstInArray = false;
                                    var value1 = arrayValue as IDictionary<string, object>;
                                    if (value1 != null)
                                    {
                                        new DynamicJsonObject(value1).ToString(sb);
                                    }
                                    else if (arrayValue is string)
                                    {
                                        sb.AppendFormat("\"{0}\"", arrayValue);
                                    }
                                    else
                                    {
                                        sb.AppendFormat("{0}", arrayValue);
                                    }
                                }

                                sb.Append("]");
                            }
                            else
                            {
                                sb.AppendFormat("{0}:{1}", name, value);
                            }
                        }
                    }
                }

                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!this.dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                var newDictionary = result as IDictionary<string, object>;
                if (newDictionary != null)
                {
                    result = new DynamicJsonObject(newDictionary);
                    return true;
                }

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    if (arrayList[0] is IDictionary<string, object>)
                    {
                        result =
                            new List<object>(
                                arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                    }
                    else
                    {
                        result = new List<object>(arrayList.Cast<object>());
                    }
                }

                return true;
            }
        }

        #endregion
    }
}
