namespace Generator.Zebra.Zebra.Types;

public record ZebraAttribute(string Category, string Attribute)
{
    // "exclusive" here meaning "same category different value"
    public bool IsExclusiveWith(ZebraAttribute? other) =>
        other is not null && other.Category == Category && other.Attribute != Attribute;
}