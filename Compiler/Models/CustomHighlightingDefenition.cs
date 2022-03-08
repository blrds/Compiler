using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Compiler
{
    class CustomHighlightingDefenition : IHighlightingDefinition
    {
        public string Name => "MUPL";

        public HighlightingRuleSet MainRuleSet => ruleSet;

        private HighlightingRuleSet ruleSet = new HighlightingRuleSet();

        public CustomHighlightingDefenition( HighlightingRuleSet ruleSet)
        {
            this.ruleSet = ruleSet;
        }

        public IEnumerable<HighlightingColor> NamedHighlightingColors => colors;

        private IEnumerable<HighlightingColor> colors = new List<HighlightingColor>();

        public IDictionary<string, string> Properties => null;
        public HighlightingColor GetNamedColor(string name)
        {
            try
            {
                return colors.Where(x => x.Name == name).First();
            }
            catch (Exception e) { 
                return null;
            }
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            if (MainRuleSet.Name == name) return MainRuleSet;
            else return null;
        }
    }
}
