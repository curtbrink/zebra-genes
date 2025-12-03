using Csp.Interfaces;

namespace Csp.Impl;

public class BaseVariable(string name) : IVariable
{
    public string Name { get; } = name;
}