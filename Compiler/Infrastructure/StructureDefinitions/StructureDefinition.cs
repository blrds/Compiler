using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Infrastructure.StructureDefinitions.Base
{
    class StructureDefinition
    {
        public KeySet KeySet { get; private set; }
        public StructureDefinition()
        {
            KeySet = RuleSetCreator.ExtractKeySet();
        }

        public bool isVarSuitable(string name)
        {
            if (KeySet.isInVarChar(name[0]) && (name.Length == 1 || !Char.IsLetter(name[1])))return false;

            if (Char.IsLetter(name[0]) && !KeySet.isValid(name[0]))
                for (int i = 1; i < name.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(name[i]) && !KeySet.isInVarChar(name[i])) return false;
                }
            else return false;
            return true;
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
                if (KeySet.isStringChar(a) && localLine == "")
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
                    if (!KeySet.isValid(a) && i + 1 == inline.Length)
                    {
                        localLine += a;
                    }
                    if (localLine != "")
                    {
                        var word = KeySet.KeyWord(localLine);
                        if (word != null)
                            answer.Last().Items.Add(new MainInformation(localLine.ToString(), word.Code, from, i));
                        else
                        {
                            int num = 0;
                            if (int.TryParse(localLine, out num))
                                answer.Last().Items.Add(new MainInformation(num.ToString(), "num", from, i));
                            else
                            {
                                float dnum = 0;
                                var helpline = localLine.Replace(KeySet.NumPoint.Construction.Last(), ',');
                                if (float.TryParse(helpline, out dnum))
                                    answer.Last().Items.Add(new MainInformation(dnum.ToString(), "dnum", from, i));
                                else
                                {
                                    if (isVarSuitable(localLine))
                                        answer.Last().Items.Add(new MainInformation(localLine, "id", from, i));
                                    else
                                        answer.Last().Items.Add(new MainInformation(localLine, "error", from, i));
                                }
                            }
                        }
                    }
                    if (KeySet.isValid(a))
                    {
                        answer.Last().Items.Add(new MainInformation(a.ToString(), "vc", i, i));
                        if (KeySet.isLineEndChar(a) && i + 1 != inline.Length)
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
