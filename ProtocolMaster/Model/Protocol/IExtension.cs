﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol
{
    public interface IExtension
    {
        bool IsCanceled { get; set; }
    }
}
