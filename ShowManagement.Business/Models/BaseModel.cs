﻿using Entities.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Business.Models
{
    public abstract class BaseModel : IObjectState
    {
        public ObjectState ObjectState { get; set; }
    }
}
