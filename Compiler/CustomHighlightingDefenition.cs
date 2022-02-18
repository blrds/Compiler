using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Compiler
{
    class CustomHighlightingDefenition : IHighlightingDefinition
    {
        public string Name => "MUPL";

        public HighlightingRuleSet MainRuleSet => ruleSet;

        private HighlightingRuleSet ruleSet = new HighlightingRuleSet();

        public CustomHighlightingDefenition()
        {
            using (StreamReader file = File.OpenText("ColorsBinding.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                colors = (List<HighlightingColor>)serializer.Deserialize(file, typeof(List<HighlightingColor>));
            }


            List<KeyConstruction> keys;
            using (StreamReader file = File.OpenText("KeyConstructions.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                keys = (List<KeyConstruction>)serializer.Deserialize(file, typeof(List<KeyConstruction>));
            }
            ruleSet.Name = "BaseRuleSet";
            foreach (var a in keys) {
                switch (a.Type) {
                    case ConstructionType.Bracket: {
                            if (a.Construction.Contains('"') || a.Construction.Contains("/*")||a.Construction.Contains("//")) {
                                var span = new HighlightingSpan();
                                span.RuleSet = ruleSet;
                                var split = a.Construction.Split('|');
                                span.StartExpression = new System.Text.RegularExpressions.Regex(split[0]);
                                if (a.Construction.Contains("//")) {
                                    span.EndExpression = new System.Text.RegularExpressions.Regex('\n'.ToString());
                                    span.SpanColor = colors.Where(x => x.Name == "Comments").First();
                                    break;
                                }
                                span.EndExpression = new System.Text.RegularExpressions.Regex(split[1]);
                                if (a.Construction.Contains('"')) {
                                    span.SpanColor = colors.Where(x => x.Name == "String").First();
                                    break;
                                }
                                if (a.Construction.Contains("/*")) {
                                    span.SpanColor = colors.Where(x => x.Name == "Comments").First();
                                    break;
                                }
                            }
                            break; 
                        }
                    case ConstructionType.Constructions: {
                            var rule = new HighlightingRule();
                            rule.Color = colors.Where(x => x.Name == "Construction").First();
                            rule.Regex = new System.Text.RegularExpressions.Regex(a.Construction);
                            ruleSet.Rules.Add(rule);
                            break;
                        }
                    case ConstructionType.Keyword: {
                            var rule = new HighlightingRule();
                            rule.Color = colors.Where(x => x.Name == "Word").First();
                            rule.Regex = new System.Text.RegularExpressions.Regex(a.Construction);
                            ruleSet.Rules.Add(rule);
                            break;
                        }
                    case ConstructionType.Value: {
                            var rule = new HighlightingRule();
                            rule.Color = colors.Where(x => x.Name == "Value").First();
                            rule.Regex = new System.Text.RegularExpressions.Regex(a.Construction);
                            ruleSet.Rules.Add(rule);
                            break; 
                        }
                    case ConstructionType.Type: {
                            var rule = new HighlightingRule();
                            rule.Color = colors.Where(x => x.Name == "Word").First();
                            rule.Regex = new System.Text.RegularExpressions.Regex(a.Construction);
                            ruleSet.Rules.Add(rule);
                            break;
                        }
                }
            }
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
