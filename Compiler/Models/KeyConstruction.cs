namespace Compiler
{
    enum ConstructionType { 
        Keyword,//ключслова(const,var,..)
        Brackets,//скобки
        Construction,//ключслова, не работающие по одному
        Value,//значения(true,false,..)
        SeparatorChar,//символы, способные отделять одну конструкцию от другой
        LineEnd,//символ конца строки
        MultilineComments,//символы, внутри которых будут многострочные коментарии
        LineComment,//символы начала однострочного коментария
        StringBrackets,//символы начала и конца строки как значения переменной
        CharBrackets//символы начала и конца символа как значения переменной
    }
    class KeyConstruction
    {
        public string Construction { get; private set; }
        public ConstructionType Type { get; private set; }

        public string Code { get; private set; }

        public KeyConstruction(string construction, ConstructionType type, string code) {
            Construction = construction;
            Type = type;
            Code = code;
        }
    }
}
