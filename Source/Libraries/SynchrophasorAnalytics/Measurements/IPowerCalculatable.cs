//******************************************************************************************************
//  IPowerCalculatable.cs
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

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Defines how an object should behave if it can calculate complex power.
    /// </summary>
    interface IPowerCalculatable
    {
        /// <summary>
        /// The three phase complex power
        /// </summary>
        Complex ThreePhaseComplexPower
        {
            get;
        }

        /// <summary>
        /// The three phase apparent power (magnitude of the complex power).
        /// </summary>
        double ThreePhaseApparentPower
        {
            get;
        }

        /// <summary>
        /// The three phase real power (real part of the complex power).
        /// </summary>
        double ThreePhaseRealPower
        {
            get;
        }

        /// <summary>
        /// The three phase reactive power (imaginary part of the complex power).
        /// </summary>
        double ThreePhaseReactivePower
        {
            get;
        }

        /// <summary>
        /// The positive sequence complex power.
        /// </summary>
        Complex PositiveSequenceComplexPower
        {
            get;
        }

        /// <summary>
        /// The positive sequence apparent power (magnitude of the complex power).
        /// </summary>
        double PositiveSequenceApparentPower
        {
            get;
        }

        /// <summary>
        /// The positive sequence real power (real part of the complex power).
        /// </summary>
        double PositiveSequenceRealPower
        {
            get;
        }

        /// <summary>
        /// The positive sequence reactive power (imaginary part of the complex power).
        /// </summary>
        double PositiveSequenceReactivePower
        {
            get;
        }

        /// <summary>
        /// The A phase complex power.
        /// </summary>
        Complex PhaseAComplexPower
        {
            get;
        }

        /// <summary>
        /// The A phase apparent power (magnitude of the complex power).
        /// </summary>
        double PhaseAApparentPower
        {
            get;
        }

        /// <summary>
        /// The A phase real power (real part of the complex power).
        /// </summary>
        double PhaseARealPower
        {
            get;
        }

        /// <summary>
        /// The A phase reactive power (imaginary part of the complex power).
        /// </summary>
        double PhaseAReactivePower
        {
            get;
        }

        /// <summary>
        /// The B phase complex power.
        /// </summary>
        Complex PhaseBComplexPower
        {
            get;
        }

        /// <summary>
        /// The B phase apparent power (magnitude of the complex power).
        /// </summary>
        double PhaseBApparentPower
        {
            get;
        }

        /// <summary>
        /// The B phase real power (real part of the complex power).
        /// </summary>
        double PhaseBRealPower
        {
            get;
        }

        /// <summary>
        /// The B phase reactive power (imaginary part of the complex power).
        /// </summary>
        double PhaseBReactivePower
        {
            get;
        }

        /// <summary>
        /// The C phase complex power.
        /// </summary>
        Complex PhaseCComplexPower
        {
            get;
        }

        /// <summary>
        /// The C phase apparent power (magnitude of the complex power).
        /// </summary>
        double PhaseCApparentPower
        {
            get;
        }

        /// <summary>
        /// The C phase real power (real part of the complex power).
        /// </summary>
        double PhaseCRealPower
        {
            get;
        }

        /// <summary>
        /// The C phase reactive power (imaginary part of the complex power).
        /// </summary>
        double PhaseCReactivePower
        {
            get;
        }
    }
}
