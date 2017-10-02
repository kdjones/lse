//******************************************************************************************************
//  CurrentFlowPostProcessingSetting.cs
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
//  06/23/2014 - Kevin D. Jones
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
    /// A setting which indicates whether, during post-processing, if current flows should be calculated for series branches for only if that series branch is inferred as in-service by the presence of a current flow measurement or if the from and to nodes can be classified as <i>observed</i> irrespective of the true status of the logical switching devices which connect them to a bus at the substation.
    /// </summary>
    public enum CurrentFlowPostProcessingSetting
    {
        /// <summary>
        /// Only allows for computation of branches which are measured by at least 1 current flow phasor of sufficient quality.
        /// </summary>
        ProcessOnlyMeasuredBranches,

        /// <summary>
        /// Allows for computation of branches as long as both from and to nodes can be considered <i>observed</i>.
        /// </summary>
        ProcessBranchesByNodeObservability
    }
}
