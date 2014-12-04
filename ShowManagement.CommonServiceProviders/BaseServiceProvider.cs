using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShowManagement.CommonServiceProviders
{
    public abstract class BaseServiceProvider
    {
        protected BaseServiceProvider(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        protected string BaseAddress { get; private set; }
    }
}
