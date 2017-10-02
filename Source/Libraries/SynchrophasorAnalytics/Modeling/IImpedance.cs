//******************************************************************************************************
//  IImpedance.cs
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
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//  06/13/2014 - Kevin D. Jones
//       Added properties for From/To side shunt susceptance.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// The interface which defines how network elements that have impedance data based on a two-port pi-model
    /// </summary>
    public interface IImpedance
    {
        /// <summary>
        /// The positive sequence complex (R + jX) series impedance.
        /// </summary>
        Complex PositiveSequenceSeriesImpedance
        {
            get;
        }

        /// <summary>
        /// The positive sequence complex 1/(R + jX) series admittance.
        /// </summary>
        Complex PositiveSequenceSeriesAdmittance
        {
            get;
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance.
        /// </summary>
        Complex PositiveSequenceShuntSusceptance
        {
            get;
        }
        
        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance for a single side of the series branch (modeled value divided by 2).
        /// </summary>
        Complex PositiveSequenceSingleSidedShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance for <b>From</b> side of the series branch. This property should be used when each side of the series branch has different shunt values. For example, in the case of a phase shifting transformer.
        /// </summary>
        Complex PositiveSequenceFromSideShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance for <b>To</b> side of the series branch. This property should be used when each side of the series branch has different shunt values. For example, in the case of a phase shifting transformer.
        /// </summary>
        Complex PositiveSequenceToSideShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The three phase complex (R +jX) series impedance as a 3 x 3 complex matrix.
        /// </summary>
        DenseMatrix ThreePhaseSeriesImpedance
        {
            get;
        }

        /// <summary>
        /// The three phase complex 1/(R +jX) series admittance as a 3 x 3 complex matrix.
        /// </summary>
        DenseMatrix ThreePhaseSeriesAdmittance
        {
            get;
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix.
        /// </summary>
        DenseMatrix ThreePhaseShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix for a single side of the series branch. (divided by 2)
        /// </summary>
        DenseMatrix ThreePhaseSingleSidedShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for <b>From</b> side of the series branch. This property should be used when each side of the series branch has different shunt values. For example, in the case of a phase shifting transformer.
        /// </summary>
        DenseMatrix ThreePhaseFromSideShuntSusceptance
        {
            get;
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for <b>To</b> side of the series branch. This property should be used when each side of the series branch has different shunt values. For example, in the case of a phase shifting transformer.
        /// </summary>
        DenseMatrix ThreePhaseToSideShuntSusceptance
        {
            get;
        }

    }
}
