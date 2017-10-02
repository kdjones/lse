//******************************************************************************************************
//  ShuntImpedanceCalculationMethod.cs
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
//  05/09/2014 - Kevin D. Jones
//       Generated original version of source code.
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Specifies how the impedance of <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> devices.
    /// </summary>
    [Serializable()]
    public enum ShuntImpedanceCalculationMethod
    {
        /// <summary>
        /// Specifies to the program to use the raw impedance parameters as the impedance.
        /// </summary>
        [XmlEnum("AsModeled")]
        UseModeledImpedance,

        /// <summary>
        /// Specifies to the program to overwrite the raw impedance parameters based on the nominal MVAR and KV rating of the shunt device.
        /// </summary>
        [XmlEnum("FromRating")]
        CalculateFromRating
    }
}
