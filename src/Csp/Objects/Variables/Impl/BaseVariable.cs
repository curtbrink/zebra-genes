using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Variables.Impl;

public class BaseVariable(string name) : IVariable
{
    public string Name { get; } = name;
}