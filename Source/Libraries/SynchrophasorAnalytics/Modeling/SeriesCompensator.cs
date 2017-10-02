//******************************************************************************************************
//  SeriesCompensator.cs
//
//  Copyright © 2013, Kevin D. Jones.  All Rights Reserved.
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
//  09/17/2013 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a series capacitor or reactor in an electric power network.
    /// </summary>
    [Serializable()]
    public class SeriesCompensator : LineSegment
    {
        #region [ Private Constants ]

        /// <summary>
        /// <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> defaults.
        /// </summary>
        private const int DEFAULT_INTERNALID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Undefined";

        private const string DEFAULT_OUTPUT_MEASUREMENT_KEY = "Undefined";

        #endregion

        #region [ Private Members ]

        private string m_outputMeasurementKey;
        private SeriesCompensatorStatus m_status;
        private SeriesCompensatorType m_type;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The output measurement key for the status of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.
        /// </summary>
        [XmlAttribute("Key")]
        public string OutputMeasurementKey
        {
            get
            {
                return m_outputMeasurementKey;
            }
            set
            {
                m_outputMeasurementKey = value;
            }
        }

        /// <summary>
        /// The type of series compensator, either <see cref="LinearStateEstimator.Modeling.SeriesCompensatorType.Capacitor"/> or <see cref="LinearStateEstimator.Modeling.SeriesCompensatorType.Reactor"/>.
        /// </summary>
        [XmlAttribute("Type")]
        public SeriesCompensatorType Type
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

        /// <summary>
        /// The status of the series compensator, either <see cref="LinearStateEstimator.Modeling.SeriesCompensatorStatus.Bypassed"/> or <see cref="LinearStateEstimator.Modeling.SeriesCompensatorStatus.Energized"/>.
        /// </summary>
        [XmlAttribute("Status")]
        public SeriesCompensatorStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public SeriesCompensator()
            : this(DEFAULT_INTERNALID, DEFAULT_NUMBER, DEFAULT_NAME, DEFAULT_DESCRIPTION, new Impedance(), DEFAULT_OUTPUT_MEASUREMENT_KEY)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> class which requires the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> parameters, the impedance of the device, and the output measurement key for reporting the status.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>. There are no restrictions on uniqueness. </param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="impedance">The <see cref="LinearStateEstimator.Modeling.Impedance"/> of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="measurementKey">The output measurement key for returning the <see cref="LinearStateEstimator.Modeling.SeriesCompensatorStatus"/> of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        public SeriesCompensator(int internalID, int number, string name, string description, Impedance impedance, string measurementKey)
            : this(internalID, number, name, description, impedance, 0, measurementKey)
        {
            m_status = SeriesCompensatorStatus.Bypassed;
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> class which requires the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> parameters, the impedance of the device, and the output measurement key for reporting the status and the internal id of the parent transmission line.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>. There are no restrictions on uniqueness. </param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="impedance">The <see cref="LinearStateEstimator.Modeling.Impedance"/> of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        /// <param name="parentTransmissionLineID">The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.LineSegment.ParentTransmissionLine"/>.</param>
        /// <param name="measurementKey">The output measurement key for returning the <see cref="LinearStateEstimator.Modeling.SeriesCompensatorStatus"/> of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>.</param>
        public SeriesCompensator(int internalID, int number, string name, string description, Impedance impedance, int parentTransmissionLineID, string measurementKey)
            : base(internalID, number, name, description, impedance)
        {
            m_outputMeasurementKey = measurementKey;
            m_status = SeriesCompensatorStatus.Bypassed;
            m_type = SeriesCompensatorType.Capacitor;
            this.RawImpedanceParameters.ParentElement = this;
        }

        #endregion

        #region [ Public Methods ]

        public void Unkeyify()
        {
            OutputMeasurementKey = DEFAULT_OUTPUT_MEASUREMENT_KEY;
        }

        /// <summary>
        /// A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> class.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="SeriesCompensator"/> class.</returns>
        public override string ToString()
        {
            return "SeriesCompensator," + InternalID.ToString() + "," + Number.ToString() + " Line: " + ParentTransmissionLine.Number.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> class.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Series Compensator -------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("ParntTrnsmsnLine: " + ParentTransmissionLine.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Type: " + Type.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Status: " + Status.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(RawImpedanceParameters.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        #endregion
    }
}
