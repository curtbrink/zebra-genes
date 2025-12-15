using Generator.Zebra.Zebra.Clues.Abstract;
using Generator.Zebra.Zebra.Types;

namespace Generator.Zebra.Zebra.Clues.Types;

public record AttributeIsBeforeClue(ZebraAttribute Attribute1, ZebraAttribute Attribute2)
    : OrderedBinaryAttributeClue<AttributeIsBeforeClue>(Attribute1, Attribute2);