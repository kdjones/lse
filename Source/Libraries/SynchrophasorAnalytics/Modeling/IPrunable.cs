using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Modeling
{
    public interface IPrunable
    {
        bool RetainWhenPruning
        {
            get;
        }
    }
}
