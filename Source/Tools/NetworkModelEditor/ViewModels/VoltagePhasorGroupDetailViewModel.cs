//******************************************************************************************************
//  VoltagePhasorGroupDetailViewModel.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Calibration;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;

namespace NetworkModelEditor.ViewModels
{
    public class VoltagePhasorGroupDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private VoltagePhasorGroup m_voltage;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_voltage.InternalID;
            }
            set
            {
                m_voltage.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_voltage.Number;
            }
            set
            {
                m_voltage.Number = value;
            }
        
        }

        public string Name
        {
            get
            {
                return m_voltage.Name;
            }
            set
            {
                m_voltage.Name = value;
            }
        
        }

        public string Acronym
        {
            get
            {
                return m_voltage.Acronym;
            }
            set
            {
                m_voltage.Acronym = value;
            }
        
        }

        public string Description
        {
            get
            {
                return m_voltage.Description;
            }
            set
            {
                m_voltage.Description = value;
            }
        
        }

        public StatusWord StatusFlag
        {
            get
            {
                return m_voltage.Status;
            }
            set
            {
                m_voltage.Status = value;
            }
        }

        public ObservableCollection<StatusWord> StatusWordSelection
        {
            get
            {
                if (m_voltage.MeasuredNode.ParentSubstation != null)
                {
                    return new ObservableCollection<StatusWord>(m_voltage.MeasuredNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.StatusWords);
                }
                else if (m_voltage.MeasuredNode.ParentTransmissionLine != null)
                {
                    return new ObservableCollection<StatusWord>(m_voltage.MeasuredNode.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.StatusWords);
                }
                else
                {
                    return new ObservableCollection<StatusWord>();
                }
            }
        }

        public CalibrationSetting CalibrationType
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.InstrumentTransformerCalibrationSetting;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting = value;
            }
        
        }

        public bool IsEnabled
        {
            get
            {
                return m_voltage.IsEnabled;
            }
            set
            {
                m_voltage.IsEnabled = value;
            }
        
        }

        public bool UseStatusFlagsForRemovingMeasurements
        {
            get
            {
                return m_voltage.UseStatusFlagForRemovingMeasurements;
            }
            set
            {
                m_voltage.UseStatusFlagForRemovingMeasurements = value;
            }
        
        }

        public bool IsPositiveSequence
        {
            get
            {
                if (m_voltage.MeasuredNode.ParentSubstation != null)
                {
                    return m_voltage.MeasuredNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.PositiveSequence);
                }
                else if (m_voltage.MeasuredNode.ParentTransmissionLine != null)
                {
                    return m_voltage.MeasuredNode.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.PositiveSequence);
                }
                else
                {
                    return true;
                }
             }
        }

        public bool IsThreePhase
        {
            get
            {
                if (m_voltage.MeasuredNode.ParentSubstation != null)
                {
                    return m_voltage.MeasuredNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.ThreePhase);
                }
                else if (m_voltage.MeasuredNode.ParentTransmissionLine != null)
                {
                    return m_voltage.MeasuredNode.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.ThreePhase);
                }
                else
                {
                    return false;
                }
            }
        }

        public string MeasurementIsIncludedKey
        {
            get
            {
                return m_voltage.MeasurementIsIncludedKey;
            }
            set
            {
                m_voltage.MeasurementIsIncludedKey = value;
            }
        }

        #region [ Positive Sequence ]

        public string PositiveSequenceMagnitudeMeasurementKey
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.MagnitudeKey;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.MagnitudeKey = value;
            }
        }

        public string PositiveSequenceMagnitudeEstimateKey
        {
            get
            {
                return m_voltage.PositiveSequence.Estimate.MagnitudeKey;
            }
            set
            {
                m_voltage.PositiveSequence.Estimate.MagnitudeKey = value;
            }
        }

        public string PositiveSequenceAngleMeasurementKey
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.AngleKey;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.AngleKey = value;
            }
        }

        public string PositiveSequenceAngleEstimateKey
        {
            get
            {
                return m_voltage.PositiveSequence.Estimate.AngleKey;
            }
            set
            {
                m_voltage.PositiveSequence.Estimate.AngleKey = value;
            }
        }

        public double PositiveSequenceRcf
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.RCF;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.RCF = value;
            }
        }

        public double PositiveSequencePacf
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.PACF;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.PACF = value;
            }
        }

        public double PositiveSequenceVariance
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.MeasurementVariance;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.MeasurementVariance = value;
            }
        }

        public bool PositiveSequenceMeasurementShouldBeCalibrated
        {
            get
            {
                return m_voltage.PositiveSequence.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_voltage.PositiveSequence.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ A Phase ]

        public string PhaseAMagnitudeMeasurementKey
        {
            get
            {
                return m_voltage.PhaseA.Measurement.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseA.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseAMagnitudeEstimateKey
        {
            get
            {
                return m_voltage.PhaseA.Estimate.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseA.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseAAngleMeasurementKey
        {
            get
            {
                return m_voltage.PhaseA.Measurement.AngleKey;
            }
            set
            {
                m_voltage.PhaseA.Measurement.AngleKey = value;
            }
        }

        public string PhaseAAngleEstimateKey
        {
            get
            {
                return m_voltage.PhaseA.Estimate.AngleKey;
            }
            set
            {
                m_voltage.PhaseA.Estimate.AngleKey = value;
            }
        }

        public double PhaseARcf
        {
            get
            {
                return m_voltage.PhaseA.Measurement.RCF;
            }
            set
            {
                m_voltage.PhaseA.Measurement.RCF = value;
            }
        }

        public double PhaseAPacf
        {
            get
            {
                return m_voltage.PhaseA.Measurement.PACF;
            }
            set
            {
                m_voltage.PhaseA.Measurement.PACF = value;
            }
        }

        public double PhaseAVariance
        {
            get
            {
                return m_voltage.PhaseA.Measurement.MeasurementVariance;
            }
            set
            {
                m_voltage.PhaseA.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseAMeasurementShouldBeCalibrated
        {
            get
            {
                return m_voltage.PhaseA.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_voltage.PhaseA.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ B Phase ]

        public string PhaseBMagnitudeMeasurementKey
        {
            get
            {
                return m_voltage.PhaseB.Measurement.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseB.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseBMagnitudeEstimateKey
        {
            get
            {
                return m_voltage.PhaseB.Estimate.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseB.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseBAngleMeasurementKey
        {
            get
            {
                return m_voltage.PhaseB.Measurement.AngleKey;
            }
            set
            {
                m_voltage.PhaseB.Measurement.AngleKey = value;
            }
        }

        public string PhaseBAngleEstimateKey
        {
            get
            {
                return m_voltage.PhaseB.Estimate.AngleKey;
            }
            set
            {
                m_voltage.PhaseB.Estimate.AngleKey = value;
            }
        }

        public double PhaseBRcf
        {
            get
            {
                return m_voltage.PhaseB.Measurement.RCF;
            }
            set
            {
                m_voltage.PhaseB.Measurement.RCF = value;
            }
        }

        public double PhaseBPacf
        {
            get
            {
                return m_voltage.PhaseB.Measurement.PACF;
            }
            set
            {
                m_voltage.PhaseB.Measurement.PACF = value;
            }
        }

        public double PhaseBVariance
        {
            get
            {
                return m_voltage.PhaseB.Measurement.MeasurementVariance;
            }
            set
            {
                m_voltage.PhaseB.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseBMeasurementShouldBeCalibrated
        {
            get
            {
                return m_voltage.PhaseB.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_voltage.PhaseB.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ C Phase ]

        public string PhaseCMagnitudeMeasurementKey
        {
            get
            {
                return m_voltage.PhaseC.Measurement.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseC.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseCMagnitudeEstimateKey
        {
            get
            {
                return m_voltage.PhaseC.Estimate.MagnitudeKey;
            }
            set
            {
                m_voltage.PhaseC.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseCAngleMeasurementKey
        {
            get
            {
                return m_voltage.PhaseC.Measurement.AngleKey;
            }
            set
            {
                m_voltage.PhaseC.Measurement.AngleKey = value;
            }
        }

        public string PhaseCAngleEstimateKey
        {
            get
            {
                return m_voltage.PhaseC.Estimate.AngleKey;
            }
            set
            {
                m_voltage.PhaseC.Estimate.AngleKey = value;
            }
        }

        public double PhaseCRcf
        {
            get
            {
                return m_voltage.PhaseC.Measurement.RCF;
            }
            set
            {
                m_voltage.PhaseC.Measurement.RCF = value;
            }
        }

        public double PhaseCPacf
        {
            get
            {
                return m_voltage.PhaseC.Measurement.PACF;
            }
            set
            {
                m_voltage.PhaseC.Measurement.PACF = value;
            }
        }

        public double PhaseCVariance
        {
            get
            {
                return m_voltage.PhaseC.Measurement.MeasurementVariance;
            }
            set
            {
                m_voltage.PhaseC.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseCMeasurementShouldBeCalibrated
        {
            get
            {
                return m_voltage.PhaseC.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_voltage.PhaseC.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]

        public VoltagePhasorGroupDetailViewModel()
        {
        }

        public VoltagePhasorGroupDetailViewModel(object voltage)
        {
            if (voltage != null && voltage is VoltagePhasorGroup)
            {
                m_voltage = voltage as VoltagePhasorGroup;
            }
        }

        #endregion

    }
}
