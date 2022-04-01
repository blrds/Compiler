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
        public List<KeyConstruction> KeyWords { get; private set; }

        public List<KeyConstruction> ValidChars { get; private set; }

        public KeyConstruction LineEndSymbol { get => ValidChars.Where(x => x.Type == ConstructionType.LineEnd).First();}

        public Tuple<KeyConstruction, KeyConstruction> MultiLineBrackets=null;
        public KeyConstruction LinecommentSymbol=null;

        public KeyConstruction StringSymbol { get=> ValidChars.Where(x => x.Type == ConstructionType.StringBrackets).First(); }
        public KeyConstruction CharSymbol { get => ValidChars.Where(x => x.Type == ConstructionType.CharBrackets).First(); }

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
                return ValidChars.Where(x =>x.Construction == Regex.Escape(c.ToString())).First();
            else throw new NullReferenceException("ValidChar");
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
                return KeyWords.Where(x => x.Construction == line).First();
            else throw new NullReferenceException("KeyWord");
        }
    }
}
