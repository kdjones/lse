//******************************************************************************************************
//  IPhasor.cs
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
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// The interface which describes how a phasor should behave.
    /// </summary>
    public interface IPhasor
    {
        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the <see cref="LinearStateEstimator.Measurements.IPhasor"/>
        /// </summary>
        VoltageLevel BaseKV
        {
            get;
            set;
        }

        /// <summary>
        /// The magnitude of the phasor value.
        /// </summary>
        double Magnitude
        {
            get;
            set;
        }

        /// <summary>
        /// The per-unitized magnitude of the phasor value.
        /// </summary>
        double PerUnitMagnitude
        {
            get;
            set;
        }

        /// <summary>
        /// The angle of the phasor value in degrees.
        /// </summary>
        double AngleInDegrees
        {
            get;
            set;
        }

        /// <summary>
        /// The angle of the phasor value in radians.
        /// </summary>
        double AngleInRadians
        {
            get;
            set;
        }

        /// <summary>
        /// The phasor value as a complex number
        /// </summary>
        Complex ComplexPhasor
        {
            get;
            set;
        }

        /// <summary>
        /// The phasor value as a per unit complex number
        /// </summary>
        Complex PerUnitComplexPhasor
        {
            get;
            set;
        }

        /// <summary>
        /// The measurement key that the magnitude should respond to.
        /// </summary>
        string MagnitudeKey
        {
            get;
            set;
        }

        /// <summary>
        /// The measurement key that the angle should respond to.
        /// </summary>
        string AngleKey
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the <see cref="LinearStateEstimator.Measurements.PhasorType"/> of the measurement.
        /// </summary>
        PhasorType Type
        {
            get;
            set;
        }

    }
}
