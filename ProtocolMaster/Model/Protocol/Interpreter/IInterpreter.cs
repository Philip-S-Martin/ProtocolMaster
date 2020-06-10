﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProtocolMaster.Model.Protocol.Interpreter
{
    public interface IInterpreter : IExtension
    {
        List<ProtocolEvent> Data { get; }
        void Generate(string protocolName);
    }
}
