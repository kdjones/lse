using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementSampler
{
    public interface IAlgorithmHost
    {
        string SpecialStatus
        {
            get;
            set;
        }
    }
}
