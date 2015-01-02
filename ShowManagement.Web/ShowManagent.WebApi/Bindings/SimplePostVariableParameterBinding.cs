using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace ShowManagent.WebApi.Bindings
{
    public class SimplePostVariableParameterBinding : HttpParameterBinding
    {
        public SimplePostVariableParameterBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// Check for simple binding parameters in POST data.
        /// Bind POST data as well as query string data
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string stringValue = null;

            NameValueCollection nvCollection = this.TryReadBody(actionContext.Request);

            if (nvCollection != null)
            {
                stringValue = nvCollection[this.Descriptor.ParameterName];
            }

            // Try reading query string if we have no POST/PUT match
            if (stringValue == null)
            {
                var query = actionContext.Request.GetQueryNameValuePairs();
                if (query != null)
                {
                    var matches = query.Where(kv => kv.Key.Equals(this.Descriptor.ParameterName, StringComparison.CurrentCultureIgnoreCase));
                    if (matches.Any())
                    {
                        stringValue = matches.First().Value;
                    }
                }
            }

            object value = this.StringToObject(stringValue);

            // Set the binding result here
            this.SetValue(actionContext, value);

            // Return a completed task with no result
            var tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }

        /// <summary>
        /// Method that implements parameter binding hookup to the 
        /// global config object's /// ParameterBindingRules collection delegate
        /// </summary>
        /// <example>
        /// GlobalConfiguration
        ///     .Configuration
        ///     .ParameterBindingRules
        ///     .Insert(0, SimplePostVariableParameterBinding.HookupParameterBinding);
        /// </example>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static HttpParameterBinding HookupParameterBinding(HttpParameterDescriptor descriptor)
        {
            HttpParameterBinding binding = null;

            var supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;

            // Only apply this binding to POST and PUT operations
            if (supportedMethods.Contains(HttpMethod.Post) || supportedMethods.Contains(HttpMethod.Put))
            {
                if (TypeConvertersCache.Converters.Keys.Contains(descriptor.ParameterType))
                {
                    binding = new SimplePostVariableParameterBinding(descriptor);
                }
            }

            return binding;
        }

        /// <summary>
        /// Read and cache the request body
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private NameValueCollection TryReadBody(HttpRequestMessage request)
        {
            object result = null;

            // Try to read out of cache first
            if (!request.Properties.TryGetValue(MULTIPLE_BODY_PARAMETERS, out result))
            {
                var contentType = request.Content.Headers.ContentType;

                // Only read if there is content and it's form data
                if (contentType == null && contentType.MediaType != "application/x-www-form-urlencoded")
                {
                    // No data
                    result = null;
                }
                else
                {
                    // Parsing the string like firstname=Hongmei&lastname=ASDASD
                    result = request.Content.ReadAsFormDataAsync().Result;
                }

                request.Properties.Add(MULTIPLE_BODY_PARAMETERS, result);
            }

            return result as NameValueCollection;
        }

        private object StringToObject(string stringValue)
        {
            object value = null;

            if (stringValue != null)
            {
                Func<string, object> convert = null;

                if (TypeConvertersCache.Converters.TryGetValue(this.Descriptor.ParameterType, out convert))
                {
                    value = convert(stringValue);
                }
                else
                {
                    value = stringValue;
                }
            }
            else
            {
                value = null;
            }

            return value;
        }


        private const string MULTIPLE_BODY_PARAMETERS = "MultipleBodyParameters";

        private struct AsyncVoid { }

        static class TypeConvertersCache
        {
            static TypeConvertersCache()
            {
                var dictionary = new Dictionary<Type, Func<string, object>>
                {
                    { typeof(string), v => v },
                    { typeof(short), v => short.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(int), v => int.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(long), v => long.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(decimal), v => decimal.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(double), v => double.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(DateTime), v => DateTime.Parse(v, CultureInfo.CurrentCulture) },
                    { typeof(bool), v => v.Equals(bool.TrueString, StringComparison.CurrentCultureIgnoreCase)
                                         || v.Equals("ON", StringComparison.CurrentCultureIgnoreCase)
                                         || v.Equals("1", StringComparison.CurrentCultureIgnoreCase) },
                };

                Converters = new ReadOnlyDictionary<Type, Func<string, object>>(dictionary);
            }

            internal static readonly IReadOnlyDictionary<Type, Func<string, object>> Converters;
        }
    }
}