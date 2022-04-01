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
            string localLine = "";
            char a = '\0';
            int from = 0;
            for (int i = 0; i < inline.Length; i++)
            {
                a = inline[i];
                if (a.ToString() == KeySet.LineEndSymbol.Construction)
                    answer.Add(new Line());
                if (a.ToString() == KeySet.StringSymbol.Construction && inline == "")
                {
                    from = i;
                    int j = i + 1;
                    for (; j < inline.Length; j++)
                    {
                        localLine += inline[j];
                        if (j + 1 == inline.Length || inline[j + 1] == '"') break;
                    }
                    if (j + 1 == inline.Length) answer.Last().Items.Add(new MainInformation('"' + localLine, "ERROR1|Uncompleted string", from, j));
                    else answer.Last().Items.Add(new MainInformation(localLine, "string", from, ++j));
                    i += j;
                    continue;
                    ;
                }
                if (a.ToString() == KeySet.CharSymbol.Construction) {
                    if (i + 3 < inline.Length && inline[i + 1] == '\\')
                    {

                    }
                    if (i + 2 < inline.Length) {
                        if (inline[i + 2] == '\'') answer.Last().Items.Add(new MainInformation(inline[i + 1].ToString(), "char", i, i + 2));
                        else {
                            int counter = 1;
                            while (true) {
                                counter++;
                                localLine += inline[i + counter];
                                if (inline[i + counter] == '\'') break;
                            }
                            answer.Last().Items.Add(new MainInformation('\'' + localLine, "ERROR2|Unreal char", i, i+counter));
                        }
                    }
                }

                if (KeySet.isValid(a))
                {

                }
                else localLine += a;
            }
            return answer;
        }
        /*   #region oldVersion

           private bool isNumber = true;//определяет число
           private bool isDotNumber = true;//определяет точку в числе
           private bool secondDotFlag = true;//определяет точку в числе
           private bool isString = false;//определяет строку
           private bool isCom = false;//определяет коментарий
           private string localLine = "";
           private void toDefault()
           {
               localLine = "";
               isNumber = true;
               isDotNumber = false;
               secondDotFlag = true;
               isString = false;
               isCom = false;
           }
           public List<Line> Decomposite(string inline)
           {
               toDefault();
               var answer = new List<Line>();

               char a = '\0';
               int from = 0;
               for (int i = 0; i < inline.Length; i++)
               {
                   a = inline[i];
                   #region StringChecker
                   var line = new List<MainInformation>();

                   if (localLine == "" && a == '\"')
                   {
                       isString = true;
                   }
                   if (localLine != "" && a == '\"' && isString)
                   {
                       line.Add(new MainInformation("\"" + localLine + "\"", "vs", from, i));
                       from = i + 1;
                       toDefault();
                       continue;
                   }
                   #endregion

                   if (!isCom)
                   {
                       if (!isString)
                       {
                           #region Numbers
                           if (!Char.IsDigit(a) && a != '.' && !KeySet.isValid(a))
                           {
                               isNumber = false;
                           }
                           if (a == '.' && isNumber)
                           {
                               if (secondDotFlag)
                               {
                                   secondDotFlag = false;
                                   isDotNumber = true;
                               }
                               else
                               {
                                   isDotNumber = false;
                                   isNumber = false;
                               }


                           }
                           #endregion
                           #region ValidSymbol
                           if (KeySet.isValid(a))
                           {
                               if (localLine != "")
                                   if (isNumber)
                                   {
                                       if (isDotNumber)
                                           line.Add(new MainInformation(localLine, "floatNUM", from, i - 1));
                                       else
                                           line.Add(new MainInformation(localLine, "intNUM", from, i - 1));
                                   }
                                   else
                                   {
                                       if (KeySet.isKeyWord(localLine))
                                       {

                                           try
                                           {
                                               line.Add(new MainInformation(localLine, KeySet.KeyWord(localLine).Code.ToString(), from, i - 1));
                                           }
                                           catch (ArgumentNullException ne)
                                           {
                                               line.Add(new MainInformation(localLine, "ERROR0", from, i - 1));
                                           }
                                       }
                                       else
                                       {
                                           if (Char.IsLetter(localLine[0]))
                                           {
                                               line.Add(new MainInformation(localLine, "varName", from, i - 1));
                                           }
                                           else
                                           {
                                               line.Add(new MainInformation(localLine, "ERROR0", from, i - 1));
                                           }
                                       }
                                   }
                               line.Add(new MainInformation(a.ToString(), KeySet.ValidChar(a).Code.ToString(), i, i));
                               if (a.ToString() == KeySet.LineEndSymbol.Construction)
                               {
                                   answer.Add(new Line(line));
                               }
                               toDefault();
                               from = i + 1;
                           }
                           #endregion


                       }
                   }
                   if (!(localLine == "" && KeySet.isValid(a))) localLine += a;
                   #region Multiline coms
                   if (i + 1 < inline.Length + 2)
                   {
                       if (a == '/' && inline[i + 1] == '*')
                       {
                           isCom = true;
                       }
                       if (a == '/' && inline[i - 1] == '*' && isCom)
                       {
                           var hline = new Line();
                           hline.Items.Add(new MainInformation(localLine, "multiline comment", from, i));
                           from = i + 1;
                           toDefault();
                           continue;
                       }
                   }
                   #endregion

               }
               return answer;
           } 
           #endregion*/
    }
}
