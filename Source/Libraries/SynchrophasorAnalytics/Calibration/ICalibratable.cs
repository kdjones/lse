//******************************************************************************************************
//  ICalibratable.cs
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
//  06/14/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynchrophasorAnalytics.Calibration
{
    /// <summary>
    /// The interface that defines the properties and methods for phasor measurements that can eb calibrated during pre-processing.
    /// </summary>
    public interface ICalibratable
    {
        /// <summary>
        /// The <b>Ratio Correction Factor</b> for the phasor measurement. A scalar multiplier for the magnitude of the phasor measurement.
        /// </summary>
        double RCF
        {
            get;
            set;
        }

        /// <summary>
        /// The <b>Phase Angle Correction Factor</b> for the phasor measurement. A additive bias for the phase angle of the phasor measurement.
        /// </summary>
        double PACF
        {
            get;
            set;
        }

        /// <summary>
        /// A boolen flag which represents whether the <see cref="LinearStateEstimator.Calibration.ICalibratable.RCF"/> and <see cref="LinearStateEstimator.Calibration.ICalibratable.PACF"/> should be used in measurement pre-processing.
        /// </summary>
        bool MeasurementShouldBeCalibrated
        {
            get;
            set;
        }

        /// <summary>
        /// A setting which determines how the measurement is used for calibrating the instrument transformers.
        /// </summary>
        CalibrationSetting InstrumentTransformerCalibrationSetting
        {
            get;
            set;
        }

        /// <summary>
        /// A method which imposes the <see cref="LinearStateEstimator.Calibration.ICalibratable.RCF"/> and <see cref="LinearStateEstimator.Calibration.ICalibratable.PACF"/> on the raw phasor values.
        /// </summary>
        void CalibratePhasors();
    }
}
