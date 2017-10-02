//******************************************************************************************************
//  NetworkElementFactory.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using NetworkModelEditor.ViewModels;
using System.Windows;

namespace NetworkModelEditor
{
    public class NetworkElementFactory
    {
        #region [ Private Constants ]

        private const int MAXIMUM_INTERNAL_ID = 99999;

        #endregion

        internal static void CreateNewChildElement(NetworkElementViewModel parentNetworkElement)
        {
            if (parentNetworkElement.Value.Element is NetworkModel)
            {
                CreateNewCompany(parentNetworkElement.Value.Element as NetworkModel);
            }
            else if (parentNetworkElement.Value.Element is Company)
            {
                CreateNewDivision(parentNetworkElement.Value.Element as Company);
            }
            else if (parentNetworkElement.Value.Element is List<Substation>)
            {
                CreateNewSubstation(parentNetworkElement.Parent.Value.Element as Division);
            }
            else if (parentNetworkElement.Value.Element is List<TransmissionLine>)
            {
                CreateNewTransmissionLine(parentNetworkElement.Parent.Value.Element as Division);
            }
            else if (parentNetworkElement.Value.Element is List<Node>)
            {
                if (parentNetworkElement.Parent.Value.Element is Substation)
                {
                    CreateNewNode(parentNetworkElement.Parent.Value.Element as Substation);
                }
                else if (parentNetworkElement.Parent.Value.Element is TransmissionLine)
                {
                    CreateNewNode(parentNetworkElement.Parent.Value.Element as TransmissionLine);
                }
            }
            else if (parentNetworkElement.Value.Element is List<ShuntCompensator>)
            {
                CreateNewShunt(parentNetworkElement.Parent.Value.Element as Substation);
            }
            else if (parentNetworkElement.Value.Element is List<CircuitBreaker>)
            {
                CreateNewCircuitBreaker(parentNetworkElement.Parent.Value.Element as Substation);
            }
            else if (parentNetworkElement.Value.Element is List<Switch>)
            {
                if (parentNetworkElement.Parent.Value.Element is Substation)
                {
                    CreateNewSwitch(parentNetworkElement.Parent.Value.Element as Substation);
                }
                else if (parentNetworkElement.Parent.Value.Element is TransmissionLine)
                {
                    CreateNewSwitch(parentNetworkElement.Parent.Value.Element as TransmissionLine);
                }
            }
            else if (parentNetworkElement.Value.Element is List<Transformer>)
            {
                CreateNewTransformer(parentNetworkElement.Parent.Value.Element as Substation);
            }
            else if (parentNetworkElement.Value.Element is List<LineSegment>)
            {
                CreateNewLineSegment(parentNetworkElement.Parent.Value.Element as TransmissionLine);
            }
            else if (parentNetworkElement.Value.Element is List<SeriesCompensator>)
            {
                CreateNewSeriesCompensator(parentNetworkElement.Parent.Value.Element as TransmissionLine);
            }
            else if (parentNetworkElement.Value.Element is List<VoltageLevel>)
            {
                CreateNewVoltageLevel(parentNetworkElement.Value.Element as List<VoltageLevel>);
            }
            else if (parentNetworkElement.Value.Element is List<StatusWord>)
            {
                CreateNewStatusWord(parentNetworkElement.Value.Element as List<StatusWord>);
            }
            else if (parentNetworkElement.Value.Element is List<BreakerStatus>)
            {
                CreateNewBreakerStatus(parentNetworkElement.Value.Element as List<BreakerStatus>);
            }
            else if (parentNetworkElement.Value.Element is List<TapConfiguration>)
            {
                CreateNewTapConfiguration(parentNetworkElement.Value.Element as List<TapConfiguration>);
            }
        }

        #region [ Series Compensators ]

        private static void CreateNewSeriesCompensator(TransmissionLine parentTransmissionLine)
        {
            int nextAvailableInternalId = GetNextAvailableSeriesCompensatorInternalId(parentTransmissionLine);

            SeriesCompensator seriesCompensator = new SeriesCompensator()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "SRSCMP " + nextAvailableInternalId.ToString(),
                Name = parentTransmissionLine.Name + " Series Compensator " + nextAvailableInternalId.ToString(),
                Description = parentTransmissionLine.Name + " Series Compensator " + nextAvailableInternalId.ToString() + " Description",
                ParentTransmissionLine = parentTransmissionLine,
                Type = SeriesCompensatorType.Capacitor,
                OutputMeasurementKey = "Undefined"
            };

