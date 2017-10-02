//******************************************************************************************************
//  Impedance.cs
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
//  09/17/2013 - Kevin D. Jones
//       Added substration (-) operator for comparing Impedance for series compensation estimation.
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//  06/13/2014 - Kevin D. Jones
//       Added functionality for From/To side shunt susceptance with different values.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// A class implementation of the impedance model of a two-port pi-model.
    /// </summary>
    [Serializable()]
    public class Impedance : IImpedance, IComparable
    {
        #region [ Private Members ]

        #region [ Resistance ]
        private double m_r1;
        private double m_r2;
        private double m_r3;
        private double m_r4;
        private double m_r5;
        private double m_r6;
        #endregion

        #region [ Reactance ]
        private double m_x1;
        private double m_x2;
        private double m_x3;
        private double m_x4;
        private double m_x5;
        private double m_x6;
        #endregion

        #region [ Conductance ]

        private double m_g1From;
        private double m_g2From;
        private double m_g3From;
        private double m_g4From;
        private double m_g5From;
        private double m_g6From;

        private double m_g1To;
        private double m_g2To;
        private double m_g3To;
        private double m_g4To;
        private double m_g5To;
        private double m_g6To;

        #endregion

        #region [ Susceptance ]

        private double m_b1From;
        private double m_b2From;
        private double m_b3From;
        private double m_b4From;
        private double m_b5From;
        private double m_b6From;

        private double m_b1To;
        private double m_b2To;
        private double m_b3To;
        private double m_b4To;
        private double m_b5To;
        private double m_b6To;

        #endregion

        /// <summary>
        /// Parent
        /// </summary>
        private INetworkDescribable m_parentElement;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The complex (R + jX) series impedance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSeriesImpedance
        {
            get
            {
                Complex Zs = (ThreePhaseSeriesImpedance[0, 0] + ThreePhaseSeriesImpedance[1, 1] + ThreePhaseSeriesImpedance[2, 2]) / 3;
                Complex Zm = (ThreePhaseSeriesImpedance[0, 1] + ThreePhaseSeriesImpedance[0, 2] + ThreePhaseSeriesImpedance[1, 2]) / 3;
                return (Zs - Zm);
            }
        }

        /// <summary>
        /// The complex 1/(R + jX) series admittance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSeriesAdmittance
        {
            get 
            { 
                return (1 / PositiveSequenceSeriesImpedance); 
            }
        }

        /// <summary>
        /// The complex (G + jB) shunt susceptance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceShuntSusceptance
        {
            get
            {
                Complex Bs = (ThreePhaseShuntSusceptance[0, 0] + ThreePhaseShuntSusceptance[1, 1] + ThreePhaseShuntSusceptance[2, 2]) / 3;
                Complex Bm = (ThreePhaseShuntSusceptance[0, 1] + ThreePhaseShuntSusceptance[0, 2] + ThreePhaseShuntSusceptance[1, 2]) / 3;
                return (Bs - Bm);
            }
        }

        /// <summary>
        /// The complex (G + jB) shunt susceptance for a single side of the series branch (divided by 2)
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSingleSidedShuntSusceptance
        {
            get
            {
                Complex Bs = (ThreePhaseSingleSidedShuntSusceptance[0, 0] + ThreePhaseSingleSidedShuntSusceptance[1, 1] + ThreePhaseSingleSidedShuntSusceptance[2, 2]) / 3;
                Complex Bm = (ThreePhaseSingleSidedShuntSusceptance[0, 1] + ThreePhaseSingleSidedShuntSusceptance[0, 2] + ThreePhaseSingleSidedShuntSusceptance[1, 2]) / 3;
                return (Bs - Bm);
            }
        }

        /// <summary>
        /// The positive sequence comlex (G + jB) shunt susceptance for the <b>From</b> side of the branch. This should be used when each side of the series branch has a different value for shunt susceptance such as with a phase shifting transformer. 
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceFromSideShuntSusceptance
        {
            get
            {
                Complex Bs = (ThreePhaseFromSideShuntSusceptance[0, 0] + ThreePhaseFromSideShuntSusceptance[1, 1] + ThreePhaseFromSideShuntSusceptance[2, 2]) / 3;
                Complex Bm = (ThreePhaseFromSideShuntSusceptance[0, 1] + ThreePhaseFromSideShuntSusceptance[0, 2] + ThreePhaseFromSideShuntSusceptance[1, 2]) / 3;
                return (Bs - Bm);
            }
        }

        /// <summary>
        /// The positive sequence comlex (G + jB) shunt susceptance for the <b>To</b> side of the branch. This should be used when each side of the series branch has a different value for shunt susceptance such as with a phase shifting transformer. 
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceToSideShuntSusceptance
        {
            get
            {
                Complex Bs = (ThreePhaseToSideShuntSusceptance[0, 0] + ThreePhaseToSideShuntSusceptance[1, 1] + ThreePhaseToSideShuntSusceptance[2, 2]) / 3;
                Complex Bm = (ThreePhaseToSideShuntSusceptance[0, 1] + ThreePhaseToSideShuntSusceptance[0, 2] + ThreePhaseToSideShuntSusceptance[1, 2]) / 3;
                return (Bs - Bm);
            }
        }

        /// <summary>
        /// The three phase complex (R + jX) series impedance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSeriesImpedance
        {
            get
            {
                DenseMatrix impedance = DenseMatrix.OfArray(new Complex[,] 
                               {{(Z1),(Z2),(Z4)},
                                {(Z2),(Z3),(Z5)},
                                {(Z4),(Z5),(Z6)}});
                return impedance;
            }
            set
            {
                Z1 = value[0, 0];
                Z2 = value[0, 1];
                Z3 = value[1, 1];
                Z4 = value[0, 2];
                Z5 = value[1, 2];
                Z6 = value[2, 2];
            }
        }

        /// <summary>
        /// The three phase complex 1/(R +jX) series admittance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSeriesAdmittance
        {
            get 
            {
                return ThreePhaseSeriesImpedance.Inverse() as DenseMatrix;
            }
            set
            {
                ThreePhaseSeriesImpedance = value.Inverse() as DenseMatrix;
            }
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseShuntSusceptance
        {
            get
            {
                DenseMatrix susceptance = DenseMatrix.OfArray(new Complex[,] 
                               {{(Y1),(Y2),(Y4)},
                                {(Y2),(Y3),(Y5)},
                                {(Y4),(Y5),(Y6)}});
                return susceptance;
            }
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix for a single side of the series branch. (divided by 2)
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSingleSidedShuntSusceptance
        {
            get
            {
                DenseMatrix susceptance = DenseMatrix.OfArray(new Complex[,] 
                               {{(Y1 / 2),(Y2 / 2),(Y4 /2)},
                                {(Y2 / 2),(Y3 / 2),(Y5 /2)},
                                {(Y4 / 2),(Y5 / 2),(Y6 /2)}});
                return susceptance;
            }
            set
            {
                Y1 = value[0, 0];
                Y2 = value[0, 1];
                Y3 = value[1, 1];
                Y4 = value[0, 2];
                Y5 = value[1, 2];
                Y6 = value[2, 2];
            }
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for the <b>From</b> side of the branch. This should be used when each side of the series branch has a different value for shunt susceptance such as with a phase shifting transformer. 
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseFromSideShuntSusceptance
        {
            get
            {
                DenseMatrix susceptance = DenseMatrix.OfArray(new Complex[,] 
                               {{(Y1FromSide / 2),(Y2FromSide / 2),(Y4FromSide /2)},
                                {(Y2FromSide / 2),(Y3FromSide / 2),(Y5FromSide /2)},
                                {(Y4FromSide / 2),(Y5FromSide / 2),(Y6FromSide /2)}});
                return susceptance;
            }
            set
            {
                Y1FromSide = value[0, 0];
                Y2FromSide = value[0, 1];
                Y3FromSide = value[1, 1];
                Y4FromSide = value[0, 2];
                Y5FromSide = value[1, 2];
                Y6FromSide = value[2, 2];
            }
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for the <b>To</b> side of the branch. This should be used when each side of the series branch has a different value for shunt susceptance such as with a phase shifting transformer. 
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseToSideShuntSusceptance
        {
            get
            {
                DenseMatrix susceptance = DenseMatrix.OfArray(new Complex[,] 
                               {{(Y1ToSide / 2),(Y2ToSide / 2),(Y4ToSide /2)},
                                {(Y2ToSide / 2),(Y3ToSide / 2),(Y5ToSide /2)},
                                {(Y4ToSide / 2),(Y5ToSide / 2),(Y6ToSide /2)}});
                return susceptance;
            }
            set
            {
                Y1ToSide = value[0, 0];
                Y2ToSide = value[0, 1];
                Y3ToSide = value[1, 1];
                Y4ToSide = value[0, 2];
                Y5ToSide = value[1, 2];
                Y6ToSide = value[2, 2];
            }
        }

        #region [ Resistance ]

        /// <summary>
        /// The real part of the (1, 1) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R1")]
        public double R1
        {
            get
            {
                return m_r1;
            }
            set
            {
                m_r1 = value;
            }
        }

        /// <summary>
        /// The real part of the (1, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R2")]
        public double R2
        {
            get
            {
                return m_r2;
            }
            set
            {
                m_r2 = value;
            }
        }

        /// <summary>
        /// The real part of the (2, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R3")]
        public double R3
        {
            get
            {
                return m_r3;
            }
            set
            {
                m_r3 = value;
            }
        }

        /// <summary>
        /// The real part of the (1, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R4")]
        public double R4
        {
            get
            {
                return m_r4;
            }
            set
            {
                m_r4 = value;
            }
        }

        /// <summary>
        /// The real part of the (2, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R5")]
        public double R5
        {
            get
            {
                return m_r5;
            }
            set
            {
                m_r5 = value;
            }
        }

        /// <summary>
        /// The real part of the (3, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("R6")]
        public double R6
        {
            get
            {
                return m_r6;
            }
            set
            {
                m_r6 = value;
            }
        }

        #endregion

        #region [ Reactance ]

        /// <summary>
        /// The imaginary part of the (1, 1) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X1")]
        public double X1
        {
            get
            {
                return m_x1;
            }
            set
            {
                m_x1 = value;
            }
        }

        /// <summary>
        /// The imaginary part of the (1, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X2")]
        public double X2
        {
            get
            {
                return m_x2;
            }
            set
            {
                m_x2 = value;
            }
        }

        /// <summary>
        /// The imaginary part of the (2, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X3")]
        public double X3
        {
            get
            {
                return m_x3;
            }
            set
            {
                m_x3 = value;
            }
        }

        /// <summary>
        /// The imaginary part of the (1, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X4")]
        public double X4
        {
            get
            {
                return m_x4;
            }
            set
            {
                m_x4 = value;
            }
        }

        /// <summary>
        /// The imaginary part of the (2, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X5")]
        public double X5
        {
            get
            {
                return m_x5;
            }
            set
            {
                m_x5 = value;
            }
        }

        /// <summary>
        /// The imaginary part of the (3, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlAttribute("X6")]
        public double X6
        {
            get
            {
                return m_x6;
            }
            set
            {
                m_x6 = value;
            }
        }

        #endregion

        #region [ Conductance ]

        /// <summary>
        /// The real part of the (1, 1) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G1")]
        public double G1
        {
            get
            {
                return m_g1From + m_g1To;
            }
            set
            {
                m_g1From = value / 2;
                m_g1To = value / 2;
            }
        }

        /// <summary>
        /// The real part of the (1, 2) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G2")]
        public double G2
        {
            get
            {
                return m_g2From + m_g2To;
            }
            set
            {
                m_g2From = value / 2;
                m_g2To = value / 2;
            }
        }

        /// <summary>
        /// The real part of the (2, 2) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G3")]
        public double G3
        {
            get
            {
                return m_g3From + m_g3To;
            }
            set
            {
                m_g3From = value / 2;
                m_g3To = value / 2;
            }
        }

        /// <summary>
        /// The real part of the (1, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G4")]
        public double G4
        {
            get
            {
                return m_g4From + m_g4To;
            }
            set
            {
                m_g4From = value / 2;
                m_g4To = value / 2;
            }
        }

        /// <summary>
        /// The real part of the (2, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G5")]
        public double G5
        {
            get
            {
                return m_g5From + m_g5To;
            }
            set
            {
                m_g5From = value / 2;
                m_g5To = value / 2;
            }
        }

        /// <summary>
        /// The real part of the (3, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("G6")]
        public double G6
        {
            get
            {
                return m_g6From + m_g6To;
            }
            set
            {
                m_g6From = value / 2;
                m_g6To = value / 2;
            }
        }

        #endregion

        #region [ Susceptance ]

        /// <summary>
        /// The imaginary part of the (1, 1) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B1")]
        public double B1
        {
            get
            {
                return m_b1From + m_b1To;
            }
            set
            {
                m_b1From = value / 2;
                m_b1To = value / 2;
            }
        }

        /// <summary>
        /// The imaginary part of the (1, 2) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B2")]
        public double B2
        {
            get
            {
                return m_b2From + m_b2To;
            }
            set
            {
                m_b2From = value / 2;
                m_b2To = value / 2;
            }
        }

        /// <summary>
        /// The imaginary part of the (2, 2) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B3")]
        public double B3
        {
            get
            {
                return m_b3From + m_b3To;
            }
            set
            {
                m_b3From = value / 2;
                m_b3To = value / 2;
            }
        }

        /// <summary>
        /// The imaginary part of the (1, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B4")]
        public double B4
        {
            get
            {
                return m_b4From + m_b4To;
            }
            set
            {
                m_b4From = value / 2;
                m_b4To = value / 2;
            }
        }

        /// <summary>
        /// The imaginary part of the (2, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B5")]
        public double B5
        {
            get
            {
                return m_b5From + m_b5To;
            }
            set
            {
                m_b5From = value / 2;
                m_b5To = value / 2;
            }
        }

        /// <summary>
        /// The imaginary part of the (3, 3) element of the 3x3 shunt susceptance matrix. 
        /// </summary>
        [XmlAttribute("B6")]
        public double B6
        {
            get
            {
                return m_b6From + m_b6To;
            }
            set
            {
                m_b6From = value / 2;
                m_b6To = value / 2;
            }
        }

        #endregion

        #region [ Complex Series Impedance ]

        /// <summary>
        /// The complex (1, 1) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z1
        {
            get
            {
                return new Complex(m_r1, m_x1);
            }
            set
            {
                m_r1 = value.Real;
                m_x1 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z1
        /// </summary>
        [XmlIgnore()]
        private string Z1FormattedString
        {
            get
            {
                if (X1 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R1.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X1).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R1.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X1).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (1, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z2
        {
            get
            {
                return new Complex(m_r2, m_x2);
            }
            set
            {
                m_r2 = value.Real;
                m_x2 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z2
        /// </summary>
        [XmlIgnore()]
        private string Z2FormattedString
        {
            get
            {
                if (X2 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R2.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X2).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R2.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X2).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (2, 2) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z3
        {
            get
            {
                return new Complex(m_r3, m_x3);
            }
            set
            {
                m_r3 = value.Real;
                m_x3 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z3
        /// </summary>
        [XmlIgnore()]
        private string Z3FormattedString
        {
            get
            {
                if (X3 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R3.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X3).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R3.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X3).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (1, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z4
        {
            get
            {
                return new Complex(m_r4, m_x4);
            }
            set
            {
                m_r4 = value.Real;
                m_x4 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z4
        /// </summary>
        [XmlIgnore()]
        private string Z4FormattedString
        {
            get
            {
                if (X4 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R4.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X4).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R4.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X4).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (2, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z5
        {
            get
            {
                return new Complex(m_r5, m_x5);
            }
            set
            {
                m_r5 = value.Real;
                m_x5 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z5
        /// </summary>
        [XmlIgnore()]
        private string Z5FormattedString
        {
            get
            {
                if (X5 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R5.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X5).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R5.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X5).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (3, 3) element of the 3x3 impedance matrix. 
        /// </summary>
        [XmlIgnore()]
        public Complex Z6
        {
            get
            {
                return new Complex(m_r6, m_x6);
            }
            set
            {
                m_r6 = value.Real;
                m_x6 = value.Imaginary;
            }
        }

        /// <summary>
        /// A formatted string version of Z6
        /// </summary>
        [XmlIgnore()]
        private string Z6FormattedString
        {
            get
            {
                if (X6 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R6.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X6).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(R6.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(X6).ToString());
                }
            }
        }

        #endregion

        #region [ Complex Shunt Admittance ]

        /// <summary>
        /// The complex (1, 1) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value.
        /// </summary>
        [XmlIgnore()]
        public Complex Y1
        {
            get
            {
                return new Complex(m_g1From + m_g1To, m_b1From + m_b1To);
            }
            set
            {
                m_g1From = value.Real / 2;
                m_b1From = value.Imaginary / 2;
                m_g1To = value.Real / 2;
                m_b1To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y1
        /// </summary>
        [XmlIgnore()]
        private string Y1FormattedString
        {
            get
            {
                if (B1 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G1.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B1).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G1.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B1).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (1, 2) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value.  
        /// </summary>
        [XmlIgnore()]
        public Complex Y2
        {
            get
            {
                return new Complex(m_g2From + m_g2To, m_b2From + m_b2To);
            }
            set
            {
                m_g2From = value.Real / 2;
                m_b2From = value.Imaginary / 2;
                m_g2To = value.Real / 2;
                m_b2To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y2
        /// </summary>
        [XmlIgnore()]
        private string Y2FormattedString
        {
            get
            {
                if (B2 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G2.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B2).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G2.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B2).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (2, 2) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value. 
        /// </summary>
        [XmlIgnore()]
        public Complex Y3
        {
            get
            {
                return new Complex(m_g3From + m_g3To, m_b3From + m_b3To);
            }
            set
            {
                m_g3From = value.Real / 2;
                m_b3From = value.Imaginary / 2;
                m_g3To = value.Real / 2;
                m_b3To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y3
        /// </summary>
        [XmlIgnore()]
        private string Y3FormattedString
        {
            get
            {
                if (B3 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G3.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B3).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G3.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B3).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (1, 3) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value. 
        /// </summary>
        [XmlIgnore()]
        public Complex Y4
        {
            get
            {
                return new Complex(m_g4From + m_g4To, m_b4From + m_b4To);
            }
            set
            {
                m_g4From = value.Real / 2;
                m_b4From = value.Imaginary / 2;
                m_g4To = value.Real / 2;
                m_b4To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y4
        /// </summary>
        [XmlIgnore()]
        private string Y4FormattedString
        {
            get
            {
                if (B4 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G4.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B4).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G4.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B4).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (2, 3) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value. 
        /// </summary>
        [XmlIgnore()]
        public Complex Y5
        {
            get
            {
                return new Complex(m_g5From + m_g5To, m_b5From + m_b5To);
            }
            set
            {
                m_g5From = value.Real / 2;
                m_b5From = value.Imaginary / 2;
                m_g5To = value.Real / 2;
                m_b5To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y5
        /// </summary>
        [XmlIgnore()]
        private string Y5FormattedString
        {
            get
            {
                if (B5 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G5.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B5).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G5.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B5).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (3, 3) element of the 3x3 shunt susceptance matrix assuming both sides of the series branch have equal shunt susceptance values. This is the total value. 
        /// </summary>
        [XmlIgnore()]
        public Complex Y6
        {
            get
            {
                return new Complex(m_g6From + m_g6To, m_b6From + m_b6To);
            }
            set
            {
                m_g6From = value.Real / 2;
                m_b6From = value.Imaginary / 2;
                m_g6To = value.Real / 2;
                m_b6To = value.Imaginary / 2;
            }
        }

        /// <summary>
        /// A formatted string version of Y6
        /// </summary>
        [XmlIgnore()]
        private string Y6FormattedString
        {
            get
            {
                if (B6 < 0)
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G6.ToString()) + " - j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B6).ToString());
                }
                else
                {
                    return TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(G6.ToString()) + " + j" + TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(Math.Abs(B6).ToString());
                }
            }
        }

        /// <summary>
        /// The complex (1, 1) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y1FromSide
        {
            get
            {
                return new Complex(m_g1From, m_b1From);
            }
            set
            {
                m_g1From = value.Real;
                m_b1From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (1, 2) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y2FromSide
        {
            get
            {
                return new Complex(m_g2From, m_b2From);
            }
            set
            {
                m_g2From = value.Real;
                m_b2From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (2, 2) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y3FromSide
        {
            get
            {
                return new Complex(m_g3From, m_b3From);
            }
            set
            {
                m_g3From = value.Real;
                m_b3From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (1, 3) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y4FromSide
        {
            get
            {
                return new Complex(m_g4From, m_b4From);
            }
            set
            {
                m_g4From = value.Real;
                m_b4From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (2, 3) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y5FromSide
        {
            get
            {
                return new Complex(m_g5From, m_b5From);
            }
            set
            {
                m_g5From = value.Real;
                m_b5From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (3, 3) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y6FromSide
        {
            get
            {
                return new Complex(m_g6From, m_b6From);
            }
            set
            {
                m_g6From = value.Real;
                m_b6From = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (1, 1) element of the 3x3 shunt susceptance matrix for the to side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y1ToSide
        {
            get
            {
                return new Complex(m_g1To, m_b1To);
            }
            set
            {
                m_g1To = value.Real;
                m_b1To = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (1, 2) element of the 3x3 shunt susceptance matrix for the to side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y2ToSide
        {
            get
            {
                return new Complex(m_g2To, m_b2To);
            }
            set
            {
                m_g2To = value.Real;
                m_b2To = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (2, 2) element of the 3x3 shunt susceptance matrix for the to side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y3ToSide
        {
            get
            {
                return new Complex(m_g3To, m_b3To);
            }
            set
            {
                m_g3To = value.Real;
                m_b3To = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (1, 3) element of the 3x3 shunt susceptance matrix for the to side of the two-port pi-model  
        /// </summary>
        [XmlIgnore()]
        public Complex Y4ToSide
        {
            get
            {
                return new Complex(m_g4To, m_b4To);
            }
            set
            {
                m_g4To = value.Real;
                m_b4To = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (2, 3) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y5ToSide
        {
            get
            {
                return new Complex(m_g5To, m_b5To);
            }
            set
            {
                m_g5To = value.Real;
                m_b5To = value.Imaginary;
            }
        }

        /// <summary>
        /// The complex (3, 3) element of the 3x3 shunt susceptance matrix for the from side of the two-port pi-model 
        /// </summary>
        [XmlIgnore()]
        public Complex Y6ToSide
        {
            get
            {
                return new Complex(m_g6To, m_b6To);
            }
            set
            {
                m_g6To = value.Real;
                m_b6To = value.Imaginary;
            }
        }

        #endregion

        /// <summary>
        /// The network element which owns the instance of the <see cref="LinearStateEstimator.Modeling.Impedance"/>.
        /// </summary>
        [XmlIgnore()]
        public INetworkDescribable ParentElement
        {
            get
            {
                return m_parentElement;
            }
            set
            {
                m_parentElement = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default (zero) values.
        /// </summary>
        public Impedance()
            :this(new double[6] {0, 0, 0, 0, 0, 0}, new double[6] {0, 0, 0, 0, 0, 0}, new double[6] {0, 0, 0, 0, 0, 0}, new double[6] {0, 0, 0, 0, 0, 0})
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="Impedance"/> class.
        /// </summary>
        /// <param name="resistance">A double array of the 6 unique resistance elements in the 3x3 impedance matrix.</param>
        /// <param name="reactance">A double array of the 6 unique reactance elements in the 3x3 impedance matrix.</param>
        /// <param name="susceptance">A double array of the 6 unique susceptance elements in the 3x3 shunt susceptance matrix.</param>
        /// <param name="conductance">A double array of the 6 unique conductance elements in the 3x3 shunt conductance matrix.</param>
        public Impedance(double[] resistance, double[] reactance, double[] susceptance, double[] conductance)
        {
            SetResistanceParameters(resistance);
            SetReactanceParameters(reactance);
            SetConductanceParameters(conductance);
            SetSusceptanceParameters(susceptance);
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Impedance"/> class. Has the format of <i>r1,r2,r3,r4,r5,r6,x1,x2,x3,x4,x5,x6,g1,g2,g3,g4,g5,g6,b1,b2,b3,b4,b5,b6</i> and can be used as a rudimentary for of the momento design pattern.
        /// </summary>
        /// <returns>A string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Impedance"/> class.</returns>
        public override string ToString()
        {
            return R1.ToString() + "," + R2.ToString() + "," +R3.ToString() + "," + R4.ToString() + "," + R5.ToString() + "," + R6.ToString() + "," + X1.ToString() + "," + X2.ToString() + "," + X3.ToString() + "," + X4.ToString() + "," + X5.ToString() + "," + X6.ToString() + "," + G1.ToString() + "," + G2.ToString() + "," + G3.ToString() + "," + G4.ToString() + "," + G5.ToString() + "," + G6.ToString() + "," + B1.ToString() + "," + B2.ToString() + "," + B3.ToString() + "," + B4.ToString() + "," + B5.ToString() + "," + B6.ToString();
        }

        /// <summary>
        /// A verbose string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Impedance"/> class and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Impedance"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Two-Port Pi-Model Impedance ----------------------------------------------{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                 (R + jX)        |        (G + jB) {0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (1, 1) | " + Z1FormattedString + " | " + Y1FormattedString + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (1, 2) | " + Z2FormattedString + " | " + Y2FormattedString + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (2, 2) | " + Z3FormattedString + " | " + Y3FormattedString + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (1, 3) | " + Z4FormattedString + " | " + Y4FormattedString + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (2, 3) | " + Z5FormattedString + " | " + Y5FormattedString + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" (3, 3) | " + Z6FormattedString + " | " + Y6FormattedString + "{0}", Environment.NewLine);
            
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Sets all of the impedance values to zero.
        /// </summary>
        public void ClearImpedanceValues()
        {
            SetResistanceParameters(new double[6]);
            SetReactanceParameters(new double[6]);
            SetConductanceParameters(new double[6]);
            SetSusceptanceParameters(new double[6]);
        }

        /// <summary>
        /// Overridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code determined by the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a memberwise clone of the <see cref="LinearStateEstimator.Modeling.Impedance"/> object.
        /// </summary>
        /// <returns>A memberwise clone of the <see cref="LinearStateEstimator.Modeling.Impedance"/> object.</returns>
        public Impedance Copy()
        {
            return (Impedance)this.MemberwiseClone();
        }

        /// <summary>
        /// Implementation of IComparable so that impedance values value be sorted in a list. Impedances are compared using Z1.Magnitude.
        /// </summary>
        /// <param name="target">The target object for comparison</param>
        /// <returns>An integer to indicate comparison results.</returns>
        public int CompareTo(object target)
        {
            if (target == null)
            {
                return 1;
            }

            Impedance impedance = target as Impedance;
            if (impedance != null)
            {
                return Z1.Magnitude.CompareTo(impedance.Z1.Magnitude);
            }
            else
            {
                throw new ArgumentException("Object is not an Impedance");
            }
        }
        
        #endregion

        #region [ Private Methods ]
        
        private void SetResistanceParameters(double[] resistance)
        {
            m_r1 = resistance[0];
            m_r2 = resistance[1];
            m_r3 = resistance[2];
            m_r4 = resistance[3];
            m_r5 = resistance[4];
            m_r6 = resistance[5];
        }

        private void SetReactanceParameters(double[] reactance)
        {
            m_x1 = reactance[0];
            m_x2 = reactance[1];
            m_x3 = reactance[2];
            m_x4 = reactance[3];
            m_x5 = reactance[4];
            m_x6 = reactance[5];
        }

        private void SetConductanceParameters(double[] conductance)
        {
            m_g1From = conductance[0];
            m_g2From = conductance[1];
            m_g3From = conductance[2];
            m_g4From = conductance[3];
            m_g5From = conductance[4];
            m_g6From = conductance[5];

            m_g1To = conductance[0];
            m_g2To = conductance[1];
            m_g3To = conductance[2];
            m_g4To = conductance[3];
            m_g5To = conductance[4];
            m_g6To = conductance[5];
        }

        private void SetSusceptanceParameters(double[] susceptance)
        {
            m_b1From = susceptance[0];
            m_b2From = susceptance[1];
            m_b3From = susceptance[2];
            m_b4From = susceptance[3];
            m_b5From = susceptance[4];
            m_b6From = susceptance[5];

            m_b1To = susceptance[0];
            m_b2To = susceptance[1];
            m_b3To = susceptance[2];
            m_b4To = susceptance[3];
            m_b5To = susceptance[4];
            m_b6To = susceptance[5];
        }

        private string TrimImpedanceStringOrAddSuffixOfZerosIfNeeded(string value)
        {
            if (value.Contains('E'))
            {
                return "0.0000000";
            }
            if (value.Count() > 9)
            {
                return value.Substring(0, 9);
            }
            else if (value.Count() == 9)
            {
                return value;
            }
            else if (value.Count() < 9)
            {
                if (value.Equals("0"))
                {
                    value += "."; 
                    
                }
                int zerosToAdd = 9 - value.Count();
                for (int i = 0; i < zerosToAdd; i++)
                {
                    value += "0";
                }
                return value;
            }
            return value;
        }

        #endregion

        #region [ Operators ]

        /// <summary>
        /// A parameterwise summation of two <see cref="LinearStateEstimator.Modeling.Impedance"/> objects. Order does not matter.
        /// </summary>
        /// <param name="z1">An <see cref="LinearStateEstimator.Modeling.Impedance"/> to be added.</param>
        /// <param name="z2">Another <see cref="LinearStateEstimator.Modeling.Impedance"/> to be added.</param>
        /// <returns>An <see cref="LinearStateEstimator.Modeling.Impedance"/> which is the parameterwise summation of the two inputs.</returns>
        public static Impedance operator + (Impedance z1, Impedance z2)
        {
            Impedance impedance = new Impedance();

            impedance.R1 = z1.R1 + z2.R1;
            impedance.R2 = z1.R2 + z2.R2;
            impedance.R3 = z1.R3 + z2.R3;
            impedance.R4 = z1.R4 + z2.R4;
            impedance.R5 = z1.R5 + z2.R5;
            impedance.R6 = z1.R6 + z2.R6;

            impedance.X1 = z1.X1 + z2.X1;
            impedance.X2 = z1.X2 + z2.X2;
            impedance.X3 = z1.X3 + z2.X3;
            impedance.X4 = z1.X4 + z2.X4;
            impedance.X5 = z1.X5 + z2.X5;
            impedance.X6 = z1.X6 + z2.X6;

            impedance.G1 = z1.G1 + z2.G1;
            impedance.G2 = z1.G2 + z2.G2;
            impedance.G3 = z1.G3 + z2.G3;
            impedance.G4 = z1.G4 + z2.G4;
            impedance.G5 = z1.G5 + z2.G5;
            impedance.G6 = z1.G6 + z2.G6;

            impedance.B1 = z1.B1 + z2.B1;
            impedance.B2 = z1.B2 + z2.B2;
            impedance.B3 = z1.B3 + z2.B3;
            impedance.B4 = z1.B4 + z2.B4;
            impedance.B5 = z1.B5 + z2.B5;
            impedance.B6 = z1.B6 + z2.B6;

            return impedance;
        }

        /// <summary>
        /// A parameterwise difference of two <see cref="LinearStateEstimator.Modeling.Impedance"/> objects. First minus the second.
        /// </summary>
        /// <param name="z1">An <see cref="LinearStateEstimator.Modeling.Impedance"/> to be subtracted from.</param>
        /// <param name="z2">Another <see cref="LinearStateEstimator.Modeling.Impedance"/> to be subtracted.</param>
        /// <returns>An <see cref="LinearStateEstimator.Modeling.Impedance"/> which is the parameterwise difference of the two inputs.</returns>
        public static Impedance operator - (Impedance z1, Impedance z2)
        {
            Impedance impedance = new Impedance();

            impedance.R1 = z1.R1 - z2.R1;
            impedance.R2 = z1.R2 - z2.R2;
            impedance.R3 = z1.R3 - z2.R3;
            impedance.R4 = z1.R4 - z2.R4;
            impedance.R5 = z1.R5 - z2.R5;
            impedance.R6 = z1.R6 - z2.R6;

            impedance.X1 = z1.X1 - z2.X1;
            impedance.X2 = z1.X2 - z2.X2;
            impedance.X3 = z1.X3 - z2.X3;
            impedance.X4 = z1.X4 - z2.X4;
            impedance.X5 = z1.X5 - z2.X5;
            impedance.X6 = z1.X6 - z2.X6;

            impedance.G1 = z1.G1 - z2.G1;
            impedance.G2 = z1.G2 - z2.G2;
            impedance.G3 = z1.G3 - z2.G3;
            impedance.G4 = z1.G4 - z2.G4;
            impedance.G5 = z1.G5 - z2.G5;
            impedance.G6 = z1.G6 - z2.G6;

            impedance.B1 = z1.B1 - z2.B1;
            impedance.B2 = z1.B2 - z2.B2;
            impedance.B3 = z1.B3 - z2.B3;
            impedance.B4 = z1.B4 - z2.B4;
            impedance.B5 = z1.B5 - z2.B5;
            impedance.B6 = z1.B6 - z2.B6;

            return impedance;
        }

        /// <summary>
        /// A scalar multiplication of a <see cref="LinearStateEstimator.Modeling.Impedance"/> object.
        /// </summary>
        /// <param name="z1">An <see cref="LinearStateEstimator.Modeling.Impedance"/> to be scaled.</param>
        /// <param name="scalar">A scalar multiplication factor.</param>
        /// <returns>A new <see cref="LinearStateEstimator.Modeling.Impedance"/> object with scaled parameters. Parent ownership is not retained through the operation.</returns>
        public static Impedance operator * (Impedance z1, double scalar)
        {
            Impedance impedance = new Impedance();

            impedance.R1 = z1.R1 * scalar;
            impedance.R2 = z1.R2 * scalar;
            impedance.R3 = z1.R3 * scalar;
            impedance.R4 = z1.R4 * scalar;
            impedance.R5 = z1.R5 * scalar;
            impedance.R6 = z1.R6 * scalar;

            impedance.X1 = z1.X1 * scalar;
            impedance.X2 = z1.X2 * scalar;
            impedance.X3 = z1.X3 * scalar;
            impedance.X4 = z1.X4 * scalar;
            impedance.X5 = z1.X5 * scalar;
            impedance.X6 = z1.X6 * scalar;

            impedance.G1 = z1.G1 * scalar;
            impedance.G2 = z1.G2 * scalar;
            impedance.G3 = z1.G3 * scalar;
            impedance.G4 = z1.G4 * scalar;
            impedance.G5 = z1.G5 * scalar;
            impedance.G6 = z1.G6 * scalar;

            impedance.B1 = z1.B1 * scalar;
            impedance.B2 = z1.B2 * scalar;
            impedance.B3 = z1.B3 * scalar;
            impedance.B4 = z1.B4 * scalar;
            impedance.B5 = z1.B5 * scalar;
            impedance.B6 = z1.B6 * scalar;

            return impedance;
        }

        /// <summary>
        /// A scalar division of a <see cref="LinearStateEstimator.Modeling.Impedance"/> object.
        /// </summary>
        /// <param name="z1">An <see cref="LinearStateEstimator.Modeling.Impedance"/> to be scaled.</param>
        /// <param name="scalar">A scalar divident.</param>
        /// <returns>A new <see cref="LinearStateEstimator.Modeling.Impedance"/> object with scaled parameters. Parent ownership is not retained through the operation.</returns>
        public static Impedance operator / (Impedance z1, int scalar)
        {
            Impedance impedance = new Impedance();

            impedance.R1 = z1.R1 / scalar;
            impedance.R2 = z1.R2 / scalar;
            impedance.R3 = z1.R3 / scalar;
            impedance.R4 = z1.R4 / scalar;
            impedance.R5 = z1.R5 / scalar;
            impedance.R6 = z1.R6 / scalar;

            impedance.X1 = z1.X1 / scalar;
            impedance.X2 = z1.X2 / scalar;
            impedance.X3 = z1.X3 / scalar;
            impedance.X4 = z1.X4 / scalar;
            impedance.X5 = z1.X5 / scalar;
            impedance.X6 = z1.X6 / scalar;

            impedance.G1 = z1.G1 / scalar;
            impedance.G2 = z1.G2 / scalar;
            impedance.G3 = z1.G3 / scalar;
            impedance.G4 = z1.G4 / scalar;
            impedance.G5 = z1.G5 / scalar;
            impedance.G6 = z1.G6 / scalar;

            impedance.B1 = z1.B1 / scalar;
            impedance.B2 = z1.B2 / scalar;
            impedance.B3 = z1.B3 / scalar;
            impedance.B4 = z1.B4 / scalar;
            impedance.B5 = z1.B5 / scalar;
            impedance.B6 = z1.B6 / scalar;

            return impedance;
        }

        #endregion

    }
}