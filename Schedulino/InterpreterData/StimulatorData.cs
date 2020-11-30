﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino.InterpreterData
{
    internal class StimulatorData
    {
        private string name;
        private string handler;
        private string behaviorPin;
        private string durationPin;

        public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
        public string Handler { get => handler; set => handler = value; }
        public string BehaviorPin { get => behaviorPin; set => behaviorPin = value; }
        public string DurationPin { get => durationPin; set => durationPin = value; }
    }
}