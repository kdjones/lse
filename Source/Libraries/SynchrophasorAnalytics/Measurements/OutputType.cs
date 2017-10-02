using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Measurements
{
    public enum OutputType
    {
        VoltageMagnitudeEstimate,
        CurrentFlowMagnitudeEstimate,
        CurrentInjectionMagnitudeEstimate,
        VoltageAngleEstimate,
        CurrentFlowAngleEstimate,
        CurrentInjectionAngleEstimate,
        VoltageMagnitudeResidual,
        CurrentFlowMagnitudeResidual,
        CurrentInjectionMagnitudeResidual,
        VoltageAngleResidual,
        CurrentFlowAngleResidual,
        CurrentInjectionAngleResidual,
        CircuitBreakerStatus,
        SwitchStatus,
        TapPosition,
        SeriesCompensatorStatus,
        PerformanceMetric,
        TopologyProfiling,
        VoltageMeasurementValidationFlag,
        CurrentFlowMeasurementValidationFlag,
        CurrentInjectionMeasurementValidationFlag,
    }
}
