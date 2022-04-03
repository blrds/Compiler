using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler.Models
{
    class KeySet
    {
        public List<KeyConstruction> KeyWords { get; private set; }//любые ключевые слова

        public List<KeyConstruction> ValidChars { get; private set; }//любые сиволы, которые разделяют конструкции

        public KeyConstruction LineEndSymbol { get => ValidChars.Where(x => x.Type == ConstructionType.LineEnd).First(); }//символ конца строки

        public Tuple<KeyConstruction, KeyConstruction> MultiLineBrackets=null;//начало и конец многострочных комментариев, не являюстя Валидами
        public KeyConstruction LinecommentSymbol=null;//начало однострочного комментария, не являюстя Валидами

        public KeyConstruction StringSymbol { get=> ValidChars.Where(x => x.Type == ConstructionType.StringBrackets).First(); }//символ начала и конца строки как значения
        public KeyConstruction CharSymbol { get => ValidChars.Where(x => x.Type == ConstructionType.CharBrackets).First(); }//символ начала и конца символа как значения

        public KeySet()
        {
            KeyWords = new List<KeyConstruction>();
            ValidChars = new List<KeyConstruction>();
            
        }
        public bool isValid(char c)
        {
            try
            {
                return ValidChars.Where(x => x.Construction == Regex.Escape(c.ToString())).Any();
            }
            catch (Exception e) { }
            return false;
        }
        public KeyConstruction ValidChar(char c) {
            if (isValid(c))
            {
                try
                {
                    return ValidChars.Where(x => x.Construction == Regex.Escape(c.ToString())).First();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else return null;
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
    }
}
