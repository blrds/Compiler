using Compiler.Infrastructure.StructureDefinitions.Base;
using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Infrastructure.GrammarCheckerStructure
{
    class GrammarChecker
    {
        public static List<Variable> VariablesDecloration(List<Line> lines)
        {
            List<Variable> answer = new List<Variable>();

            foreach (var line in lines)
            {
                var variable = new Line();
                int[] array = new int[] { -1, -1, -1, -1, -1 };
                if (line.Items.Where(x => x.Code != "vc").Any()) answer.Add(new Variable());
                else continue;
                int i = 0;
                foreach (var a in line.Items)
                {
                    if (a.Code == "error") answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденная конструкция " + a.Construction + " на позиции " + a.From + ":" + a.To));
                    if (a.Construction != "=" && a.Construction != ";" && a.Code == "vc") continue;
                    if (a.Construction == "let" || a.Construction == "var" || a.Construction == "const")
                    {
                        if (array[0] != -1)
                        {
                            answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденное ключевое слово " + a.Construction + " на позиции "+ a.From+":"+a.To));
                            continue;
                        }
                        else
                        {
                            answer.Last().Decloration = a.Construction;
                            array[0] = i;
                            i++;
                            continue;
                        }
                    }
                    if (a.Code == "id")
                    {
                        if (array[1] != -1)
                        {
                            answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденный идентификатор " + a.Construction + " на позиции " + a.From + ":" + a.To));
                            continue;
                        }
                        else
                        {
                            answer.Last().Name = a.Construction;
                            array[1] = i;
                            i++;
                            continue;
                        }
                    }
                    if (a.Construction == "=")
                    {
                        if (array[2] != -1) answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденное = " + a.Construction + " на позиции " + a.From + ":" + a.To));
                        else
                        {
                            array[2] = i;
                            i++;
                            continue;
                        }

                    }
                    if (a.Code == "num" || a.Code == "dnum" || a.Code == "string" || a.Construction == "null" || a.Construction == "true" || a.Construction == "false")
                    {
                        if (array[3] != -1) answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденное значение " + a.Construction + " на позиции " + a.From + ":" + a.To));
                        else
                        {
                            if (a.Code == "num") { answer.Last().Type = "int"; answer.Last().Value = int.Parse(a.Construction); }
                            if (a.Code == "dnum") { answer.Last().Type = "double"; answer.Last().Value = double.Parse(a.Construction); }
                            if (a.Code == "string") { answer.Last().Type = "string"; answer.Last().Value = a.Construction; }
                            if (a.Construction == "unsigned") { answer.Last().Type = "unsigned"; answer.Last().Value = null; }
                            if (a.Construction == "true" || a.Construction == "false") { answer.Last().Type = "boolean"; answer.Last().Value = Boolean.Parse(a.Construction); }
                            array[3] = i;
                            i++;
                            continue;

                        }
                    }
                    if (a.Construction == ";")
                    {
                        if (array[4] != -1) answer.Last().DeclorationErrors.Add(new ArgumentException("Непредвиденное ; " + a.Construction + " на позиции " + a.From + ":" + a.To));
                        else
                        {
                            array[4] = i;
                            i++;
                            continue;
                        }
                    }

                }
                bool flag = true;
                for (int j = 0; j < array.Length; j++)
                {
                    if (!(array[j] == j && (j==array.Length-1||array[j] < array[j + 1]))) flag &= false;
                    else flag &= true;
                    if (array[j] == -1) {
                        switch (j) {
                            case 0: {
                                    answer.Last().DeclorationErrors.Add(new ArgumentException("Ожидалось ключевое слово"));
                                    break;
                                }

                            case 1:
                                {
                                    answer.Last().DeclorationErrors.Add(new ArgumentException("Ожидался индентификатор переменной"));
                                    break;
                                }
                            case 2:
                                {
                                    answer.Last().DeclorationErrors.Add(new ArgumentException("Ожидалось = "));
                                    break;
                                }
                            case 3:
                                {
                                    answer.Last().DeclorationErrors.Add(new ArgumentException("Ожидалось значение"));
                                    break;
                                }
                            case 4:
                                {
                                    answer.Last().DeclorationErrors.Add(new ArgumentException("Ожидалось ;"));
                                    break;
                                }
                        }
                    }
                }
                flag &= array[array.Length - 1] == array.Length - 1;
                answer.Last().isCorrect = flag & (!answer.Last().DeclorationErrors.Any());

            }
            return answer;
        }
    }
}

