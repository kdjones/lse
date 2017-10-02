using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Indicates what the inferred state of a <see cref="LinearStateEstimator.Modeling.SwitchingDeviceBase"/> is.
    /// </summary>
    [Serializable()]
    public enum SwitchingDeviceInferredState
    {
        /// <summary>
        /// Represents a switching device that is normally open
        /// </summary>
        [XmlEnum("Open")]
        Open,

        /// <summary>
        /// Represents a switching device that is normall closed
        /// </summary>
        [XmlEnum("Closed")]
        Closed,

        /// <summary>
        /// Represents a unknown state.
        /// </summary>
        [XmlEnum("Uknown")]
        Unknown
    }
}
