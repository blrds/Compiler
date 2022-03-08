using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Compiler.Models
{
    class RuleSetCreator
    {
        public static HighlightingRuleSet ExtractRuleSet() {
            var ruleSet = new HighlightingRuleSet();

            #region Colors
            IEnumerable<HighlightingColor> colors = new List<HighlightingColor>();

            using (StreamReader file = File.OpenText("ColorsBinding.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                colors = (List<HighlightingColor>)serializer.Deserialize(file, typeof(List<HighlightingColor>));
            } 
            #endregion

            List<KeyConstruction> keys;
            using (StreamReader file = File.OpenText("KeyConstructions.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                keys = (List<KeyConstruction>)serializer.Deserialize(file, typeof(List<KeyConstruction>));
            }
            ruleSet.Name = "BaseRuleSet";

            foreach (var a in keys)
            {
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
                    if (a.Type == ConstructionType.Constructions)
                    {
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
                            default: {
                                    rule.Color = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Color.FromRgb(0, 0, 0))};
                                    break;
                                }
                        }
                        rule.Regex = new Regex("\\b" + a.Construction + "\\b");
                    }

                    ruleSet.Rules.Add(rule);
                }
            }

            return ruleSet;
        }

        public static List<KeyConstruction> ExtractRuleList() {
            var answer = new List<KeyConstruction>();

            return answer;
        }
    }
}
