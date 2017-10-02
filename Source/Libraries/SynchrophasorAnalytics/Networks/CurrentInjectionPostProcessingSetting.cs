//******************************************************************************************************
//  CurrentInjectionPostProcessingSetting.cs
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
//  07/27/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Networks
{
    /// <summary>
    /// A setting which indicates whether, during post-processing, if current injections should be calculated for shunt branches for only if that shunt branch is inferred as in-service by the presence of a current injection measurement or if the connected node can be classified as <i>observed</i> irrespective of the true status of the logical switching devices which connect them to a bus at the substation.
    /// </summary>
    public enum CurrentInjectionPostProcessingSetting
    {
        /// <summary>
        /// Only allows for computation of shunts which are measured by a current injection phasor of sufficient quality.
        /// </summary>
        ProcessOnlyMeasuredShunts,

        /// <summary>
        /// Allows for computation of shunts as long as the connected node can be considered <i>observed</i>.
        /// </summary>
        ProcessShuntsByNodeObservability
    }
}
