using Generator.Zebra.Clues.Abstract;
using Generator.Zebra.Types;

namespace Generator.Zebra.Clues.Types;

public record AttributeIsBeforeClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : OrderedBinaryAttributeClue<AttributeIsBeforeClue>(Attribute1, Attribute2);