using Compiler.Infrastructure.StructureDefinitions.Base;
using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Infrastructure.GrammarCheckerStructure
{
    class GrammarChecker
    {
        public static List<Variable> VariablesDecloration(List<Line> lines) {
            List<Variable> answer = new List<Variable>();
            
            foreach (var line in lines) {
                int[] array = new int[] {-1, -1, -1, -1, -1};
                if (line.Items.Where(x => x.Code != "vc").Any()) answer.Add(new Variable());
                int i = 0;
                foreach (var a in line.Items) {
                    if (a.Construction != "=" && a.Construction!=";" && a.Code=="vc") continue;
                    if (a.Construction == "let" || a.Construction == "var" || a.Construction == "const") {
                        if (array[0] != -1)
                        {
                            answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected decloration key word " + a.Construction));
                            continue;
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if (a.Construction == "let") answer.Last().Decloration = DeclorationType.let;
                                if (a.Construction == "var") answer.Last().Decloration = DeclorationType.var;
                                if (a.Construction == "const") answer.Last().Decloration = DeclorationType.constant;
                            }
                            else
                            {
                                answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected decloration key word " + a.Construction));
                                continue;
                            }
                            array[0] = i;
                        }
                    }
                    if (a.Code=="id")
                    {
                        if (array[1] != -1)
                        {
                            answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected decloration id " + a.Construction));
                            continue;
                        }
                        else
                        {
                            if (i == 1) answer.Last().Name = a.Construction;
                            else
                            {
                                answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected decloration id " + a.Construction));
                                continue;
                            }
                            array[1] = i;
                        }
                    }
                    if (a.Construction=="=")
                    {
                        if (array[2] != -1 && i!=2) answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected character " + a.Construction));
                        else array[2] = i;

                    }
                    if (a.Code=="num"||a.Code=="dnum"||a.Code=="string"||a.Construction=="null"||a.Construction=="true"||a.Construction=="false")
                    {
                        if (array[3] != -1) answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected value " + a.Construction));
                        else
                        {
                            if (i == 3)
                            {
                                if (a.Code == "num") { answer.Last().Type = "int"; answer.Last().Value = int.Parse(a.Construction); }
                                if (a.Code == "dnum") { answer.Last().Type = "double"; answer.Last().Value = double.Parse(a.Construction); }
                                if (a.Code == "string") { answer.Last().Type = "string"; answer.Last().Value = a.Construction; }
                                if (a.Construction == "unsigned") { answer.Last().Type = "unsigned"; answer.Last().Value = null; }
                                if (a.Construction == "true" || a.Construction == "false") { answer.Last().Type = "boolean"; answer.Last().Value = Boolean.Parse(a.Construction); }
                            }else answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected value " + a.Construction));
                            array[3] = i;
                        }
                    }
                    if (a.Construction == ";")
                    {
                        if (array[4] != -1 && i!=4) answer.Last().DeclorationErrors.Add(new ArgumentException("unexpected character " + a.Construction));
                        else array[4] = i;
                    }
                    i++;
                }    
            }
            return answer;
        }
    }
}
