//******************************************************************************************************
//  NetworkElement.cs
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
//  02/01/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using NetworkModelEditor.Interfaces;
using NetworkModelEditor.ViewModels;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Testing;

namespace NetworkModelEditor
{
    public class NetworkElement : IParent
    {
        private object m_element;
        private Type m_elementType;

        public object Element
        {
            get
            {
                return m_element;
            }
            set
            {
                m_element = value;
            }
        }

        public Type ElementType
        {
            get
            {
                return m_elementType;
            }
        }

        public string Name
        {
            get
            {
                if (m_element is INetworkDescribable)
                {
                    if (m_element is StatusWord)
                    {
                        return (m_element as StatusWord).Description;
                    }
                    if (m_element is NetworkModel)
                    {
                        return "Model";
                    }
                    return (m_element as INetworkDescribable).Name;
                }
                else
                {
                    if (m_element is Network)
                    {
                        return "Network";
                    }
                    else if (m_element is NetworkModel)
                    {
                        return "Model";
                    }
                    else if (m_element is List<Company>)
                    {
                        return "Companies";
                    }
                    else if (m_element is List<Division>)
                    {
                        return "Divisions";
                    }
                    else if (m_element is List<Substation>)
                    {
                        return "Substations";
                    }
                    else if (m_element is List<TransmissionLine>)
                    {
                        return "Transmission Lines";
                    }
                    else if (m_element is List<Node>)
                    {
                        return "Nodes";
                    }
                    else if (m_element is List<ShuntCompensator>)
                    {
                        return "Shunt Compensators";
                    }
                    else if (m_element is List<CircuitBreaker>)
                    {
                        return "Circuit Breakers";
                    }
                    else if (m_element is List<Switch>)
                    {
                        return "Switches";
                    }
                    else if (m_element is List<Transformer>)
                    {
                        return "Transformers";
                    }
                    else if (m_element is List<SeriesCompensator>)
                    {
                        return "Series Compensators";
                    }
                    else if (m_element is List<LineSegment>)
                    {
                        return "Line Segments";
                    }
                    else if (m_element is List<VoltageLevel>)
                    {
                        return "Voltage Levels";
                    }
                    else if (m_element is List<StatusWord>)
                    {
                        return "Status Words";
                    }
                    else if (m_element is List<BreakerStatus>)
                    {
                        return "Breaker Statuses";
                    }
                    else if (m_element is List<TapConfiguration>)
                    {
                        return "Tap Configurations";
                    }
                    else if (m_element is List<RawMeasurements>)
                    {
                        return "Measurement Samples";
                    }
                    else if (m_element is RawMeasurements)
                    {
                        RawMeasurements sample = m_element as RawMeasurements;
                        return $"Sample {sample.Identifier}";
                    }
                    else
                    {
                        return "Undefined Network Element";
                    }
                }
            }
        }
        
        public List<NetworkElement> Children
        {
            get 
            {
                return GetNetworkElementChildren();
            }
        }

        public NetworkElement(object networkElement)
        {
            m_element = networkElement;
            GetNetworkElementType();
        }

