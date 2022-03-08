using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Infrastructure.StructureDefinitions.Base
{
    class MainInformation
    {
        public MainInformation(string construction, int code, int from, int to)
        {
            Construction = construction;
            Code = code;
            From = from;
            To = to;
        }

        public string Construction { get; private set; }
        public int Code { get; private set; }

        public int From { get; private set;  }
        public int To { get; private set;  }
    }
}
