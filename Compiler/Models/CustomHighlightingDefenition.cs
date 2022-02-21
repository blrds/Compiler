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
                if (a.Type == ConstructionType.Brackets)
                {
                    if (a.Construction.Contains('"') || a.Construction.Contains("*") || a.Construction.Contains("//"))
                    {

                        var span = new HighlightingSpan();
                        string str = "";
                        if (a.Construction.Contains("//"))
                        {
                            str = "Comments";
                            span.StartExpression = new Regex(a.Construction, (RegexOptions)548);
                            span.EndExpression = new Regex("$", (RegexOptions)516);
                        }
                        else
                        {
                            var split = a.Construction.Split('|');
                            span.StartExpression = new Regex(split[0]);
                            span.EndExpression = new Regex(split[1]);

                            if (a.Construction.Contains('"')) str = "String";
                            if (a.Construction.Contains("*")) str = "Comments";

                        }
                        span.SpanColor = colors.Where(x => x.Name == str).First();
                        span.StartColor = colors.Where(x => x.Name == str).First();
                        span.EndColor = colors.Where(x => x.Name == str).First();
                        span.RuleSet = new HighlightingRuleSet { Name = str };
                        span.SpanColorIncludesStart = true;
                        span.SpanColorIncludesEnd = true;
                        ruleSet.Spans.Add(span);

                    }
                }
                else
                {
                    var rule = new HighlightingRule();
                    if (a.Type == ConstructionType.Constructions) {
                        rule.Color = colors.Where(x => x.Name == "Construction").First();
                        var split = a.Construction.Split('|');
                        rule.Regex = new Regex("\\b" + a.Construction + "\\b");
                    }
                    else
                    {
                        switch (a.Type)
                        {
                            case ConstructionType.Keyword:
                                {
                                    rule.Color = colors.Where(x => x.Name == "Word").First();
                                    break;
                                }
                            case ConstructionType.Value:
                                {
                                    rule.Color = colors.Where(x => x.Name == "Value").First();
                                    break;
                                }
                            case ConstructionType.Type:
                                {
                                    rule.Color = colors.Where(x => x.Name == "Word").First();
                                    break;
                                }
                        }
                        rule.Regex = new Regex("\\b" + a.Construction + "\\b");
                    }
                    
                    ruleSet.Rules.Add(rule);
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