            if (parentTransmissionLine.FromNode != null)
            {
                seriesCompensator.FromNode = parentTransmissionLine.FromNode;
            }
            if (parentTransmissionLine.ToNode != null)
            {
                seriesCompensator.ToNode = parentTransmissionLine.ToNode;
            }

            parentTransmissionLine.SeriesCompensators.Add(seriesCompensator);

        }

        private static int GetNextAvailableSeriesCompensatorInternalId(TransmissionLine parentTransmissionLine)
        {
            return GetNextAvailableInternalId(GetSeriesCompensatorInternalIds(parentTransmissionLine));
        }

        private static List<int> GetSeriesCompensatorInternalIds(TransmissionLine parentTransmissionLine)
        {
            List<int> seriesCompensatorInternalIds = new List<int>();

            foreach (Company company in parentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (SeriesCompensator seriesCompensator in transmissionLine.SeriesCompensators)
                        {
                            seriesCompensatorInternalIds.Add(seriesCompensator.InternalID);
                        }
                    }
                }
            }
            return seriesCompensatorInternalIds;
        }

        #endregion

        #region [ Tap Configurations ]

        private static void CreateNewTapConfiguration(List<TapConfiguration> tapConfigurations)
        {
            int nextAvailableInternalId = GetNextAvailableTapConfigurationInternalId(tapConfigurations);

            tapConfigurations.Add(new TapConfiguration()
            {
                InternalID = nextAvailableInternalId,
                Name = "Tap Configuration",
                Description = "Tap Configuration Description"
            });
        }

        private static int GetNextAvailableTapConfigurationInternalId(List<TapConfiguration> tapConfigurations)
        {
            return GetNextAvailableInternalId(GetTapConfigurationInternalIds(tapConfigurations));
        }

        private static List<int> GetTapConfigurationInternalIds(List<TapConfiguration> tapConfigurations)
        {
            List<int> tapConfigurationInternalIds = new List<int>();

            foreach (TapConfiguration tapConfiguration in tapConfigurations)
            {
                tapConfigurationInternalIds.Add(tapConfiguration.InternalID);
            }

            return tapConfigurationInternalIds;
        }

        #endregion

        #region [ Breaker Statuses ]

        private static void CreateNewBreakerStatus(List<BreakerStatus> breakerStatuses)
        {
            int nextAvailableInternalId = GetNextAvailableBreakerStatusInternalId(breakerStatuses);

            breakerStatuses.Add(new BreakerStatus()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                IsEnabled = true
            });
        }

        private static int GetNextAvailableBreakerStatusInternalId(List<BreakerStatus> breakerStatuses)
        {
            return GetNextAvailableInternalId(GetBreakerStatusInternalIds(breakerStatuses));
        }

        private static List<int> GetBreakerStatusInternalIds(List<BreakerStatus> breakerStatuses)
        {
            List<int> breakerStatusInternalIds = new List<int>();

            foreach (BreakerStatus breakerStatus in breakerStatuses)
            {
                breakerStatusInternalIds.Add(breakerStatus.InternalID);
            }

            return breakerStatusInternalIds;
        }

        #endregion

        #region [ Status Words ]

        private static void CreateNewStatusWord(List<StatusWord> statusWords)
        {
            int nextAvailableInternalId = GetNextAvailableStatusWordInternalId(statusWords);

            statusWords.Add(new StatusWord()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                IsEnabled = true
            });
        }

        private static int GetNextAvailableStatusWordInternalId(List<StatusWord> statusWords)
        {
            return GetNextAvailableInternalId(GetStatusWordInternalIds(statusWords));
        }

        private static List<int> GetStatusWordInternalIds(List<StatusWord> statusWords)
        {
            List<int> statusWordInternalIds = new List<int>();

            foreach (StatusWord statusWord in statusWords)
            {
                statusWordInternalIds.Add(statusWord.InternalID);
            }

            return statusWordInternalIds;
        }

        #endregion

        #region [ Voltage Levels ]

        private static void CreateNewVoltageLevel(List<VoltageLevel> voltageLevels)
        {
            int nextAvailableInternalId = GetNextAvailableVoltageLevelInternalId(voltageLevels);

            voltageLevels.Add(new VoltageLevel()
            {
                InternalID = nextAvailableInternalId,
                Value = 500
            });
        }

        private static int GetNextAvailableVoltageLevelInternalId(List<VoltageLevel> voltageLevels)
        {
            return GetNextAvailableInternalId(GetVoltageLevelInternalIds(voltageLevels));
        }

        private static List<int> GetVoltageLevelInternalIds(List<VoltageLevel> voltageLevels)
        {
            List<int> voltageLevelInternalIds = new List<int>();

            foreach (VoltageLevel voltageLevel in voltageLevels)
            {
                voltageLevelInternalIds.Add(voltageLevel.InternalID);
            }

            return voltageLevelInternalIds;
        }

        #endregion

        #region [ Companies ]

        internal static void CreateNewCompany(NetworkModel parentModel)
        {
            int nextAvailableInternalId = GetNextAvailableCompanyInternalId(parentModel);

            parentModel.Companies.Add(new Company()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "CO " + nextAvailableInternalId.ToString(),
                Name = "Company " + nextAvailableInternalId.ToString(),
                Description = "Company " + nextAvailableInternalId.ToString() + " Description",
                ParentModel = parentModel
            });
        }

        private static int GetNextAvailableCompanyInternalId(NetworkModel parentModel)
        {
            return GetNextAvailableInternalId(GetCompanyInternalIds(parentModel));
        }

        private static List<int> GetCompanyInternalIds(NetworkModel parentModel)
        {
            List<int> companyInternalIds = new List<int>();

            foreach (Company company in parentModel.Companies)
            {
                companyInternalIds.Add(company.InternalID);
            }

            return companyInternalIds;
        }

        #endregion

        #region [ Divisions ]

        internal static void CreateNewDivision(Company parentCompany)
        {
            int nextAvailableInternalId = GetNextAvailableDivisionInternalId(parentCompany);

            parentCompany.Divisions.Add(new Division()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "DIV " + nextAvailableInternalId.ToString(),
                Name = parentCompany.Name + " Division " + nextAvailableInternalId.ToString(),
                Description = parentCompany.Name + " Division " + nextAvailableInternalId.ToString() + " Description",
                ParentCompany = parentCompany
            });
        }

        private static int GetNextAvailableDivisionInternalId(Company parentCompany)
        {
            return GetNextAvailableInternalId(GetDivisionInternalIds(parentCompany));
        }

        private static List<int> GetDivisionInternalIds(Company parentCompany)
        {
            List<int> divisionInternalIds = new List<int>();

            foreach (Company company in parentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    divisionInternalIds.Add(division.InternalID);
                }
            }

            return divisionInternalIds;
        }

        #endregion

        #region [ Substations ]

        internal static void CreateNewSubstation(Division parentDivision)
        {
            int nextAvailableInternalID = GetNextAvailableSubstationInternalId(parentDivision);

            parentDivision.Substations.Add(new Substation()
            {
                InternalID = nextAvailableInternalID,
                Number = nextAvailableInternalID,
                Acronym = "SUB " + nextAvailableInternalID.ToString(),
                Name = parentDivision.Name + " Substation " + nextAvailableInternalID.ToString(),
                Description = parentDivision.Name + " Substation " + nextAvailableInternalID.ToString() + " Description",
                ParentDivision = parentDivision
            });
        }

        private static int GetNextAvailableSubstationInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetSubstationInternalIds(parentDivision));
        }

        private static List<int> GetSubstationInternalIds(Division parentDivision)
        {
            List<int> substationInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        substationInternalIds.Add(substation.InternalID);
                    }
                }
            }

            return substationInternalIds;
        }

        #endregion

        #region [ Transmission Lines ]

        internal static void CreateNewTransmissionLine(Division parentDivision)
        {
            int nextAvailableInternalId = GetNextAvailableTransmissionLineInternalId(parentDivision);
            int[] nextTwoAvailableCurrentFlowInternalIds = GetNextTwoAvailableCurrentFlowPhasorGroupInternalIds(parentDivision);

            while (parentDivision.Substations.Count < 2)
            {
                CreateNewSubstation(parentDivision);
            }

            TransmissionLine transmissionLine = new TransmissionLine()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "LINE " + nextAvailableInternalId.ToString(),
                Name = parentDivision.Name + " Transmission Line " + nextAvailableInternalId.ToString(),
                Description = parentDivision.Name + " Transmission Line " + nextAvailableInternalId.ToString() + " Description",
                ParentDivision = parentDivision,
                FromSubstation = parentDivision.Substations[0],
                ToSubstation = parentDivision.Substations[1],
            };

            while (transmissionLine.FromSubstation.Nodes.Count < 1)
            {
                CreateNewNode(transmissionLine.FromSubstation);
            }

            while (transmissionLine.ToSubstation.Nodes.Count < 1)
            {
                CreateNewNode(transmissionLine.ToSubstation);
            }

            transmissionLine.FromNode = transmissionLine.FromSubstation.Nodes[0];
            transmissionLine.ToNode = transmissionLine.ToSubstation.Nodes[0];

            transmissionLine.FromSubstationCurrent = new CurrentFlowPhasorGroup()
            {
                InternalID = nextTwoAvailableCurrentFlowInternalIds[0],
                Number = nextTwoAvailableCurrentFlowInternalIds[0],
                Acronym = "I " + nextAvailableInternalId.ToString(),
                Name = "From Phasor Name",
                Description = "From Current Flow Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredBranch = transmissionLine,
                MeasuredFromNode = transmissionLine.FromNode,
                MeasuredToNode = transmissionLine.ToNode
            };

            transmissionLine.FromSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
            transmissionLine.FromSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;

            transmissionLine.ToSubstationCurrent = new CurrentFlowPhasorGroup()
            {
                InternalID = nextTwoAvailableCurrentFlowInternalIds[1],
                Number = nextTwoAvailableCurrentFlowInternalIds[1],
                Acronym = "I " + nextAvailableInternalId.ToString(),
                Name = "To Phasor Name",
                Description = "To Current Flow Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredBranch = transmissionLine,
                MeasuredFromNode = transmissionLine.ToNode,
                MeasuredToNode = transmissionLine.FromNode
            };

            transmissionLine.ToSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
            transmissionLine.ToSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;

            parentDivision.TransmissionLines.Add(transmissionLine);
        }

        private static int GetNextAvailableTransmissionLineInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetTransmissionLineInternalIds(parentDivision));
        }

        private static List<int> GetTransmissionLineInternalIds(Division parentDivision)
        {
            List<int> transmissionLineInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        transmissionLineInternalIds.Add(transmissionLine.InternalID);
                    }
                }
            }

            return transmissionLineInternalIds;
        }

        #endregion

        #region [ Nodes ]

        internal static void CreateNewNode(Substation parentSubstation)
        {
            int nextAvailableInternalId = GetNextAvailableNodeInternalId(parentSubstation.ParentDivision);

            while (parentSubstation.ParentDivision.ParentCompany.ParentModel.VoltageLevels.Count < 1)
            {
                CreateNewVoltageLevel(parentSubstation.ParentDivision.ParentCompany.ParentModel.VoltageLevels);
            }

            Node node = new Node()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "ND " + nextAvailableInternalId.ToString(),
                Name = parentSubstation.Name + " ND " + nextAvailableInternalId.ToString(),
                Description = parentSubstation.Name + " ND " + nextAvailableInternalId.ToString() + " Description",
                ParentSubstation = parentSubstation,
                BaseKV = parentSubstation.ParentDivision.ParentCompany.ParentModel.VoltageLevels[0],
            };

            VoltagePhasorGroup voltage = new VoltagePhasorGroup()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "V " + nextAvailableInternalId.ToString(),
                Name = "Phasor Name",
                Description = "Voltage Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredNode = node
            };

            voltage.ZeroSequence.Measurement.BaseKV = node.BaseKV;
            voltage.ZeroSequence.Estimate.BaseKV = node.BaseKV;
            voltage.NegativeSequence.Measurement.BaseKV = node.BaseKV;
            voltage.NegativeSequence.Estimate.BaseKV = node.BaseKV;
            voltage.PositiveSequence.Measurement.BaseKV = node.BaseKV;
            voltage.PositiveSequence.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseA.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseA.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseB.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseB.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseC.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseC.Estimate.BaseKV = node.BaseKV;

            node.Voltage = voltage;

            parentSubstation.Nodes.Add(node);
        }

        internal static void CreateNewNode(TransmissionLine parentTransmissionLine)
        {
            int nextAvailableInternalId = GetNextAvailableNodeInternalId(parentTransmissionLine.ParentDivision);

            while (parentTransmissionLine.ParentDivision.ParentCompany.ParentModel.VoltageLevels.Count < 1)
            {
                CreateNewVoltageLevel(parentTransmissionLine.ParentDivision.ParentCompany.ParentModel.VoltageLevels);
            }

            Node node = new Node()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "ND " + nextAvailableInternalId.ToString(),
                Name = parentTransmissionLine.Name + " ND " + nextAvailableInternalId.ToString(),
                Description = parentTransmissionLine.Name + " ND " + nextAvailableInternalId.ToString() + " Description",
                ParentTransmissionLine = parentTransmissionLine,
                BaseKV = parentTransmissionLine.ParentDivision.ParentCompany.ParentModel.VoltageLevels[0],
            };

            VoltagePhasorGroup voltage = new VoltagePhasorGroup()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "V " + nextAvailableInternalId.ToString(),
                Name = "Phasor Name",
                Description = "Voltage Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredNode = node
            };

            voltage.ZeroSequence.Measurement.BaseKV = node.BaseKV;
            voltage.ZeroSequence.Estimate.BaseKV = node.BaseKV;
            voltage.NegativeSequence.Measurement.BaseKV = node.BaseKV;
            voltage.NegativeSequence.Estimate.BaseKV = node.BaseKV;
            voltage.PositiveSequence.Measurement.BaseKV = node.BaseKV;
            voltage.PositiveSequence.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseA.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseA.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseB.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseB.Estimate.BaseKV = node.BaseKV;
            voltage.PhaseC.Measurement.BaseKV = node.BaseKV;
            voltage.PhaseC.Estimate.BaseKV = node.BaseKV;

            node.Voltage = voltage;

            parentTransmissionLine.Nodes.Add(node);
        }

        private static int GetNextAvailableNodeInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetNodeInternalIds(parentDivision));
        }

        private static List<int> GetNodeInternalIds(Division parentDivision)
        {
            List<int> nodeInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Node node in substation.Nodes)
                        {
                            nodeInternalIds.Add(node.InternalID);
                        }
                    }

                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (Node node in transmissionLine.Nodes)
                        {
                            nodeInternalIds.Add(node.InternalID);
                        }
                    }
                }
            }

            return nodeInternalIds;
        }
        
        #endregion

        #region [ Transformer ]

        internal static void CreateNewTransformer(Substation parentSubstation)
        {
            int nextAvailableInternalId = GetNextAvailableTransformerInternalId(parentSubstation.ParentDivision);
            int[] nextTwoAvailableCurrentFlowInternalIds = GetNextTwoAvailableCurrentFlowPhasorGroupInternalIds(parentSubstation.ParentDivision);

            Transformer transformer = new Transformer()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "TX " + nextAvailableInternalId.ToString(),
                Name = parentSubstation.Name + " TX " + nextAvailableInternalId.ToString(),
                Description = parentSubstation.Name + " TX " + nextAvailableInternalId.ToString() + " Description",
                ParentSubstation = parentSubstation,
                FromNodeConnectionType = TransformerConnectionType.Wye,
                ToNodeConnectionType = TransformerConnectionType.Wye,
                FixedTapPosition = 0,
                UltcIsEnabled = false
            };

            if (parentSubstation.Nodes.Count < 2)
            {
                CreateNewNode(parentSubstation);
                CreateNewNode(parentSubstation);
            }

            if (parentSubstation.ParentDivision.ParentCompany.ParentModel.TapConfigurations.Count < 1)
            {
                CreateNewTapConfiguration(parentSubstation.ParentDivision.ParentCompany.ParentModel.TapConfigurations);
            }

            transformer.FromNode = parentSubstation.Nodes[parentSubstation.Nodes.Count - 1];
            transformer.ToNode = parentSubstation.Nodes[parentSubstation.Nodes.Count - 2];
            transformer.Tap = parentSubstation.ParentDivision.ParentCompany.ParentModel.TapConfigurations[0];

            transformer.FromNodeCurrent = new CurrentFlowPhasorGroup()
            {
                InternalID = nextTwoAvailableCurrentFlowInternalIds[0],
                Number = nextTwoAvailableCurrentFlowInternalIds[0],
                Acronym = "I " + nextAvailableInternalId.ToString(),
                Name = "From Phasor Name",
                Description = "From Current Flow Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredBranch = transformer,
                MeasuredFromNode = transformer.FromNode
            };

            transformer.FromNodeCurrent.ZeroSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.ZeroSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.NegativeSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.NegativeSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PositiveSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PositiveSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseA.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseA.Estimate.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseB.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseB.Estimate.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseC.Measurement.BaseKV = transformer.FromNode.BaseKV;
            transformer.FromNodeCurrent.PhaseC.Estimate.BaseKV = transformer.FromNode.BaseKV;

            transformer.ToNodeCurrent = new CurrentFlowPhasorGroup()
            {
                InternalID = nextTwoAvailableCurrentFlowInternalIds[1],
                Number = nextTwoAvailableCurrentFlowInternalIds[1],
                Acronym = "I " + nextAvailableInternalId.ToString(),
                Name = "To Phasor Name",
                Description = "To Current Flow Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredBranch = transformer,
                MeasuredToNode = transformer.ToNode
            };

            transformer.ToNodeCurrent.ZeroSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.ZeroSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.NegativeSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.NegativeSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PositiveSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PositiveSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseA.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseA.Estimate.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseB.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseB.Estimate.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseC.Measurement.BaseKV = transformer.ToNode.BaseKV;
            transformer.ToNodeCurrent.PhaseC.Estimate.BaseKV = transformer.ToNode.BaseKV;

            parentSubstation.Transformers.Add(transformer);
        }

        private static int GetNextAvailableTransformerInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetTransformerInternalIds(parentDivision));
        }

        private static List<int> GetTransformerInternalIds(Division parentDivision)
        {
            List<int> transformerInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Transformer transformer in substation.Transformers)
                        {
                            transformerInternalIds.Add(transformer.InternalID);
                        }
                    }
                }
            }

            return transformerInternalIds;
        }

        #endregion

        #region [ Shunts ]

        internal static void CreateNewShunt(Substation parentSubstation)
        {
            int nextAvailableInternalId = GetNextAvailableShuntInternalId(parentSubstation.ParentDivision);

            ShuntCompensator shunt = new ShuntCompensator()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Name = parentSubstation.Name + " SH " + nextAvailableInternalId.ToString(),
                Description = parentSubstation.Name + " SH " + nextAvailableInternalId.ToString() + " Description",
                ParentSubstation = parentSubstation,
                ImpedanceCalculationMethod = ShuntImpedanceCalculationMethod.CalculateFromRating,
                NominalMvar = 100
            };

            shunt.Current = new CurrentInjectionPhasorGroup()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "I " + nextAvailableInternalId.ToString(),
                Name = "From Phasor Name",
                Description = "From Current Flow Phasor Group Description",
                IsEnabled = true,
                UseStatusFlagForRemovingMeasurements = true,
                MeasuredBranch = shunt,
                MeasurementDirectionConvention = CurrentInjectionDirectionConvention.IntoTheShunt
            };

            if (parentSubstation.Nodes.Count == 0)
            {
                CreateNewNode(parentSubstation);
            }

            shunt.ConnectedNode = parentSubstation.Nodes[0];

            shunt.Current.ZeroSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.ZeroSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.NegativeSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.NegativeSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PositiveSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PositiveSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseA.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseA.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseB.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseB.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseC.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
            shunt.Current.PhaseC.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;

            parentSubstation.Shunts.Add(shunt);
        }

        private static int GetNextAvailableShuntInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetShuntInternalIds(parentDivision));
        }

        private static List<int> GetShuntInternalIds(Division parentDivision)
        {
            List<int> shuntInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (ShuntCompensator shunt in substation.Shunts)
                        {
                            shuntInternalIds.Add(shunt.InternalID);
                        }
                    }
                }
            }

            return shuntInternalIds;
        }
        
        #endregion

        #region [ Switches ]

        internal static void CreateNewSwitch(Substation parentSubstation)
        {
            int nextAvailableInternalId = GetNextAvailableSwitchInternalId(parentSubstation.ParentDivision);

            while (parentSubstation.Nodes.Count < 2)
            {
                CreateNewNode(parentSubstation);
            }
            
            parentSubstation.Switches.Add(new Switch()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "SW " + nextAvailableInternalId.ToString(),
                Name = parentSubstation.Name + " SW " + nextAvailableInternalId.ToString(),
                Description = parentSubstation.Name + " SW " + nextAvailableInternalId.ToString() + " Description",
                ParentSubstation = parentSubstation,
                NormalState = SwitchingDeviceNormalState.Closed,
                MeasurementKey = "Undefined",
                FromNode = parentSubstation.Nodes[0],
                ToNode = parentSubstation.Nodes[1]
            });
        }

        internal static void CreateNewSwitch(TransmissionLine parentTransmissionLine)
        {
            int nextAvailableInternalId = GetNextAvailableSwitchInternalId(parentTransmissionLine.ParentDivision);

            while (parentTransmissionLine.Nodes.Count < 2)
            {
                CreateNewNode(parentTransmissionLine);
            }
            
            parentTransmissionLine.Switches.Add(new Switch()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "SW " + nextAvailableInternalId.ToString(),
                Name = parentTransmissionLine.Name + " SW " + nextAvailableInternalId.ToString(),
                Description = parentTransmissionLine.Name + " SW " + nextAvailableInternalId.ToString() + " Description",
                ParentTransmissionLine = parentTransmissionLine,
                NormalState = SwitchingDeviceNormalState.Closed,
                MeasurementKey = "Undefined",
                FromNode = parentTransmissionLine.Nodes[0],
                ToNode = parentTransmissionLine.Nodes[1]
            });
        }

        private static int GetNextAvailableSwitchInternalId(Division parentDivision)
        {
            return GetNextAvailableInternalId(GetSwitchInternalIds(parentDivision));
        }

        private static List<int> GetSwitchInternalIds(Division parentDivision)
        {
            List<int> switchInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Switch circuitSwitch in substation.Switches)
                        {
                            switchInternalIds.Add(circuitSwitch.InternalID);
                        }
                    }

                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (Switch circuitSwitch in transmissionLine.Switches)
                        {
                            switchInternalIds.Add(circuitSwitch.InternalID);
                        }
                    }
                }
            }

            return switchInternalIds;
        }
        
        #endregion

        #region [ Circuit Breakers ]

        internal static void CreateNewCircuitBreaker(Substation parentSubstation)
        {
            int nextAvailableInternalId = GetNextAvailableCircuitBreakerInternalId(parentSubstation);

            while (parentSubstation.Nodes.Count < 2)
            {
                CreateNewNode(parentSubstation);
            }
            
            CircuitBreaker circuitBreaker = new CircuitBreaker()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "CB " + nextAvailableInternalId.ToString(),
                Name = parentSubstation.Name + " CB " + nextAvailableInternalId.ToString(),
                Description = parentSubstation.Name + " CB " + nextAvailableInternalId.ToString() + " Description",
                ParentSubstation = parentSubstation,
                NormalState = SwitchingDeviceNormalState.Closed,
                MeasurementKey = "Undefined",
                FromNode = parentSubstation.Nodes[0],
                ToNode = parentSubstation.Nodes[1]
            };

            parentSubstation.CircuitBreakers.Add(circuitBreaker);
        }

        private static int GetNextAvailableCircuitBreakerInternalId(Substation parentSubstation)
        {
            return GetNextAvailableInternalId(GetCircuitBreakerInternalIds(parentSubstation));
        }

        private static List<int> GetCircuitBreakerInternalIds(Substation parentSubstation)
        {
            List<int> circuitBreakerInternalIds = new List<int>();

            foreach (Company company in parentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
                        {
                            circuitBreakerInternalIds.Add(circuitBreaker.InternalID);
                        }
                    }
                }
            }

            return circuitBreakerInternalIds;
        }

        #endregion

        #region [ LineSegments ]

        internal static void CreateNewLineSegment(TransmissionLine parentTransmissionLine)
        {
            int nextAvailableInternalId = GetNextAvailableLineSegmentInternalId(parentTransmissionLine);

            LineSegment lineSegment = new LineSegment()
            {
                InternalID = nextAvailableInternalId,
                Number = nextAvailableInternalId,
                Acronym = "LN " + nextAvailableInternalId.ToString(),
                Name = parentTransmissionLine.Name + " Line Segment " + nextAvailableInternalId.ToString(),
                Description = parentTransmissionLine.Name + " Line Segment " + nextAvailableInternalId.ToString() + " Description",
                ParentTransmissionLine = parentTransmissionLine
            };

            if (parentTransmissionLine.FromNode != null)
            {
                lineSegment.FromNode = parentTransmissionLine.FromNode;
            }
            if (parentTransmissionLine.ToNode != null)
            {
                lineSegment.ToNode = parentTransmissionLine.ToNode;
            }

            parentTransmissionLine.LineSegments.Add(lineSegment);
        }

        private static int GetNextAvailableLineSegmentInternalId(TransmissionLine parentTransmissionLine)
        {
            return GetNextAvailableInternalId(GetLineSegmentsInternalIds(parentTransmissionLine));
        }

        private static List<int> GetLineSegmentsInternalIds(TransmissionLine parentTransmissionLine)
        {
            List<int> lineSegmentInternalIds = new List<int>();

            foreach (Company company in parentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (LineSegment lineSegment in transmissionLine.LineSegments)
                        {
                            lineSegmentInternalIds.Add(lineSegment.InternalID);
                        }
                    }
                }
            }

            return lineSegmentInternalIds;
        }

        #endregion

        private static int GetNextAvailableInternalId(List<int> existingInternalIds)
        {
            for (int i = 1; i < MAXIMUM_INTERNAL_ID; i++)
            {
                if (!existingInternalIds.Contains(i))
                {
                    return i;
                }
            }
            return MAXIMUM_INTERNAL_ID;
        }

        private static int[] GetNextTwoAvailableCurrentFlowPhasorGroupInternalIds(Division parentDivision)
        {
            List<int> currentFlowPhasorGroupInternalIds = GetCurrentFlowPhasorGroupInternalIds(parentDivision);

            int[] nextTwoAvailableInternalIds = new int[2];
            nextTwoAvailableInternalIds[0] = GetNextAvailableInternalId(currentFlowPhasorGroupInternalIds);
            currentFlowPhasorGroupInternalIds.Add(nextTwoAvailableInternalIds[0]);
            nextTwoAvailableInternalIds[1] = GetNextAvailableInternalId(currentFlowPhasorGroupInternalIds);
            return nextTwoAvailableInternalIds;
        }

        private static List<int> GetCurrentFlowPhasorGroupInternalIds(Division parentDivision)
        {
            List<int> currentFlowInternalIds = new List<int>();

            foreach (Company company in parentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Transformer transformer in substation.Transformers)
                        {
                            currentFlowInternalIds.Add(transformer.FromNodeCurrent.InternalID);
                            currentFlowInternalIds.Add(transformer.ToNodeCurrent.InternalID);
                        }
                    }

                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        currentFlowInternalIds.Add(transmissionLine.FromSubstationCurrent.InternalID);
                        currentFlowInternalIds.Add(transmissionLine.ToSubstationCurrent.InternalID);
                    }
                }
            }
            return currentFlowInternalIds;
        }

    }
}
