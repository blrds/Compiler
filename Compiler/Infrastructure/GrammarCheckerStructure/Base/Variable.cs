using System;
using System.Collections.Generic;

namespace Compiler.Models
{
    class Variable
    {
        public string Decloration { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }

        public bool isCorrect { get; set; }
        public List<ArgumentException> DeclorationErrors { get; private set; }

        public Variable() {
            DeclorationErrors = new List<ArgumentException>();
            Decloration = "const";
            Name = "a";
            Value = 0;
            Type = "";
            isCorrect = false;
        }

        public override string ToString()
        {
            string val = "";
            if (Value != null) val = Value.ToString();
            else val = "null";
            return Decloration.ToString()+" "+Name + "=" +val+";";
        }
    }
}
