using Generator.Zebra.Clues.Abstract;

namespace Generator.Zebra.Clues.Types;

public record AttributeIsBeforeClue(string Category1, string Value1, string Category2, string Value2)
    : OrderedBinaryAttributeClue<AttributeIsBeforeClue>(Category1, Value1, Category2, Value2)
{
    
}