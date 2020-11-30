using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino.InterpreterData
{
    internal class PairingData
    {
        private string name;
        private string handler;

        public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
        public string Handler { get => handler; set => handler = value; }
    }
}
