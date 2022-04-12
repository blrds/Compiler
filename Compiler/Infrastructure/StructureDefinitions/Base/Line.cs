using System.Collections.Generic;

namespace Compiler.Infrastructure.StructureDefinitions.Base
{
    class Line
    {
        public List<MainInformation> Items;
        public Line() {
            Items = new List<MainInformation>();
        }
        public Line(List<MainInformation> items)
        {
            Items = new List<MainInformation>();
        }

        public override string ToString()
        {
            string answer = "";
            foreach (var a in Items)
                answer += (a.Code + " ");
            return answer;
        }
    }
}
