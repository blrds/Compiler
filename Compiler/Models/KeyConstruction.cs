namespace Compiler
{
    enum ConstructionType { 
        Keyword,//ключслова(const,var,..)
        NumPoint,//символ отделения челой и дробной частей
        Construction,//ключслова, не работающие по одному
        Value,//значения(true,false,..)
        EqualChar,//символы, способные отделять одну конструкцию от другой
        LineEnd,//символ конца строки
        MultilineComments,//символы, внутри которых будут многострочные коментарии
        LineComment,//символы начала однострочного коментария
        StringBrackets,//символы начала и конца строки как значения переменной
        VariableSymbol//символы, которые могут быть использованны при декларации переменных
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
