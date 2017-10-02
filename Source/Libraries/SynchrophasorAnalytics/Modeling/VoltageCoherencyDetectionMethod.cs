using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Modeling
{
    [Serializable()]
    public enum VoltageCoherencyDetectionMethod
    {
        [XmlEnum("Angle")]
        AngleDelta,
        
        [XmlEnum("Magnitude")]
        MagnitudeDelta,

        [XmlEnum("TotalVector")]
        TotalVectorDelta
    }
}
