using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Variable;

public class Variable(string name) : IVariable
{
    public string Name { get; } = name;
}