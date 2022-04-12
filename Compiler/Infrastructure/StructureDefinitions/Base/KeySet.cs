using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Compiler.Models
{
    class KeySet
    {
        public List<KeyConstruction> KeyWords { get; private set; }//любые ключевые слова

        public List<KeyConstruction> EqualChars { get; private set; }//любые сиволы, которые разделяют конструкции

        public List<KeyConstruction> LineEndChars { get; private set; }//символ конца строки

        public Tuple<KeyConstruction, KeyConstruction> MultiLineBrackets=null;//начало и конец многострочных комментариев, не являюстя Валидами
        public KeyConstruction LinecommentSymbol=null;//начало однострочного комментария, не являюстя Валидами

        public List<KeyConstruction> StringChars { get; private set; }//символ начала и конца строки как значения
        
        public List<KeyConstruction> InVarsChars { get; private set;} // символы, использующиеся в идентификаторах переменных

        public KeyConstruction NumPoint { get; set; }//символ отделения челой и дробной частей
        public KeySet()
        {
            KeyWords = new List<KeyConstruction>();
            EqualChars = new List<KeyConstruction>();
            StringChars = new List<KeyConstruction>();
            InVarsChars = new List<KeyConstruction>();
            LineEndChars = new List<KeyConstruction>();
        }
        public bool isValid(char c)
        {
            try
            {
                return (!InVarsChars.Where(x => x.Construction == Regex.Escape(c.ToString())).Any() && !Char.IsLetterOrDigit(c) && Regex.Escape(c.ToString())!=NumPoint.Construction);
            }
            catch (Exception e) { }
            return false;
        }

        public bool isKeyWord(string word)
        {
            try
            {
                return KeyWords.Where(x =>x.Construction==word).Any();
            }
            catch (Exception e) { }
            return false;
        }
        public KeyConstruction KeyWord(string line) {
            if (isKeyWord(line))
            {
                try
                {
                    return KeyWords.Where(x => x.Construction == line).First();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else return null;
        }
        public bool isStringChar(char c) {
            return StringChars.Where(x => x.Construction == Regex.Escape(c.ToString())).Any();
        }
        public bool isLineEndChar(char c)
        {
            return LineEndChars.Where(x => x.Construction == Regex.Escape(c.ToString())).Any();
        }
        public bool isInVarChar(char c)
        {
            return InVarsChars.Where(x => x.Construction == Regex.Escape(c.ToString())).Any();
        }
    }
}
