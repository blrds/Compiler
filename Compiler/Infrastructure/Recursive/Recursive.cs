using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Infrastructure.Recursive
{
    class Recursive
    {
        private List<string> stages = new List<string>();
        public List<string> Start(string line)
        {
            Expression(line,0);
            return stages;
        }

        private void Expression(string line, int tabcount)
        {
            string tabs = "";
            int i = 0;
            for (i = 0; i < tabcount; i++)
                tabs += "-";
            i = 0;
            int bCount = 0;
            string firstSubLine = "";
            while (true)
            {
                if (i >= line.Length) break;
                if (line[i] == '(') bCount++;
                if (line[i] == ')') bCount--;
                if (bCount < 0) {
                    stages.Add("Излишняя )");
                    return;
                }
                if (bCount==0 &&( line[i] == '+' || line[i] == '-'))
                {
                    break;
                }
                else
                {
                    firstSubLine += line[i];
                    i++;
                }
            }
            if (bCount > 0) {
                stages.Add("Отсутсвует )");
                return;
            }
            if (firstSubLine == line)
            {
                stages.Add(tabs+"T(" + line + ")");
                Term(line,++tabcount);
            }
            else
            {

                string secondSubLine = line.Substring(i + 1, line.Length - i - 1);
                stages.Add(tabs+"E(" + firstSubLine + ")" + line[i] + "T(" + secondSubLine + ")");
                Expression(firstSubLine,++tabcount);
                Term(secondSubLine,++tabcount);
            }
        }

        private void Term(string line, int tabcount)
        {
            string tabs = "";
            int i = 0;
            for (i = 0; i < tabcount; i++)
                tabs += "-";
            i = 0;
            int bCount = 0;
            string firstSubLine = "";
            while (true)
            {
                if (i >= line.Length) break;
                if (line[i] == '(') bCount++;
                if (line[i] == ')') bCount--;
                if (bCount < 0)
                {
                    stages.Add("Излишняя )");
                    return;
                }
                if (bCount == 0 && (line[i] == '*' || line[i] == '/'))
                {
                    break;
                }
                else
                {
                    firstSubLine += line[i];
                    i++;
                }
            }
            if (bCount > 0)
            {
                stages.Add("Отсутсвует )");
                return;
            }
            if (firstSubLine == line)
            {
                stages.Add(tabs+"F(" + line + ")");
                Factor(line,++tabcount);
            }
            else
            {
                string secondSubLine = line.Substring(i + 1, line.Length - i - 1);
                stages.Add(tabs+"T(" + firstSubLine + ")" + line[i] + "F(" + secondSubLine + ")");
                Term(firstSubLine,++tabcount);
                Factor(secondSubLine,++tabcount);
            }
        }

        private void Factor(string line, int tabcount)
        {
            string tabs = "";
            int i = 0;
            for (i = 0; i < tabcount; i++)
                tabs += "-";
            i = 0;
            int bCount = 0;
            string firstSubLine = "";
            while (true)
            {
                if (i >= line.Length) break;
                if (line[i] == '(') bCount++;
                if (line[i] == ')') bCount--;
                if (bCount < 0)
                {
                    stages.Add("Излишняя )");
                    return;
                }
                if (bCount == 0 && line[i] == '^')
                {
                    break;
                }
                else
                {
                    firstSubLine += line[i];
                    i++;
                }
            }
            if (bCount > 0)
            {
                stages.Add("Отсутсвует )");
                return;
            }
            if (firstSubLine == line)
            {
                stages.Add(tabs+"V(" + line + ")");
                Value(line,++tabcount);
            }
            else
            {
                string secondSubLine = line.Substring(i+1,line.Length-i-1);
                stages.Add(tabs+"V(" + firstSubLine + ")" + line[i] + "F(" + secondSubLine + ")");
                Value(firstSubLine,++tabcount);
                Factor(secondSubLine,++tabcount);
            }
        }

        private void Value(string line, int tabcount)
        {
            string tabs = "";
            int i = 0;
            for (i = 0; i < tabcount; i++)
                tabs += "-";
            i = 0;
            if (line[0] == '(')
            {
                if (line.Last() == ')')
                {
                    string subline = line.Substring(1, line.Length - 2);
                    Expression(subline,++tabcount);
                }
                else
                {
                    stages.Add("Отсутсвует )");
                    return;
                }
            }
            Regex id = new Regex(@"^[a-zA-Z](\w)*");
            Regex num = new Regex(@"^(\d)*$");
            if (id.IsMatch(line)) stages.Add(tabs+"ID=" + line);
            if (num.IsMatch(line)) stages.Add(tabs + "NUM=" + line);
        }
    }
}
