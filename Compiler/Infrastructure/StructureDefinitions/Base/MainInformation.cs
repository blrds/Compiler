using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Infrastructure.StructureDefinitions.Base
{
    class MainInformation
    {
        public MainInformation(string construction, string code, int from, int to)
        {
            Construction = construction;
            Code = code;
            From = from;
            To = to;
        }

        public string Construction { get; private set; }
        public string Code { get; private set; }

        public int From { get; private set;  }
        public int To { get; private set;  }

        public override string ToString()
        {
            return Construction+":"+Code+":"+From+":"+To+"\n";
        }
    }
}
