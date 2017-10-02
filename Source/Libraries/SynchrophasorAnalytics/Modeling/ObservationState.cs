//******************************************************************************************************
//  ObservationState.cs
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
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents all of the possible states for a <see cref="LinearStateEstimator.Modeling.Node"/> during observability analysis.
    /// </summary>
    [Serializable()]
    public enum ObservationState
    {
        /// <summary>
        /// Indicates direct observability of the <see cref="LinearStateEstimator.Modeling.Node"/> via a voltage phasor
        /// </summary>
        [XmlEnum("Direct")]
        DirectlyObserved,

        /// <summary>
        /// Indicates indirect observability of the <see cref="LinearStateEstimator.Modeling.Node"/> via a neighboring voltage phasor and/or current phasor
        /// </summary>
        [XmlEnum("Indirect")]
        IndirectlyObserved,

        /// <summary>
        /// Indicates that the present measurement set cannot observe the <see cref="LinearStateEstimator.Modeling.Node"/> .
        /// </summary>
        [XmlEnum("Unobserved")]
        Unobserved
    }
}
