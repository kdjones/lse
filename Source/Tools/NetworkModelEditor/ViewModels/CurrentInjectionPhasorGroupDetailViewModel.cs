//******************************************************************************************************
//  CurrentInjectionPhasorGroupDetailViewModel.cs
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
//  05/12/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Calibration;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Networks;
using System.Windows;

namespace NetworkModelEditor.ViewModels
{
    public class CurrentInjectionPhasorGroupDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private CurrentInjectionPhasorGroup m_current;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_current.InternalID;
            }
            set
            {
                m_current.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_current.Number;
            }
            set
            {
                m_current.Number = value;
            }
        
        }

        public string Name
        {
            get
            {
                return m_current.Name;
            }
            set
            {
                m_current.Name = value;
            }
        
        }

        public string Acronym
        {
            get
            {
                return m_current.Acronym;
            }
            set
            {
                m_current.Acronym = value;
            }
        
        }

        public string Description
        {
            get
            {
                return m_current.Description;
            }
            set
            {
                m_current.Description = value;
            }
        
        }

        public StatusWord StatusWord
        {
            get
            {
                return m_current.Status;
            }
            set
            {
                m_current.Status = value;
            }
        
        }

        public CalibrationSetting CalibrationType
        {
            get
            {
                return m_current.PositiveSequence.Measurement.InstrumentTransformerCalibrationSetting;
            }
            set
            {
                m_current.PositiveSequence.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_current.PhaseA.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_current.PhaseB.Measurement.InstrumentTransformerCalibrationSetting = value;
                m_current.PhaseC.Measurement.InstrumentTransformerCalibrationSetting = value;
            }
        
        }

        public bool IsEnabled
        {
            get
            {
                return m_current.IsEnabled;
            }
            set
            {
                m_current.IsEnabled = value;
            }
        
        }

        public bool UseStatusFlagsForRemovingMeasurements
        {
            get
            {
                return m_current.UseStatusFlagForRemovingMeasurements;
            }
            set
            {
                m_current.UseStatusFlagForRemovingMeasurements = value;
            }
        
        }

        public ObservableCollection<StatusWord> StatusWordSelection
        {
            get
            {
                if (m_current.MeasuredConnectedNode == null)
                {
                    return new ObservableCollection<StatusWord>(new List<StatusWord>());
                }
                return new ObservableCollection<StatusWord>(m_current.MeasuredConnectedNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.StatusWords);
            }
        }

        public bool IsPositiveSequence
        {
            get
            {
                if (m_current.MeasuredBranch is TransmissionLine)
                {
                    return (m_current.MeasuredBranch as TransmissionLine).ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.PositiveSequence);
                }
                else if (m_current.MeasuredBranch is Transformer)
                {
                    return (m_current.MeasuredBranch as Transformer).ParentSubstation.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.PositiveSequence);
                }
                return false;
            }
        }

        public bool IsThreePhase
        {
            get
            {
                return (m_current.MeasuredBranch as ShuntCompensator).ParentSubstation.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration.Equals(PhaseSelection.ThreePhase);
            }
        }

        public CurrentInjectionDirectionConvention MeasurementDirectionConvention
        {
            get
            {
                return m_current.MeasurementDirectionConvention;
            }
            set
            {
                m_current.MeasurementDirectionConvention = value;
            }
        }

        public string MeasurementIsIncludedKey
        {
            get
            {
                return m_current.MeasurementIsIncludedKey;
            }
            set
            {
                m_current.MeasurementIsIncludedKey = value;
            }
        }

        #region [ Positive Sequence ]

        public string PositiveSequenceMagnitudeMeasurementKey
        {
            get
            {
                return m_current.PositiveSequence.Measurement.MagnitudeKey;
            }
            set
            {
                m_current.PositiveSequence.Measurement.MagnitudeKey = value;
            }
        }

        public string PositiveSequenceMagnitudeEstimateKey
        {
            get
            {
                return m_current.PositiveSequence.Estimate.MagnitudeKey;
            }
            set
            {
                m_current.PositiveSequence.Estimate.MagnitudeKey = value;
            }
        }

        public string PositiveSequenceAngleMeasurementKey
        {
            get
            {
                return m_current.PositiveSequence.Measurement.AngleKey;
            }
            set
            {
                m_current.PositiveSequence.Measurement.AngleKey = value;
            }
        }

        public string PositiveSequenceAngleEstimateKey
        {
            get
            {
                return m_current.PositiveSequence.Estimate.AngleKey;
            }
            set
            {
                m_current.PositiveSequence.Estimate.AngleKey = value;
            }
        }

        public double PositiveSequenceRcf
        {
            get
            {
                return m_current.PositiveSequence.Measurement.RCF;
            }
            set
            {
                m_current.PositiveSequence.Measurement.RCF = value;
            }
        }

        public double PositiveSequencePacf
        {
            get
            {
                return m_current.PositiveSequence.Measurement.PACF;
            }
            set
            {
                m_current.PositiveSequence.Measurement.PACF = value;
            }
        }

        public double PositiveSequenceVariance
        {
            get
            {
                return m_current.PositiveSequence.Measurement.MeasurementVariance;
            }
            set
            {
                m_current.PositiveSequence.Measurement.MeasurementVariance = value;
            }
        }

        public bool PositiveSequenceMeasurementShouldBeCalibrated
        {
            get
            {
                return m_current.PositiveSequence.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_current.PositiveSequence.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ A Phase ]

        public string PhaseAMagnitudeMeasurementKey
        {
            get
            {
                return m_current.PhaseA.Measurement.MagnitudeKey;
            }
            set
            {
                m_current.PhaseA.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseAMagnitudeEstimateKey
        {
            get
            {
                return m_current.PhaseA.Estimate.MagnitudeKey;
            }
            set
            {
                m_current.PhaseA.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseAAngleMeasurementKey
        {
            get
            {
                return m_current.PhaseA.Measurement.AngleKey;
            }
            set
            {
                m_current.PhaseA.Measurement.AngleKey = value;
            }
        }

        public string PhaseAAngleEstimateKey
        {
            get
            {
                return m_current.PhaseA.Estimate.AngleKey;
            }
            set
            {
                m_current.PhaseA.Estimate.AngleKey = value;
            }
        }

        public double PhaseARcf
        {
            get
            {
                return m_current.PhaseA.Measurement.RCF;
            }
            set
            {
                m_current.PhaseA.Measurement.RCF = value;
            }
        }

        public double PhaseAPacf
        {
            get
            {
                return m_current.PhaseA.Measurement.PACF;
            }
            set
            {
                m_current.PhaseA.Measurement.PACF = value;
            }
        }

        public double PhaseAVariance
        {
            get
            {
                return m_current.PhaseA.Measurement.MeasurementVariance;
            }
            set
            {
                m_current.PhaseA.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseAMeasurementShouldBeCalibrated
        {
            get
            {
                return m_current.PhaseA.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_current.PhaseA.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ B Phase ]

        public string PhaseBMagnitudeMeasurementKey
        {
            get
            {
                return m_current.PhaseB.Measurement.MagnitudeKey;
            }
            set
            {
                m_current.PhaseB.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseBMagnitudeEstimateKey
        {
            get
            {
                return m_current.PhaseB.Estimate.MagnitudeKey;
            }
            set
            {
                m_current.PhaseB.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseBAngleMeasurementKey
        {
            get
            {
                return m_current.PhaseB.Measurement.AngleKey;
            }
            set
            {
                m_current.PhaseB.Measurement.AngleKey = value;
            }
        }

        public string PhaseBAngleEstimateKey
        {
            get
            {
                return m_current.PhaseB.Estimate.AngleKey;
            }
            set
            {
                m_current.PhaseB.Estimate.AngleKey = value;
            }
        }

        public double PhaseBRcf
        {
            get
            {
                return m_current.PhaseB.Measurement.RCF;
            }
            set
            {
                m_current.PhaseB.Measurement.RCF = value;
            }
        }

        public double PhaseBPacf
        {
            get
            {
                return m_current.PhaseB.Measurement.PACF;
            }
            set
            {
                m_current.PhaseB.Measurement.PACF = value;
            }
        }

        public double PhaseBVariance
        {
            get
            {
                return m_current.PhaseB.Measurement.MeasurementVariance;
            }
            set
            {
                m_current.PhaseB.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseBMeasurementShouldBeCalibrated
        {
            get
            {
                return m_current.PhaseB.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_current.PhaseB.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion

        #region [ C Phase ]

        public string PhaseCMagnitudeMeasurementKey
        {
            get
            {
                return m_current.PhaseC.Measurement.MagnitudeKey;
            }
            set
            {
                m_current.PhaseC.Measurement.MagnitudeKey = value;
            }
        }

        public string PhaseCMagnitudeEstimateKey
        {
            get
            {
                return m_current.PhaseC.Estimate.MagnitudeKey;
            }
            set
            {
                m_current.PhaseC.Estimate.MagnitudeKey = value;
            }
        }

        public string PhaseCAngleMeasurementKey
        {
            get
            {
                return m_current.PhaseC.Measurement.AngleKey;
            }
            set
            {
                m_current.PhaseC.Measurement.AngleKey = value;
            }
        }

        public string PhaseCAngleEstimateKey
        {
            get
            {
                return m_current.PhaseC.Estimate.AngleKey;
            }
            set
            {
                m_current.PhaseC.Estimate.AngleKey = value;
            }
        }

        public double PhaseCRcf
        {
            get
            {
                return m_current.PhaseC.Measurement.RCF;
            }
            set
            {
                m_current.PhaseC.Measurement.RCF = value;
            }
        }

        public double PhaseCPacf
        {
            get
            {
                return m_current.PhaseC.Measurement.PACF;
            }
            set
            {
                m_current.PhaseC.Measurement.PACF = value;
            }
        }

        public double PhaseCVariance
        {
            get
            {
                return m_current.PhaseC.Measurement.MeasurementVariance;
            }
            set
            {
                m_current.PhaseC.Measurement.MeasurementVariance = value;
            }
        }

        public bool PhaseCMeasurementShouldBeCalibrated
        {
            get
            {
                return m_current.PhaseC.Measurement.MeasurementShouldBeCalibrated;
            }
            set
            {
                m_current.PhaseC.Measurement.MeasurementShouldBeCalibrated = value;
            }
        }

        #endregion
        
        #endregion

        #region [ Constructors ]

        public CurrentInjectionPhasorGroupDetailViewModel()
        {
        }

        public CurrentInjectionPhasorGroupDetailViewModel(object current)
        {
            if (current != null && current is CurrentInjectionPhasorGroup)
            {
                m_current = current as CurrentInjectionPhasorGroup;
            }
        }

        #endregion

    }
}
