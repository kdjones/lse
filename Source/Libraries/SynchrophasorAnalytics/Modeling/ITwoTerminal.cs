//******************************************************************************************************
//  ITwoTerminal.cs
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

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Defines basic properties common to all elements in the power system that are connected to two terminals.
    /// </summary>
    public interface ITwoTerminal
    {
        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> which the two terminal device originates.
        /// </summary>
        Node FromNode
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.FromNode"/>
        /// </summary>
        int FromNodeID
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> which the two terminal device terminates.
        /// </summary>
        Node ToNode
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.ToNode"/>
        /// </summary>
        int ToNodeID
        {
            get;
            set;
        }
    }
}
