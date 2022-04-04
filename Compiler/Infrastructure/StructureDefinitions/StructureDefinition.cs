using Compiler.Models;
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
        public KeySet KeySet { get; private set; }
        public StructureDefinition()
        {
            KeySet = RuleSetCreator.ExtractKeySet();
        }

        public List<Line> Decomposite(string inline)
        {
            var answer = new List<Line>();
            answer.Add(new Line());
            string localLine = "";
            char a = '\0';
            int from = 0;
            for (int i = 0; i < inline.Length; i++)
            {
                a = inline[i];
                if (a.ToString() == KeySet.StringSymbol.Construction && localLine == "")
                {
                    from = i;
                    int j = i + 1;
                    localLine += '"';
                    for (; j < inline.Length; j++)
                    {
                        localLine += inline[j];
                        if (j + 1 == inline.Length || inline[j] == '"') break;
                    }
                    if (j + 1 == inline.Length && inline[j] != '"') answer.Last().Items.Add(new MainInformation(localLine, "ERROR1|Uncompleted string", from, j));
                    else answer.Last().Items.Add(new MainInformation(localLine, "string", from, j + 1));
                    i = j;
                    localLine = "";
                    from = i;
                    continue;
                }

                if (KeySet.isValid(a) || i + 1 == inline.Length)
                {
                    if (localLine != "")
                    {
                        var word = KeySet.KeyWord(localLine);
                        if (word != null)
                            answer.Last().Items.Add(new MainInformation(localLine.ToString(), "kw", from, i));
                        else
                        {
                            int num = 0;
                            if (int.TryParse(localLine, out num))
                                answer.Last().Items.Add(new MainInformation(num.ToString(), "num", from, i));
                            else
                            {
                                double dnum = 0;
                                if (double.TryParse(localLine, out dnum))
                                    answer.Last().Items.Add(new MainInformation(dnum.ToString(), "dnum", from, i));
                                else
                                {
                                    answer.Last().Items.Add(new MainInformation(localLine.ToString(), "id", from, i));
                                }
                            }
                        }
                    }
                    if (KeySet.isValid(a))
                    {
                        answer.Last().Items.Add(new MainInformation(a.ToString(), "vc", i, i));
                        if (a.ToString() == KeySet.LineEndSymbol.Construction && i+1!=inline.Length)
                            answer.Add(new Line());
                    }
                    from = i + 1;
                    localLine = "";
                }
                else localLine += a;
            }
            return answer;
        }

    }
}
