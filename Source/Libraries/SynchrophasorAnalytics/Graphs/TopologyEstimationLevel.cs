//******************************************************************************************************
//  TopologyEstimationLevel.cs
//
//  Copyright © 2017, Kevin D. Jones.  All Rights Reserved.
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
//  08/29/17- Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// This setting dictates the assumptions used by the topology estimator and topology processor
    /// when reducing the network model from a node-breaker to a bus-branch model for state computation.
    /// This setting is made on a per-substation basis.
    /// </summary>
    public enum TopologyEstimationLevel
    {
        /// <summary>
        /// Each measured, energized node becomes its own bus and state variable.
        /// </summary>
        Zero,

        /// <summary>
        /// Each measured, energized node is grouped together based only on available breaker telemetry.
        /// </summary>
        One,

        /// <summary>
        /// Encompasses Level One behavior but supplements breaker statuses with angle-across breaker
        /// comparied to a coherency threshold to act as pseudo breaker telemetry.
        /// </summary>
        Two,

        /// <summary>
        /// All node groupings are determined by voltage coherency thresholds.
        /// </summary>
        Three,

        /// <summary>
        /// Encompasses behavior in Level Three but supplements with availalble breaker telemetry
        /// and angle-across breaker pseudo telemetry to provide a more complete picture. This level
        /// enables detection of bad topology telemetry and topology errors.
        /// </summary>
        Four,
    }
}
