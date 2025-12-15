using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Variable;

public class OrderedVariable(string name, int id) : Variable(name), IOrderedVariable
{
    public int Id { get; } = id;
}