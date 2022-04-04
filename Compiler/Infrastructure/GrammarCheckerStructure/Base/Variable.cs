using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Models
{
    enum DeclorationType{
        var,
        let,
        constant,
        unsigned
    }
    class Variable
    {
        public DeclorationType Decloration { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public List<ArgumentException> DeclorationErrors { get; private set; }

        public Variable() {
            DeclorationErrors = new List<ArgumentException>();
            Decloration = DeclorationType.unsigned;
            Name = "";
            Value = null;
            Type = "";
        }

        public override string ToString()
        {
            return Name + "=" + Value.ToString();
        }
    }
}
