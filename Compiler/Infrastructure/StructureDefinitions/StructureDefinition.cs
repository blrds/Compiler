using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Infrastructure.StructureDefinitions.Base
{
    class StructureDefinition
    {
        List<KeyConstruction> Constructions = new List<KeyConstruction>();
        public StructureDefinition(HighlightingRuleSet ruleSet)
        {
            
        }
        public List<MainInformation> Decomposite(string inline) {
            var answer = new List<MainInformation>();
            string localLine = "";
            foreach (var a in inline) {
                localLine += a;

                #region KeyWordcheck
                foreach (var kw in KeyWords)
                {
                    if (kw.IsMatch(localLine)) { 
                        answer = new 
                    }
                } 
                #endregion
            }
            return answer;
        }
    }
}
