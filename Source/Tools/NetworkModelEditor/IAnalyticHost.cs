using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Testing;

namespace NetworkModelEditor
{
    public interface IAnalyticHost
    {
        #region [ Status Bar ]

        string ActionStatus
        {
            get;
            set;
        }

        string CommunicationStatus
        {
            get;
            set;
        }

        void AddMeasurementSample(RawMeasurements sample);
        #endregion
    }
}
