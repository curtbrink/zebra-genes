using Csp.Core.Models.Models.Csp;

namespace Csp.Core.Models.Builders;

public abstract class BaseCspBuilder<TCsp, TCspDomain> where TCsp : BaseCsp<TCspDomain>
{
    public abstract TCsp Build();
}