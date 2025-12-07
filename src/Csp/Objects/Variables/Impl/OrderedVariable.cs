using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Variables.Impl;

public class OrderedVariable(string name, int id) : BaseVariable(name), IOrderedVariable
{
    public int Id { get; } = id;
}