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
        private static ICollection<KeyConstruction> ExtractKeys()
        {
            List<KeyConstruction> keys;
            using (StreamReader file = File.OpenText("KeyConstructions.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                keys = (List<KeyConstruction>)serializer.Deserialize(file, typeof(List<KeyConstruction>));
            }
            return keys;
        }
        public static HighlightingRuleSet ExtractRuleSet()
        {
            var ruleSet = new HighlightingRuleSet();

            #region Colors
            IEnumerable<HighlightingColor> colors = new List<HighlightingColor>();

            using (StreamReader file = File.OpenText("ColorsBinding.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                colors = (List<HighlightingColor>)serializer.Deserialize(file, typeof(List<HighlightingColor>));
            }
            #endregion

            var keys = ExtractKeys();
            ruleSet.Name = "BaseRuleSet";

            foreach (var a in keys)
            {
                if (a.Type == ConstructionType.Construction || a.Type == ConstructionType.Keyword || a.Type == ConstructionType.Value)
                {
                    var rule = new HighlightingRule();
                    bool add = true;
                    if (a.Type == ConstructionType.Construction)
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
                            default:
                                {
                                    add = false;
                                    break;
                                }
                        }
                        rule.Regex = new Regex("\\b" + a.Construction + "\\b");
                    }
                    if (add)
                        ruleSet.Rules.Add(rule);
                }
                else {
                    var span = new HighlightingSpan();
                    string str = "";
                    if (a.Type == ConstructionType.MultilineComments)
                    {
                        var split = a.Construction.Split('|');
                        span.StartExpression = new Regex(split[0]);
                        span.EndExpression = new Regex(split[1]);
                        str = "Comments";
                    }
                    else if (a.Type == ConstructionType.LineComment)
                    {
                        str = "Comments";
                        span.StartExpression = new Regex(a.Construction, (RegexOptions)548);
                        span.EndExpression = new Regex("$", (RegexOptions)516);
                    }
                    else if (a.Type == ConstructionType.StringBrackets)
                    {
                        str = "String";
                        var split = a.Construction.Split('|');
                        span.StartExpression = new Regex(split[0]);
                        span.EndExpression = new Regex(split[1]);
                    }
                    else continue;
                    span.SpanColor = colors.Where(x => x.Name == str).First();
                    span.StartColor = colors.Where(x => x.Name == str).First();
                    span.EndColor = colors.Where(x => x.Name == str).First();
                    span.RuleSet = new HighlightingRuleSet { Name = str };
                    span.SpanColorIncludesStart = true;
                    span.SpanColorIncludesEnd = true;
                    ruleSet.Spans.Add(span);
                }
            }
            return ruleSet;
        }

        public static KeySet ExtractKeySet()
        {
            var answer = new KeySet();
            var keys = ExtractKeys();
            foreach (var a in keys)
            {
                switch (a.Type)
                {
                    case ConstructionType.EqualChar: {
                            answer.EqualChars.Add(a);
                            break;
                        }
                    case ConstructionType.Keyword:
                        {
                            answer.KeyWords.Add(a);
                            break;
                        }
                    case ConstructionType.NumPoint:
                        {
                            answer.NumPoint = a;
                            break;
                        }
                    case ConstructionType.Construction:
                        {
                            var b = a.Construction.Split('|');
                            foreach (var c in b)
                                answer.KeyWords.Add(new KeyConstruction(c, a.Type, a.Code));
                            break;
                        }
                    case ConstructionType.Value:
                        {
                            answer.KeyWords.Add(a);
                            break;
                        }
                    case ConstructionType.LineEnd:
                        {
                            answer.LineEndChars.Add(a);
                            break;
                        }
                    case ConstructionType.MultilineComments:
                        {
                            var b = a.Construction.Split('|');
                            var c0 = new KeyConstruction(b[0], a.Type, a.Code);
                            var c1 = new KeyConstruction(b[1], a.Type, a.Code);
                            answer.MultiLineBrackets = new System.Tuple<KeyConstruction, KeyConstruction>(c0, c1);
                            break;
                        }
                    case ConstructionType.LineComment:
                        {
                            answer.LinecommentSymbol = a;
                            break;
                        }
                    case ConstructionType.StringBrackets:
                        {
                            var b = a.Construction.Split('|');
                            var c = new KeyConstruction(b[0], a.Type, a.Code);
                            answer.StringChars.Add(c);
                            break;
                        }
                    case ConstructionType.VariableSymbol: {
                            answer.InVarsChars.Add(a);
                            break;
                        }
                    default: { break; }
                }
            }
            return answer;
        }
    }
}
