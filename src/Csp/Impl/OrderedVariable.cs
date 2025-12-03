using Csp.Interfaces;

namespace Csp.Impl;

public class OrderedVariable(string name, int id) : BaseVariable(name), IOrderedVariable
{
    public int Id { get; } = id;
}