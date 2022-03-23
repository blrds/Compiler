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
        public List<MainInformation> Decomposite(string inline)
        {
            toDefault();
            var answer = new List<MainInformation>();

            char a = '\0';
            int from = 0;
            for (int i = 0; i < inline.Length; i++)
            {
                a = inline[i];
                #region StringChecker

                if (localLine == "" && a == '\"')
                {
                    isString = true;
                }
                if (localLine != "" && a == '\"' && isString)
                {
                    answer.Add(new MainInformation("\""+localLine+"\"", "vs", from, i));
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
                        if (!Char.IsDigit(a)&&a!='.'&& !KeySet.isValid(a))
                        {
                            isNumber = false;
                        }
                        if (a == '.' && isNumber)
                        {
                            if (secondDotFlag)
                            {
                                secondDotFlag = false;
                                isDotNumber = true;
                            }else
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
                                        answer.Add(new MainInformation(localLine, "floatNUM", from, i-1));
                                    else
                                        answer.Add(new MainInformation(localLine, "intNUM", from, i-1));
                                }
                                else
                                {
                                    if (KeySet.isKeyWord(localLine))
                                    {

                                        try
                                        {
                                            answer.Add(new MainInformation(localLine, KeySet.KeyWord(localLine).Code.ToString(), from, i-1));
                                        }
                                        catch (ArgumentNullException ne)
                                        {
                                            answer.Add(new MainInformation(localLine, "ERROR0", from, i-1));
                                        }
                                    }
                                    else {
                                        if (Char.IsLetter(localLine[0]))
                                        {
                                            answer.Add(new MainInformation(localLine, "varName", from, i-1));
                                        }
                                        else {
                                            answer.Add(new MainInformation(localLine, "ERROR0", from, i-1));
                                        }
                                    }
                                }
                            answer.Add(new MainInformation(a.ToString(), KeySet.ValidChar(a).Code.ToString(), i, i));
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
                        answer.Add(new MainInformation(localLine, "multiline comment", from, i));
                        from = i + 1;
                        toDefault();
                        continue;
                    }
                }
                #endregion

            }
            return answer;
        }
    }
}