        private List<NetworkElement> GetNetworkElementChildren()
        {
            if (m_element is Network)
            {
                NetworkElement networkModel = new NetworkElement((m_element as Network).Model);
                NetworkElement voltageLevels = new NetworkElement((m_element as Network).Model.VoltageLevels);
                NetworkElement statusWords = new NetworkElement((m_element as Network).Model.StatusWords);
                NetworkElement breakerStatuses = new NetworkElement((m_element as Network).Model.BreakerStatuses);
                NetworkElement tapConfigurations = new NetworkElement((m_element as Network).Model.TapConfigurations);
                return new List<NetworkElement>(new NetworkElement[] { networkModel, voltageLevels, statusWords, breakerStatuses, tapConfigurations });
            }
            else if (m_element is NetworkModel)
            {
                return ((from child in (m_element as NetworkModel).Companies select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is Company)
            {
                return ((from child in (m_element as Company).Divisions select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is Division)
            {
                NetworkElement substations = new NetworkElement((m_element as Division).Substations);
                NetworkElement transmissionLines = new NetworkElement((m_element as Division).TransmissionLines);
                return new List<NetworkElement>(new NetworkElement[] { substations, transmissionLines });
            }
            else if (m_element is List<Substation>)
            {
                return new List<NetworkElement>((from child in (m_element as List<Substation>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is Substation)
            {
                NetworkElement nodes = new NetworkElement((m_element as Substation).Nodes);
                NetworkElement shunts = new NetworkElement((m_element as Substation).Shunts);
                NetworkElement circuitBreakers = new NetworkElement((m_element as Substation).CircuitBreakers);
                NetworkElement switches = new NetworkElement((m_element as Substation).Switches);
                NetworkElement transformers = new NetworkElement((m_element as Substation).Transformers);
                return new List<NetworkElement>(new NetworkElement[] { nodes, shunts, circuitBreakers, switches, transformers });
            }
            else if (m_element is List<TransmissionLine>)
            {
                return new List<NetworkElement>((from child in (m_element as List<TransmissionLine>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is TransmissionLine)
            {
                NetworkElement nodes = new NetworkElement((m_element as TransmissionLine).Nodes);
                NetworkElement lineSegments = new NetworkElement((m_element as TransmissionLine).LineSegments);
                NetworkElement seriesCompensators = new NetworkElement((m_element as TransmissionLine).SeriesCompensators);
                NetworkElement switches = new NetworkElement((m_element as TransmissionLine).Switches);
                NetworkElement fromSubstationCurrent = new NetworkElement((m_element as TransmissionLine).FromSubstationCurrent);
                NetworkElement toSubstationCurrent = new NetworkElement((m_element as TransmissionLine).ToSubstationCurrent);
                return new List<NetworkElement>(new NetworkElement[] { nodes, lineSegments, seriesCompensators, switches, fromSubstationCurrent, toSubstationCurrent });
            }
            else if (m_element is List<Node>)
            {
                return new List<NetworkElement>((from child in (m_element as List<Node>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is Node)
            {
                NetworkElement voltage = new NetworkElement((m_element as Node).Voltage);
                return new List<NetworkElement>(new NetworkElement[] { voltage });
            }
            else if (m_element is List<Transformer>)
            {
                return new List<NetworkElement>((from child in (m_element as List<Transformer>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is Transformer)
            {
                NetworkElement fromNodeCurrent = new NetworkElement((m_element as Transformer).FromNodeCurrent);
                NetworkElement toNodeCurrent = new NetworkElement((m_element as Transformer).ToNodeCurrent);
                return new List<NetworkElement>(new NetworkElement[] { fromNodeCurrent, toNodeCurrent });
            }
            else if (m_element is List<ShuntCompensator>)
            {
                return new List<NetworkElement>((from child in (m_element as List<ShuntCompensator>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is ShuntCompensator)
            {
                NetworkElement current = new NetworkElement((m_element as ShuntCompensator).Current);
                return new List<NetworkElement>(new NetworkElement[] { current });
            }
            else if (m_element is List<Switch>)
            {
                return new List<NetworkElement>((from child in (m_element as List<Switch>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<Transformer>)
            {
                return new List<NetworkElement>((from child in (m_element as List<Transformer>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<SeriesCompensator>)
            {
                return new List<NetworkElement>((from child in (m_element as List<SeriesCompensator>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<LineSegment>)
            {
                return new List<NetworkElement>((from child in (m_element as List<LineSegment>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<CircuitBreaker>)
            {
                return new List<NetworkElement>((from child in (m_element as List<CircuitBreaker>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<VoltageLevel>)
            {
                return new List<NetworkElement>((from child in (m_element as List<VoltageLevel>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<StatusWord>)
            {
                return new List<NetworkElement>((from child in (m_element as List<StatusWord>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<BreakerStatus>)
            {
                return new List<NetworkElement>((from child in (m_element as List<BreakerStatus>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<TapConfiguration>)
            {
                return new List<NetworkElement>((from child in (m_element as List<TapConfiguration>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else if (m_element is List<RawMeasurements>)
            {
                return new List<NetworkElement>((from child in (m_element as List<RawMeasurements>) select new NetworkElement(child)).ToList<NetworkElement>());
            }
            else
            {
                return null;
            }
        }

        private void GetNetworkElementType()
        {
            if (m_element is Network)
            {
                m_elementType = typeof(Network);
            }
            else if (m_element is NetworkModel)
            {
                m_elementType = typeof(NetworkModel);
            }
            else if (m_element is Company)
            {
                m_elementType = typeof(Company);
            }
            else if (m_element is Division)
            {
                m_elementType = typeof(Division);
            }
            else if (m_element is List<Substation>)
            {
                m_elementType = typeof(List<Substation>);
            }
            else if (m_element is Substation)
            {
                m_elementType = typeof(Substation);
            }
            else if (m_element is List<TransmissionLine>)
            {
                m_elementType = typeof(List<TransmissionLine>);
            }
            else if (m_element is TransmissionLine)
            {
                m_elementType = typeof(TransmissionLine);
            }
            else if (m_element is List<Node>)
            {
                m_elementType = typeof(List<Node>);
            }
            else if (m_element is Node)
            {
                m_elementType = typeof(Node);
            }
            else if (m_element is List<ShuntCompensator>)
            {
                m_elementType = typeof(List<ShuntCompensator>);
            }
            else if (m_element is ShuntCompensator)
            {
                m_elementType = typeof(ShuntCompensator);
            }
            else if (m_element is List<SeriesCompensator>)
            {
                m_elementType = typeof(List<SeriesCompensator>);
            }
            else if (m_element is SeriesCompensator)
            {
                m_elementType = typeof(SeriesCompensator);
            }
            else if (m_element is List<LineSegment>)
            {
                m_elementType = typeof(List<LineSegment>);
            }
            else if (m_element is LineSegment)
            {
                m_elementType = typeof(LineSegment);
            }
            else if (m_element is List<CircuitBreaker>)
            {
                m_elementType = typeof(List<CircuitBreaker>);
            }
            else if (m_element is CircuitBreaker)
            {
                m_elementType = typeof(CircuitBreaker);
            }
            else if (m_element is List<VoltageLevel>)
            {
                m_elementType = typeof(List<VoltageLevel>);
            }
            else if (m_element is VoltageLevel)
            {
                m_elementType = typeof(VoltageLevel);
            }
            else if (m_element is List<StatusWord>)
            {
                m_elementType = typeof(List<StatusWord>);
            }
            else if (m_element is StatusWord)
            {
                m_elementType = typeof(StatusWord);
            }
            else if (m_element is List<BreakerStatus>)
            {
                m_elementType = typeof(List<BreakerStatus>);
            }
            else if (m_element is BreakerStatus)
            {
                m_elementType = typeof(BreakerStatus);
            }
            else if (m_element is List<TapConfiguration>)
            {
                m_elementType = typeof(List<TapConfiguration>);
            }
            else if (m_element is TapConfiguration)
            {
                m_elementType = typeof(TapConfiguration);
            }
            else if (m_element is VoltagePhasorGroup)
            {
                m_elementType = typeof(VoltagePhasorGroup);
            }
            else if (m_element is CurrentFlowPhasorGroup)
            {
                m_elementType = typeof(CurrentFlowPhasorGroup);
            }
            else if (m_element is CurrentInjectionPhasorGroup)
            {
                m_elementType = typeof(CurrentInjectionPhasorGroup);
            }
            else if (m_element is List<Transformer>)
            {
                m_elementType = typeof(List<Transformer>);
            }
            else if (m_element is Transformer)
            {
                m_elementType = typeof(Transformer);
            }
            else if (m_element is RawMeasurements)
            {
                m_elementType = typeof(RawMeasurements);
            }
            else if (m_element is RawMeasurementsMeasurement)
            {
                m_elementType = typeof(RawMeasurementsMeasurement);
            }
            else if (m_element is List<RawMeasurements>)
            {
                m_elementType = typeof(List<RawMeasurements>);
            }
            else
            {
                m_elementType = typeof(Nullable);
            }
        }

        private string GetNetworkElementXmlSource()
        {
            if (m_element != null)
            {
                // Create an XmlSerializer with the type of Network
                XmlSerializer serializer = new XmlSerializer(ElementType);

                // Open a connection to the file and path.
                StringWriter writer = new StringWriter();

                // Serialize this instance of NetworkMeasurements
                serializer.Serialize(writer, m_element);

                // Close the connection
                writer.Close();

                return writer.ToString();

            }

            return "No source available.";
        }

        public string AsXml()
        {
            return GetNetworkElementXmlSource();
        }

    }
}
