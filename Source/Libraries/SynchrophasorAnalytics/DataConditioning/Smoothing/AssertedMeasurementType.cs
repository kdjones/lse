//******************************************************************************************************
//  AssertedMeasurementType.cs
//
//  Copyright © 2012, Kevin D. Jones.  All Rights Reserved.
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
//  11/01/2012 - Kevin D. Jones
//       Generated original version of source code in Matlab
//  04/05/2014 - Kevin D. Jones
//       Translated and refactored original version of source code into C#
//  06/14/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.DataConditioning.Smoothing
{
    /// <summary>
    /// An enumeration which dictates the type of measurement that is asserted as the actual measurement in the smoothing algorithm of the <see cref="LinearStateEstimator.DataConditioning.Smoothing.Smoother"/> class.
    /// </summary>
    public enum AssertedMeasurementType
    {
        /// <summary>
        /// Represents an asserted measurement which is below the observation residual threshold/tolerance.
        /// </summary>
        RawMeasurement,

        /// <summary>
        /// Represents an asserted measurement which was previously the optimal predicted estimate for a measurement which was outside the observation residual threshold/tolerance.
        /// </summary>
        OptimalPredictedEstimate
    }
}
