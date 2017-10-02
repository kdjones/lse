//******************************************************************************************************
//  InputOutputSettings.cs
//
//  Copyright © 2014, Kevin D. Jones.  All Rights Reserved.
//
//  This file is licensed to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/07/2014 - Kevin D. Jones
//       Generated original version of source code.
//  07/27/2014 - Kevin D. Jones
//       Added ReturnsCurrentInjections, ReturnsTapPositions, ReturnsSeriesCompensatorStatus properties
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Networks
{
    /// <summary>
    /// Represents the settings to determine what types of data the <see cref="LinearStateEstimator.Networks.Network"/> accepts as input and returns as output.
    /// </summary>
    [Serializable()]
    public class InputOutputSettings
    {
        #region [ Private Fields ]

        // Input Settings
        private bool m_acceptsMeasurements;
        private bool m_acceptsEstimates;

        // Output Settings
        private bool m_returnsStateEstimate;
        private bool m_returnsCurrentFlow;
        private bool m_returnsCurrentInjection;
        private bool m_returnsVoltageResiduals;
        private bool m_returnsCurrentResiduals;
        private bool m_returnsCircuitBreakerStatus;
        private bool m_returnsSwitchStatus;
        private bool m_returnsTapPositions;
        private bool m_returnsSeriesCompensatorStatus;
        private bool m_returnsPerformanceMetrics;
        private bool m_returnsTopologyProfilingInformation;
        private bool m_returnsMeasurementValidationFlags;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A flag that determines whether the model accepts measured values during OnNewMeasurements()
        /// </summary>
        [XmlAttribute("AcceptsMeasurements")]
        public bool AcceptsMeasurements
        {
            get
            {
                return m_acceptsMeasurements;
            }
            set
            {
                m_acceptsMeasurements = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model accepts estimated values during OnNewMeasurements()
        /// </summary>
        [XmlAttribute("AcceptsEstimates")]
        public bool AcceptsEstimates
        {
            get
            {
                return m_acceptsEstimates;
            }
            set
            {
                m_acceptsEstimates = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns state estimated values
        /// </summary>
        [XmlAttribute("ReturnsStateEstimate")]
        public bool ReturnsStateEstimate
        {
            get
            {
                return m_returnsStateEstimate;
            }
            set
            {
                m_returnsStateEstimate = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns estimated current flow values
        /// </summary>
        [XmlAttribute("ReturnsCurrentFlow")]
        public bool ReturnsCurrentFlow
        {
            get
            {
                return m_returnsCurrentFlow;
            }
            set
            {
                m_returnsCurrentFlow = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns estimated current injection values
        /// </summary>
        [XmlAttribute("ReturnsCurrentInjection")]
        public bool ReturnsCurrentInjection
        {
            get
            {
                return m_returnsCurrentInjection;
            }
            set
            {
                m_returnsCurrentInjection = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns voltage measurement residual values
        /// </summary>
        [XmlAttribute("ReturnsVoltageResiduals")]
        public bool ReturnsVoltageResiduals
        {
            get
            {
                return m_returnsVoltageResiduals;
            }
            set
            {
                m_returnsVoltageResiduals = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns current measurement residual values
        /// </summary>
        [XmlAttribute("ReturnCurrentResiduals")]
        public bool ReturnsCurrentResiduals
        {
            get
            {
                return m_returnsCurrentResiduals;
            }
            set
            {
                m_returnsCurrentResiduals = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns circuit breaker statuses
        /// </summary>
        [XmlAttribute("ReturnCircuitBreakerStatus")]
        public bool ReturnsCircuitBreakerStatus
        {
            get
            {
                return m_returnsCircuitBreakerStatus;
            }
            set
            {
                m_returnsCircuitBreakerStatus = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns switch status values
        /// </summary>
        [XmlAttribute("ReturnSwitchStatus")]
        public bool ReturnsSwitchStatus
        {
            get
            {
                return m_returnsSwitchStatus;
            }
            set
            {
                m_returnsSwitchStatus = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns tap position values
        /// </summary>
        [XmlAttribute("ReturnsTapPositions")]
        public bool ReturnsTapPositions
        {
            get
            {
                return m_returnsTapPositions;
            }
            set
            {
                m_returnsTapPositions = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns the status of the series compensators.
        /// </summary>
        [XmlAttribute("ReturnsSeriesCompensatorStatus")]
        public bool ReturnsSeriesCompensatorStatus
        {
            get
            {
                return m_returnsSeriesCompensatorStatus;
            }
            set
            {
                m_returnsSeriesCompensatorStatus = value;
            }
        }

        [XmlAttribute("ReturnsPerformanceMetrics")]
        public bool ReturnsPerformanceMetrics
        {
            get
            {
                return m_returnsPerformanceMetrics;
            }
            set
            {
                m_returnsPerformanceMetrics = value;
            }
        }

        [XmlAttribute("ReturnsTopologyProfilingInformation")]
        public bool ReturnsTopologyProfilingInformation
        {
            get
            {
                return m_returnsTopologyProfilingInformation;
            }
            set
            {
                m_returnsTopologyProfilingInformation = value;
            }
        }

        [XmlAttribute("ReturnsMeasurementValidationFlags")]
        public bool ReturnsMeasurementValidationFlags
        {
            get
            {
                return m_returnsMeasurementValidationFlags;
            }
            set
            {
                m_returnsMeasurementValidationFlags = value;
            }
        }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// The default constructor for the class. Sets <see cref="LinearStateEstimator.Networks.InputOutputSettings.AcceptsMeasurements"/> and <see cref="LinearStateEstimator.Networks.InputOutputSettings.ReturnsStateEstimate"/> to <b>true</b> and the remaining parameters to <b>false</b>.
        /// </summary>
        public InputOutputSettings()
        {
            // Default input settings
            m_acceptsMeasurements = true;
            m_acceptsEstimates = false;

            // Default output settings
            m_returnsStateEstimate = true;
            m_returnsCurrentFlow = false;
            m_returnsCurrentInjection = false;
            m_returnsVoltageResiduals = false;
            m_returnsCurrentResiduals = false;
            m_returnsCircuitBreakerStatus = false;
            m_returnsSwitchStatus = false;
            m_returnsTapPositions = false;
            m_returnsSeriesCompensatorStatus = false;
            m_returnsPerformanceMetrics = true;
            m_returnsTopologyProfilingInformation = false;
            m_returnsMeasurementValidationFlags = false;
        }

        #endregion
    }
}
