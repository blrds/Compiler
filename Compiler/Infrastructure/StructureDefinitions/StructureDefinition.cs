using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            if (KeySet.isInVarChar(name[0]) && (name.Length == 1 || !Char.IsLetter(name[1]))) return false;

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
                #region str

                //if (KeySet.isStringChar(a) && localLine == "")
                //{
                //    from = i;
                //    int j = i + 1;
                //    localLine += '"';
                //    for (; j < inline.Length; j++)
                //    {
                //        localLine += inline[j];
                //        if (j + 1 == inline.Length || inline[j] == '"') break;
                //    }
                //    if (j + 1 == inline.Length && inline[j] != '"') answer.Last().Items.Add(new MainInformation(localLine, "ERROR1|Uncompleted string", from, j));
                //    else answer.Last().Items.Add(new MainInformation(localLine, "string", from, j + 1));
                //    i = j;
                //    localLine = "";
                //    from = i;
                //    continue;
                //}
                #endregion
                #region mlc
                if (i <= inline.Length - 5 && Regex.Escape(a + "" + inline[i + 1]) == KeySet.MultiLineBrackets.Item1.Construction)
                {
                    string safeline = KeySet.MultiLineBrackets.Item1.Construction;
                    char b = '\0';
                    int j = 0;
                    bool flag = false;
                    for (j = i + 2; j < inline.Length; j++)
                    {
                        b = inline[j];
                        if (Regex.Escape(b + "" + inline[j + 1]) == KeySet.MultiLineBrackets.Item2.Construction)
                        {
                            flag = true;
                            break;
                        }
                        else safeline += b;
                    }
                    if (flag)
                    {
                        safeline += KeySet.MultiLineBrackets.Item2.Construction;
                        answer.Last().Items.Add(new MainInformation(safeline, "mlc", i, j + 1));
                    }
                    else answer.Last().Items.Add(new MainInformation(safeline, "ERROR3|незаконченный многострочный комментарий", i, j + 1));
                    i = j + 1;
                    continue;
                }
                #endregion
                #region lc
                if (i <= inline.Length - 2 && Regex.Escape(a + "" + inline[i + 1]) == KeySet.LinecommentSymbol.Construction)
                {
                    string safeline = KeySet.LinecommentSymbol.Construction;
                    char b = '\0';
                    int j = 0;
                    bool flag = false;
                    for (j = i + 2; j < inline.Length; j++)
                    {
                        b = inline[j];
                        if (b == '\r')
                        {
                            flag = true;
                            break;
                        }
                        else safeline += b;
                    }
                    safeline += '\r';
                    answer.Last().Items.Add(new MainInformation(safeline, "lc", i, j + 1));
                    i = j + 1;
                    continue;
                }
                #endregion
                #region vals n id

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
                                        answer.Last().Items.Add(new MainInformation(localLine, "ERROR2|неизвестная конструкция", from, i));
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

                #endregion
                else localLine += a;
            }
            return answer;
        }

    }
}
