//******************************************************************************************************
//  CalibrationSetting.cs
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
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Calibration
{
    #region [ Enumerations ]

    /// <summary>
    /// Defines how a <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> is included in the instrument transformer calibration algorithm.
    /// </summary>
    [Serializable()]
    public enum CalibrationSetting
    {
        /// <summary>
        /// Measurement group is included in primary calibration algorithm and calibrated itself.
        /// </summary>
        [XmlEnum("Active")]
        Active,

        /// <summary>
        /// Measurement group is not included in primary calibration algorithm but is calibrated during a second sweep.
        /// </summary>
        [XmlEnum("Passive")]
        Passive,

        /// <summary>
        /// Measurement group is not included in primary calibration and is never calibrated itself.
        /// </summary>
        [XmlEnum("Inactive")]
        Inactive,

        /// <summary>
        /// Measurement group is included in primary calibration but is not calibrated itself because it is assumed to be the reference point for the calibration.
        /// </summary>
        [XmlEnum("Perfect")]
        Perfect
    }

    #endregion
}
