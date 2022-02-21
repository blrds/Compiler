namespace Compiler
{
    enum ConstructionType { 
        Keyword,
        Brackets,
        Constructions,
        Value,
        Type
    }
    class KeyConstruction
    {
        public string Construction { get; private set; }
        public ConstructionType Type { get; private set; }

        public int Code { get; private set; }

        public KeyConstruction(string construction, ConstructionType type, int code) {
            Construction = construction;
            Type = type;
            Code = code;
        }
    }
}
