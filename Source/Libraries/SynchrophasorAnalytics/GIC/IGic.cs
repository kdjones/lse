//******************************************************************************************************
//  IGic.cs
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
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.GIC
{
    /// <summary>
    /// The interface which defines how transformers should supply information about GIC flow during GMD events
    /// </summary>
    public interface IGic
    {
        /// <summary>
        /// The GIC flow in positive sequence as measured by the <b>From</b> side of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        double PositiveSequenceGicQFlowByFromSide
        {
            get;
        }

        /// <summary>
        /// The GIC flow in positive sequence as measured by the <b>To</b> side of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        double PositiveSequenceGicFlowByToSide
        {
            get;
        }

        /// <summary>
        /// The GIC flow in thre phase quantities as measured by the <b>From</b> side of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        double ThreePhaseGicQFlowByFromSide
        {
            get;
        }

        /// <summary>
        /// The GIC flow in thre phase quantities as measured by the <b>To</b> side of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        double ThreePhaseGicFlowByToSide
        {
            get;
        }
    }
}
