using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Builders;

public partial class ZebraBuilder
{
    public class ZebraConstraintBuilder
    {
        private readonly ZebraBuilder _builder;
        private readonly IVariable _primary;

        internal ZebraConstraintBuilder(ZebraBuilder builder, IVariable primary)
        {
            _builder = builder;
            _primary = primary;
        }

        public ZebraBuilder MustBeInPosition(int pos) => MustBeInPosition([pos]);

        public ZebraBuilder MustBeInPosition(List<int> pos)
        {
            var constraint = new OneOfConstraint(_primary, pos);
            return AddAndClose(constraint);
        }

        public ZebraBuilder Has(string other) => Is(other);

        public ZebraBuilder Is(string other)
        {
            var otherVar = _builder.GetVariable(other);
            var constraint = new EqualsConstraint(_primary, otherVar);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsAdjacentTo(string other)
        {
            var otherVar = _builder.GetVariable(other);
            var constraint = new AdjacentConstraint(_primary, otherVar);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsBefore(string other) => IsBefore(other, false);

        public ZebraBuilder IsAfter(string other) => IsBefore(other, true);

        private ZebraBuilder IsBefore(string other, bool isAfter)
        {
            var otherVar = _builder.GetVariable(other);
            var before = isAfter ? otherVar : _primary;
            var after = isAfter ? _primary : otherVar;
            var constraint = new BeforeConstraint(before, after);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsImmediatelyBefore(string other) => IsNBefore(other, 1);

        public ZebraBuilder IsImmediatelyAfter(string other) => IsNAfter(other, 1);
        
        public ZebraBuilder IsNBefore(string other, int pos) => IsNBefore(other, pos, false);
        
        public ZebraBuilder IsNAfter(string other, int pos) => IsNBefore(other, pos, true);

        private ZebraBuilder IsNBefore(string other, int n, bool reverse)
        {
            var otherVar = _builder.GetVariable(other);
            var before = reverse ? otherVar : _primary;
            var after = reverse ? _primary : otherVar;
            var constraint = new OffsetConstraint(before, after, n);
            return AddAndClose(constraint);
        }

        private ZebraBuilder AddAndClose(IConstraint<int> constraint)
        {
            if (!CheckForDuplicates(constraint)) return _builder;
            
            var conflictingConstraint = CheckForConflicts(constraint);
            if (conflictingConstraint != null)
            {
                throw new Exception(
                    $"Constraint conflicts with existing constraint \"{conflictingConstraint.Description}\"");
            }
            
            _builder._constraints.Add(constraint);

            return _builder;
        }

        private bool CheckForDuplicates(IConstraint<int> newConstraint)
        {
            // todo this would be useful for idempotency!
            return true;
        }

        private IConstraint<int>? CheckForConflicts(IConstraint<int> newConstraint)
        {
            // todo catch low-hanging logical fruit such as:
            // - equals on the same category
            // - same OneOfs on the same category
            // - etc
            return null;
        }
    }
}