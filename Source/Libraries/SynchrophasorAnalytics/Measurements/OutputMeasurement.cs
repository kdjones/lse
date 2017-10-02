using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Measurements
{

    public class OutputMeasurement
    {
        private const string LINEAR_STATE_ESTIMATOR = "LSE";
        private const string VOLTAGE_PHASOR_MAGNITUDE = "VPHM";
        private const string VOLTAGE_PHASOR_ANGLE = "VPHA";
        private const string CURRENT_PHASOR_MAGNITUDE = "IPHM";
        private const string CURRENT_PHASOR_ANGLE = "IPHA";
        private const string CALCULATED_VALUE = "CALC";
        private const string DIGITAL = "DIGI";
        private const string STATISTIC = "STAT";

        private const string VOLTAGE = "V";
        private const string CURRENT = "I";
        private const string RESIDUAL = "R";
        private const string ESTIMATE = "E";
        private const string STATUS = "S";
        private const string METRIC = "M";
        private const string POSITION = "P";
        private const string EMPTY = "";

        private const string CIRCUIT_BREAKER = "CB";
        private const string SWITCH = "SW";
        private const string LINE = "LN";
        private const string TRANSFORMER = "XF";
        private const string NODE = "ND";
        private const string SHUNT = "SH";
        private const string PERFORMANCE_METRIC = "PM";
        private const string SUBSTATION = "SB";

        private string m_key;
        private double m_value;
        private string m_description;
        private string m_deviceSuffix;
        private string m_deviceId;
        private int m_internalId;
        private string m_substationName;
        private string m_pointTagOverride = "Undefined";

        private OutputType m_type;
        private MeasuredDeviceType m_measuredDeviceType;

        public MeasuredDeviceType MeasuredDeviceType
        {
            get
            {
                return m_measuredDeviceType;
            }
            set
            {
                m_measuredDeviceType = value;
            }
        }

        public int InternalId
        {
            get
            {
                return m_internalId;
            }
            set
            {
                m_internalId = value;
            }
        }

        public string DeviceId
        {
            get
            {
                return m_deviceId;
            }
            set
            {
                m_deviceId = value;
            }
        }

        public string PointTagOverride
        {
            get
            {
                return m_pointTagOverride;
            }
            set
            {
                m_pointTagOverride = value;
            }
        }

        public string SubstationName
        {
            get
            {
                return m_substationName;
            }
            set
            {
                m_substationName = value;
            }
        }

        public string PointTag
        {
            get
            {
                if (PointTagOverride != "Undefined")
                {
                    return PointTagOverride;
                }
                return GetPointTag();
            }
        }

        public string DevicePrefix
        {
            get
            {
                return LINEAR_STATE_ESTIMATOR;
            }
        }

        public string DeviceSuffix
        {
            get
            {
                return m_deviceSuffix;
            }
            set
            {
                m_deviceSuffix = value;
            }
        }

        public string Key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        public string SignalType
        {
            get
            {
                return GetSignalType();
            }
        }

        public OutputType OutputType
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        public double Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        public OutputMeasurement()
            :this("", 0.0, "")
        {

        }

        public OutputMeasurement(string key, double value, string description)
        {
            m_key = key;
            m_value = value;
            m_description = description;
        }

        private string GetSignalType()
        {
            switch (OutputType)
            {
                case OutputType.CircuitBreakerStatus:
                    return DIGITAL;
                case OutputType.CurrentFlowAngleEstimate:
                    return CURRENT_PHASOR_ANGLE;
                case OutputType.CurrentFlowAngleResidual:
                    return CURRENT_PHASOR_ANGLE;
                case OutputType.CurrentFlowMagnitudeEstimate:
                    return CURRENT_PHASOR_MAGNITUDE;
                case OutputType.CurrentFlowMagnitudeResidual:
                    return CURRENT_PHASOR_MAGNITUDE;
                case OutputType.CurrentInjectionAngleEstimate:
                    return CURRENT_PHASOR_ANGLE;
                case OutputType.CurrentInjectionAngleResidual:
                    return CURRENT_PHASOR_ANGLE;
                case OutputType.CurrentInjectionMagnitudeEstimate:
                    return CURRENT_PHASOR_MAGNITUDE;
                case OutputType.CurrentInjectionMagnitudeResidual:
                    return CURRENT_PHASOR_MAGNITUDE;
                case OutputType.VoltageMeasurementValidationFlag:
                    return DIGITAL;
                case OutputType.CurrentFlowMeasurementValidationFlag:
                    return DIGITAL;
                case OutputType.CurrentInjectionMeasurementValidationFlag:
                    return DIGITAL;
                case OutputType.PerformanceMetric:
                    return STATISTIC;
                case OutputType.SeriesCompensatorStatus:
                    return DIGITAL;
                case OutputType.SwitchStatus:
                    return DIGITAL;
                case OutputType.TapPosition:
                    return CALCULATED_VALUE;
                case OutputType.TopologyProfiling:
                    return STATISTIC;
                case OutputType.VoltageAngleEstimate:
                    return VOLTAGE_PHASOR_ANGLE;
                case OutputType.VoltageAngleResidual:
                    return VOLTAGE_PHASOR_ANGLE;
                case OutputType.VoltageMagnitudeEstimate:
                    return VOLTAGE_PHASOR_MAGNITUDE;
                case OutputType.VoltageMagnitudeResidual:
                    return VOLTAGE_PHASOR_MAGNITUDE;

            }
            return "NONE";
        }

        private string GetDeviceType()
        {
            switch (MeasuredDeviceType)
            {
                case MeasuredDeviceType.CircuitBreaker:
                    return CIRCUIT_BREAKER;
                case MeasuredDeviceType.Line:
                    return LINE;
                case MeasuredDeviceType.Node:
                    return NODE;
                case MeasuredDeviceType.Shunt:
                    return SHUNT;
                case MeasuredDeviceType.Switch:
                    return SWITCH;
                case MeasuredDeviceType.Transformer:
                    return TRANSFORMER;
                case MeasuredDeviceType.PerformanceMetric:
                    return PERFORMANCE_METRIC;
                case MeasuredDeviceType.Substation:
                    return SUBSTATION;

            }
            return "NONE";
        }

        private string GetPointTagPrefix()
        {
            return $"{DeviceId}_{GetDeviceType()}";
        }

        private string GetPointTagSuffix()
        {
            switch (OutputType)
            {
                case OutputType.CircuitBreakerStatus:
                    return STATUS;
                case OutputType.CurrentFlowAngleEstimate:
                    return $"{CURRENT}A{ESTIMATE}{InternalId}";
                case OutputType.CurrentFlowAngleResidual:
                    return $"{CURRENT}A{RESIDUAL}{InternalId}";
                case OutputType.CurrentFlowMagnitudeEstimate:
                    return $"{CURRENT}M{ESTIMATE}{InternalId}";
                case OutputType.CurrentFlowMagnitudeResidual:
                    return $"{CURRENT}M{RESIDUAL}{InternalId}";
                case OutputType.CurrentInjectionAngleEstimate:
                    return $"{CURRENT}A{ESTIMATE}{InternalId}";
                case OutputType.CurrentInjectionAngleResidual:
                    return $"{CURRENT}A{RESIDUAL}{InternalId}";
                case OutputType.CurrentInjectionMagnitudeEstimate:
                    return $"{CURRENT}M{ESTIMATE}{InternalId}";
                case OutputType.CurrentInjectionMagnitudeResidual:
                    return $"{CURRENT}M{RESIDUAL}{InternalId}";
                case OutputType.VoltageMeasurementValidationFlag:
                    return $"V{STATUS}{InternalId}";
                case OutputType.CurrentFlowMeasurementValidationFlag:
                    return $"FLOW{STATUS}{InternalId}";
                case OutputType.CurrentInjectionMeasurementValidationFlag:
                    return $"INJ{STATUS}{InternalId}";
                case OutputType.PerformanceMetric:
                    return $"{METRIC}{InternalId}";
                case OutputType.SeriesCompensatorStatus:
                    return $"{POSITION}{InternalId}";
                case OutputType.SwitchStatus:
                    return $"{STATUS}{InternalId}";
                case OutputType.TapPosition:
                    return $"{POSITION}{InternalId}";
                case OutputType.TopologyProfiling:
                    return $"{POSITION}{InternalId}";
                case OutputType.VoltageAngleEstimate:
                    return $"{VOLTAGE}A{ESTIMATE}{InternalId}";
                case OutputType.VoltageAngleResidual:
                    return $"{VOLTAGE}A{RESIDUAL}{InternalId}";
                case OutputType.VoltageMagnitudeEstimate:
                    return $"{VOLTAGE}M{ESTIMATE}{InternalId}";
                case OutputType.VoltageMagnitudeResidual:
                    return $"{VOLTAGE}M{RESIDUAL}{InternalId}";

            }
            return "NONE";
        }
        
        private string GetPointTag()
        {
            return $"{GetPointTagPrefix()}:{GetPointTagSuffix()}";
        }

        public override string ToString()
        {
            return $"{DevicePrefix}!{DeviceSuffix} | {PointTag} | {SignalType} | {Description}";
        }

        public string ToVerboseString()
        {
            return $"{PointTag} | {Value} | {Description}  | {DevicePrefix}!{DeviceSuffix} | {SignalType} | {Key}";
        }
    }
}
