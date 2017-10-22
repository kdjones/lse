using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkModelEditor
{
    [Serializable()]
    public enum AnalysisReportType
    {
        [XmlEnum("AMatrix")]
        AMatrix,

        [XmlEnum("IIMatrix")]
        IIMatrix,

        [XmlEnum("YMatrix")]
        YMatrix,

        [XmlEnum("YsMatrix")]
        YsMatrix,

        [XmlEnum("YshMatrix")]
        YshMatrix,

        [XmlEnum("ModeledStatusWords")]
        ModeledStatusWords,

        [XmlEnum("MappedStatusWords")]
        MappedStatusWords,

        [XmlEnum("MappedLSEInvalidStatusWords")]
        MappedLSEInvalidStatusWords,

        [XmlEnum("ActiveStatusWords")]
        ActiveStatusWords,

        [XmlEnum("ReceivedMeasurements")]
        ReceivedMeasurements,

        [XmlEnum("UnreceivedMeasurements")]
        UnreceivedMeasurements,
        
        [XmlEnum("OutputMeasurements")]
        OutputMeasurements,

        [XmlEnum("PerformanceMetrics")]
        PerformanceMetrics,

        [XmlEnum("StateEstimateOutput")]
        StateEstimateOutput,

        [XmlEnum("CurrentFlowEstimateOutput")]
        CurrentFlowEstimateOutput,

        [XmlEnum("CurrentInjectionEstimateOutput")]
        CurrentInjectionEstimateOutput,

        [XmlEnum("VoltageResidualOutput")]
        VoltageResidualOutput,

        [XmlEnum("CurrentResidualOutput")]
        CurrentResidualOutput,

        [XmlEnum("CircuitBreakerStatusOutput")]
        CircuitBreakerStatusOutput,

        [XmlEnum("SwitchStatusOutput")]
        SwitchStatusOutput,

        [XmlEnum("TopologyProfilingOutput")]
        TopologyProfilingOutput,

        [XmlEnum("MeasurementValidationFlagOutput")]
        MeasurementValidationFlagOutput,

        [XmlEnum("ObservedBuses")]
        ObservedBuses,

        [XmlEnum("ModeledVoltages")]
        ModeledVoltages,

        [XmlEnum("ExpectedVoltages")]
        ExpectedVoltages,

        [XmlEnum("ActiveVoltages")]
        ActiveVoltages,

        [XmlEnum("InactiveVoltages")]
        InactiveVoltages,

        [XmlEnum("ReportedVoltages")]
        ReportedVoltages,

        [XmlEnum("UnreportedVoltages")]
        UnreportedVoltages,

        [XmlEnum("ActiveVoltagesByStatusWord")]
        ActiveVoltagesByStatusWord,

        [XmlEnum("InactiveVoltagesByStatusWord")]
        InactiveVoltagesByStatusWord,

        [XmlEnum("InactiveVoltagesByMeasurement")]
        InactiveVoltagesByMeasurement,

        [XmlEnum("ModeledCurrentFlows")]
        ModeledCurrentFlows,

        [XmlEnum("ExpectedCurrentFlows")]
        ExpectedCurrentFlows,

        [XmlEnum("InactiveCurrentFlows")]
        InactiveCurrentFlows,

        [XmlEnum("IncludedCurrentFlows")]
        IncludedCurrentFlows,

        [XmlEnum("ExcludedCurrentFlows")]
        ExcludedCurrentFlows,

        [XmlEnum("ActiveCurrentFlows")]
        ActiveCurrentFlows,

        [XmlEnum("ReportedCurrentFlows")]
        ReportedCurrentFlows,

        [XmlEnum("UnreporedCurrentFlows")]
        UnreporedCurrentFlows,

        [XmlEnum("ActiveCurrentFlowsByStatusWord")]
        ActiveCurrentFlowsByStatusWord,

        [XmlEnum("InactiveCurrentFlowsByStatusWord")]
        InactiveCurrentFlowsByStatusWord,

        [XmlEnum("InactiveCurrentFlowsByMeasurement")]
        InactiveCurrentFlowsByMeasurement,

        [XmlEnum("ModeledCurrentInjections")]
        ModeledCurrentInjections,

        [XmlEnum("ExpectedCurrentInjections")]
        ExpectedCurrentInjections,

        [XmlEnum("ActiveCurrentInjections")]
        ActiveCurrentInjections,

        [XmlEnum("InactiveCurrentInjections")]
        InactiveCurrentInjections,

        [XmlEnum("ReportedCurrentInjections")]
        ReportedCurrentInjections,

        [XmlEnum("UnreporedCurrentInjections")]
        UnreporedCurrentInjections,

        [XmlEnum("ActiveCurrentInjectionsByStatusWord")]
        ActiveCurrentInjectionsByStatusWord,

        [XmlEnum("InactiveCurrentInjectionsByStatusWord")]
        InactiveCurrentInjectionsByStatusWord,
        
        [XmlEnum("InactiveCurrentInjectionsByMeasurement")]
        InactiveCurrentInjectionsByMeasurement,

        [XmlEnum("ModeledBreakerStatuses")]
        ModeledBreakerStatuses,

        [XmlEnum("ExpectedBreakerStatuses")]
        ExpectedBreakerStatuses,

        [XmlEnum("ReportedBreakerStatuses")]
        ReportedBreakerStatuses,

        [XmlEnum("CircuitBreakers")]
        CircuitBreakers,

        [XmlEnum("Switches")]
        Switches
    }
}
