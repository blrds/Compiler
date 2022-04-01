namespace Compiler
{
    enum ConstructionType { 
        Keyword,
        Brackets,
        Construction,
        Value,
        SeparatorChar,
        LineEnd,
        MultilineComments,//
        LineComment,//
        StringBrackets,//
        CharBrackets//
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
