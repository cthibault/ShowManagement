using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Exceptions
{
    class RestServiceResponseException : Exception
    {
        public RestServiceResponseException(string message, IRestResponse response)
            : base(message)
        {
            this.RestResponse = response;
        }

        public IRestResponse RestResponse { get; private set; }
    }
}
