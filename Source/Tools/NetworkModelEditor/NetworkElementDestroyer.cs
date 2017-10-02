//******************************************************************************************************
//  NetworkElementDestroyer.cs
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
using SynchrophasorAnalytics.Testing;
//using NetworkModelEditor.Models;

namespace NetworkModelEditor
{
    public class NetworkElementDestroyer
    {
        public static void DestroyNetworkElement(NetworkElementViewModel networkElementViewModel)
        {
            object networkElement = networkElementViewModel.Value.Element;

            if (networkElement is Company)
            {
                DeleteCompany(networkElement as Company);
            }
            else if (networkElement is Division)
            {
                DeleteDivision(networkElement as Division);
            }
            else if (networkElement is Substation)
            {
                DeleteSubstation(networkElement as Substation);
            }
            else if (networkElement is TransmissionLine)
            {
                DeleteTransmissionLine(networkElement as TransmissionLine);
            }
            else if (networkElement is Node)
            {
                DeleteNode(networkElement as Node);
            }
            else if (networkElement is ShuntCompensator)
            {
                DeleteShunt(networkElement as ShuntCompensator);
            }
            else if (networkElement is Switch)
            {
                DeleteSwitch(networkElement as Switch);
            }
            else if (networkElement is CircuitBreaker)
            {
                DeleteCircuitBreaker(networkElement as CircuitBreaker);
            }
            else if (networkElement is Transformer)
            {
                DeleteTransformer(networkElement as Transformer);
            }
            else if (networkElement is SeriesCompensator)
            {
                DeleteSeriesCompensator(networkElement as SeriesCompensator);
            }
            else if (networkElement is LineSegment)
            {
                DeleteLineSegment(networkElement as LineSegment);
            }
            else if (networkElement is VoltageLevel)
            {
                DeleteVoltageLevel(networkElement as VoltageLevel, networkElementViewModel.Parent.Value.Element as List<VoltageLevel>);
            }
            else if (networkElement is StatusWord)
            {
                DeleteStatusWord(networkElement as StatusWord, networkElementViewModel.Parent.Value.Element as List<StatusWord>);
            }
            else if (networkElement is BreakerStatus)
            {
                DeleteBreakerStatus(networkElement as BreakerStatus, networkElementViewModel.Parent.Value.Element as List<BreakerStatus>);
            }
            else if (networkElement is TapConfiguration)
            {
                DeleteTapConfiguration(networkElement as TapConfiguration, networkElementViewModel.Parent.Value.Element as List<TapConfiguration>);
            }
            else if (networkElement is RawMeasurements)
            {
                MainWindowViewModel mainWindow = networkElementViewModel.NetworkTree.MainWindow as MainWindowViewModel;
                mainWindow.DeleteMeasurementSample(networkElement as RawMeasurements);
            }
        }

        private static void DeleteSeriesCompensator(SeriesCompensator seriesCompensator)
        {
            seriesCompensator.ParentTransmissionLine.SeriesCompensators.Remove(seriesCompensator);
        }

        private static void DeleteShunt(ShuntCompensator shuntCompensator)
        {
            shuntCompensator.ParentSubstation.Shunts.Remove(shuntCompensator);
        }
        
        private static void DeleteCompany(Company company)
        {
            company.ParentModel.Companies.Remove(company);
        }

        private static void DeleteDivision(Division division)
        {
            division.ParentCompany.Divisions.Remove(division);
        }

        private static void DeleteSubstation(Substation substation)
        {
            substation.ParentDivision.Substations.Remove(substation);
        }

        private static void DeleteTransmissionLine(TransmissionLine transmissionLine)
        {
            transmissionLine.ParentDivision.TransmissionLines.Remove(transmissionLine);
        }

        private static void DeleteNode(Node node)
        {
            if (node.ParentSubstation != null)
            {
                node.ParentSubstation.Nodes.Remove(node);
            }
            if (node.ParentTransmissionLine != null)
            {
                node.ParentTransmissionLine.Nodes.Remove(node);
            }
        }

        private static void DeleteCircuitBreaker(CircuitBreaker circuitBreaker)
        {
            circuitBreaker.ParentSubstation.CircuitBreakers.Remove(circuitBreaker);
        }

        private static void DeleteSwitch(Switch circuitSwitch)
        {
            if (circuitSwitch.ParentSubstation != null)
            {
                circuitSwitch.ParentSubstation.Switches.Remove(circuitSwitch);
            }
            if (circuitSwitch.ParentTransmissionLine != null)
            {
                circuitSwitch.ParentTransmissionLine.Switches.Remove(circuitSwitch);
            }
        }

        private static void DeleteTransformer(Transformer transformer)
        {
            transformer.ParentSubstation.Transformers.Remove(transformer);
        }

        private static void DeleteLineSegment(LineSegment lineSegment)
        {
            lineSegment.ParentTransmissionLine.LineSegments.Remove(lineSegment);
        }

        private static void DeleteVoltageLevel(VoltageLevel voltageLevel, List<VoltageLevel> parent)
        {
            parent.Remove(voltageLevel);
        }

        private static void DeleteStatusWord(StatusWord statusWord, List<StatusWord> parent)
        {
            parent.Remove(statusWord);
        }

        private static void DeleteBreakerStatus(BreakerStatus breakerStatus, List<BreakerStatus> parent)
        {
            parent.Remove(breakerStatus);
        }

        private static void DeleteTapConfiguration(TapConfiguration tapConfiguration, List<TapConfiguration> parent)
        {
            parent.Remove(tapConfiguration);
        }

    }
}
