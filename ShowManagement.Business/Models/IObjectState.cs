using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public interface IObjectState
    {
        ObjectState ObjectState { get; set; }
    }
}
