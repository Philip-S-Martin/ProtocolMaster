﻿using ProtocolMasterCore.Protocol.Interpreter;
using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.NullExtensions
{
    [InterpreterMeta("No Interpreter", "")]
    public class NullInterpreter : IInterpreter
    {
        public bool IsCanceled { get; set; }
        public List<ProtocolEvent> Generate(string protocolName)
        {
            return null;
        }
    }


}
