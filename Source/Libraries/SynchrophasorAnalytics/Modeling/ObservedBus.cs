//******************************************************************************************************
//  ObservedBus.cs
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
//  06/01/2013 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a collection of <see cref="LinearStateEstimator.Modeling.Node"/> objects which topology processing has determined are directly electrically connected together.
    /// </summary>
    public class ObservedBus : INetworkDescribable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;

        #endregion

        #region [ Private Members ]

        private int m_internalID;
        private List<Node> m_observedNodes;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.ObservedBus"/> which is intended to be unique among other objects of the same type.
        /// </summary>
        public int InternalID
        {
            get
            {
                return m_internalID;
            }
            set
            {
                m_internalID = value;
            }
        }

        /// <summary>
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.ObservedBus"/>. Will always return the same value as <see cref="LinearStateEstimator.Modeling.ObservedBus.InternalID"/> and the setter will take no action.
        /// </summary>
        public int Number
        {
            get
            {
                return m_internalID;
            }
            set
            {
                // Does nothing but must exist do to INetworkDescribable interface.
            }
        }

        /// <summary>
        /// Returns a string which contains a list of all of the <see cref="LinearStateEstimator.Modeling.Node.Acronym"/> of the nodes in the <see cref="LinearStateEstimator.Modeling.ObservedBus"/>. The setter will take no action.
        /// </summary>
        public string Acronym
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_observedNodes)
                {
                    stringBuilder.AppendFormat(node.Acronym + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }

        /// <summary>
        /// Returns a string which contains a list of all of the <see cref="LinearStateEstimator.Modeling.Node.Name"/> of the nodes in the <see cref="LinearStateEstimator.Modeling.ObservedBus"/>. The setter will take no action.
        /// </summary>
        public string Name
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_observedNodes)
                {
                    stringBuilder.AppendFormat(node.Name + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }

        /// <summary>
        /// Returns a string which contains a list of all of the <see cref="LinearStateEstimator.Modeling.Node.Description"/> of the nodes in the <see cref="LinearStateEstimator.Modeling.ObservedBus"/>. The setter will take no action.
        /// </summary>
        public string Description
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_observedNodes)
                {
                    stringBuilder.AppendFormat(node.Description + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the type of the object as a string.
        /// </summary>
        public string ElementType
        {
            get 
            { 
                return this.GetType().ToString(); 
            }
        }

        /// <summary>
        /// The list of <see cref="LinearStateEstimator.Modeling.Node"/> which belong to the <see cref="LinearStateEstimator.Modeling.ObservedBus"/>.
        /// </summary>
        public List<Node> Nodes
        {
            get
            {
                return m_observedNodes;
            }
            set
            {
                m_observedNodes = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the collection of <see cref="LinearStateEstimator.Modeling.ObservedBus"/>. If the list is empty, an exception will be thrown.
        /// </summary>
        public VoltageLevel BaseKV
        {
            get
            {
                if (m_observedNodes.Count < 1)
                {
                    throw new Exception("Observed Bus cannot have a voltage level without having children nodes");
                }
                else
                {
                    return m_observedNodes[0].BaseKV.DeepCopy();
                }
            }
        }

        /// <summary>
        /// Sets the value of the voltage phasor for the <see cref="LinearStateEstimator.Modeling.Node.Voltage"/> of each of the <see cref="LinearStateEstimator.Modeling.Node"/> objects.
        /// </summary>
        public VoltagePhasorGroup Value
        {
            set
            {
                foreach (Node node in m_observedNodes)
                {
                    node.Voltage.PositiveSequence.Estimate.ComplexPhasor = value.PositiveSequence.Estimate.ComplexPhasor;
                    node.Voltage.PhaseA.Estimate.ComplexPhasor = value.PhaseA.Estimate.ComplexPhasor;
                    node.Voltage.PhaseB.Estimate.ComplexPhasor = value.PhaseB.Estimate.ComplexPhasor;
                    node.Voltage.PhaseC.Estimate.ComplexPhasor = value.PhaseC.Estimate.ComplexPhasor;
                }
            }
        }

        public Complex AveragePerUnitComplexVoltage
        {
            get
            {
                return ComputeAverageComplexVoltage();
            }
        }

        public double AveragePerUnitVoltageMagnitude
        {
            get
            {
                return ComputeAveragePerUnitMagnitude();
            }
        }

        public double AverageVoltageAngle
        {
            get
            {
                return ComputeAverageVoltageAngle();
            }
        }


        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values and an empty list of <see cref="LinearStateEstimator.Modeling.Node"/>.
        /// </summary>
        public ObservedBus()
            :this(DEFAULT_INTERNAL_ID, new List<Node>())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> for when there is only one known <see cref="LinearStateEstimator.Modeling.Node"/> at the time of construction.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.ObservedBus"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="observedNode">A single <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        public ObservedBus(int internalID, Node observedNode)
        {
            m_internalID = internalID;
            m_observedNodes = new List<Node>();
            m_observedNodes.Add(observedNode);
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> for when there is only one known <see cref="LinearStateEstimator.Modeling.Node"/> at the time of construction.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.ObservedBus"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="observedNodes">A list of <see cref="LinearStateEstimator.Modeling.Node"/> objects.</param>
        public ObservedBus(int internalID, List<Node> observedNodes)
        {
            m_internalID = internalID;
            m_observedNodes = observedNodes;
        }

        #endregion

        #region [ Public Methods ]

        public bool IsCoherentWith(Node node)
        {
            foreach (Node observedNode in m_observedNodes)
            {

                if (node.ParentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.AngleDelta)
                {
                    double a_cos = Math.Cos(AveragePerUnitComplexVoltage.Phase);
                    double a_sin = Math.Sin(AveragePerUnitComplexVoltage.Phase);
                    double b_cos = Math.Cos(node.Voltage.PositiveSequence.Measurement.AngleInRadians);
                    double b_sin = Math.Sin(node.Voltage.PositiveSequence.Measurement.AngleInRadians);
                    Complex a = new Complex(a_cos, a_sin);
                    Complex b = new Complex(b_cos, b_sin);
                    Complex delta = a - b;
                    if ((delta.Phase * 180 / Math.PI) <= node.ParentSubstation.AngleDeltaThresholdInDegrees)
                    {
                        return true;
                    }
                }
                else if (node.ParentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.MagnitudeDelta)
                {
                    double a = AveragePerUnitVoltageMagnitude;
                    double b = node.Voltage.PositiveSequence.Measurement.PerUnitMagnitude;
                    double delta = a - b;
                    if (delta <= node.ParentSubstation.PerUnitMagnitudeDeltaThreshold)
                    {
                        return true;
                    }
                }
                else if (node.ParentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.TotalVectorDelta)
                {
                    Complex a = AveragePerUnitComplexVoltage;
                    Complex b = node.Voltage.PositiveSequence.Measurement.PerUnitComplexPhasor;
                    Complex delta = a - b;
                    if (delta.Magnitude <= node.ParentSubstation.TotalVectorDeltaThreshold)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public Complex ComputeAverageComplexVoltage()
        {
            double nodeCount = (double)m_observedNodes.Count;
            Complex totalValue = new Complex(0.0, 0.0);

            foreach (Node node in m_observedNodes)
            {
                totalValue += node.Voltage.PositiveSequence.Measurement.PerUnitComplexPhasor;
            }

            return new Complex((totalValue.Real)/nodeCount, totalValue.Imaginary/nodeCount);
        }

        public double ComputeAveragePerUnitMagnitude()
        {
            return ComputeAverageComplexVoltage().Magnitude;
        }

        public double ComputeAverageVoltageAngle()
        {
            return ComputeAverageComplexVoltage().Phase;
        }


        public void MergeWith(ObservedBus bus)
        {
            m_observedNodes.AddRange(bus.Nodes);
        }

        /// <summary>
        /// A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> class and each of its <see cref="LinearStateEstimator.Modeling.Node"/> objects.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> class.</returns>
        public override string ToString()
        {
            string observedBusAsString = "";
            foreach (Node node in m_observedNodes)
            {
                observedBusAsString += node.InternalID + "|" + node.Name + ",";
            }
            observedBusAsString.Remove(observedBusAsString.Length - 1);
            return "-OBS- ID: " + m_internalID.ToString() + "  " + observedBusAsString;
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> class.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ObservedBus"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Observed Bus -------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            foreach (Node node in m_observedNodes)
            {
                stringBuilder.AppendFormat(node.ToVerboseString() + "{0}", Environment.NewLine);
            }
            
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        #endregion
    }
}
