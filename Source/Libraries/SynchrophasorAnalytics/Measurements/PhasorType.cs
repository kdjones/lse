//******************************************************************************************************
//  PhasorType.cs
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

namespace SynchrophasorAnalytics.Measurements
{
    #region [ Enumeration ] 

    /// <summary>
    /// Defines the data type for the <see cref="PhasorBase"/> which is intrinsically a complex number. To define translation from per unit to base values, the phasor must be defined as voltage, current, or complex power.
    /// </summary>
    /// <seealso cref="PhasorBase"/>
    public enum PhasorType
    {
        /// <summary>
        /// The enumeration for a voltage phasor.
        /// </summary>
        [XmlEnum("V")]
        VoltagePhasor,

        /// <summary>
        /// The enumeration for a current phasor.
        /// </summary>
        [XmlEnum("I")]
        CurrentPhasor,

        /// <summary>
        /// The enumeration for the phasor representation of complex power
        /// </summary>
        [XmlEnum("S")]
        ComplexPower,
    }

    #endregion
}
