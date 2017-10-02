//******************************************************************************************************
//  LocationFlag.cs
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
using System.Threading.Tasks;

namespace LinearStateEstimator.Islanding
{
    /// <summary>
    /// A flag used by the <see cref="IslandingAnalyzer"/> to indicate the geographic location of the islanding conditin.
    /// </summary>
    public enum LocationFlag
    {
        /// <summary>
        /// No islanding condition is occurring, therefore, no geographical location can be set.
        /// </summary>
        NoIslandingCondition,

        /// <summary>
        /// Corresponds to an islanding the geographical region of Yorktown.
        /// </summary>
        Yorktown,

        /// <summary>
        /// Corresponds to an islanding the geographical region of Chesterfield.
        /// </summary>
        Chesterfield,

        /// <summary>
        /// Corresponds to an islanding the geographical region of Chesapeake.
        /// </summary>
        Chesapeake,

        /// <summary>
        /// Corresponds to an islanding condition in which the geographical location cannot be determined.
        /// </summary>
        Undetermined
    }
}
