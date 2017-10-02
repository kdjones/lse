using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Psse
{
    public class Branch
    {
        #region [ Private Members ]

        private int m_fromBusNumber;
        private int m_toBusNumber;
        private string m_circuitId;
        private double m_resistance;
        private double m_reactance;
        private double m_charging;
        private double m_ratingA;
        private double m_ratingB;
        private double m_ratingC;
        private double m_fromSideShuntConductance;
        private double m_fromSideShuntSusceptance;
        private double m_toSideShuntConductance;
        private double m_toSideShuntSusceptance;
        private int m_status;
        private int m_meteredEndBus;
        private double m_length;
        private int m_firstOwner;
        private double m_firstOwnerFractionalOwnership;
        private int m_secondOwner;
        private double m_secondOwnerFractionalOwnership;
        private int m_thirdOwner;
        private double m_thirdOwnerFractionalOwnership;
        private int m_fourthOwner;
        private double m_fourthOwnerFractionalOwnership;

        #endregion

        #region [ Public Properties ]

        public int FromBusNumber
        {
            get
            {
                return m_fromBusNumber;
            }
            set
            {
                m_fromBusNumber = value;
            }
        }

        public int ToBusNumber
        {
            get
            {
                return m_toBusNumber;
            }
            set
            {
                m_toBusNumber = value;
            }
        }

        public string CircuitId
        {
            get
            {
                return m_circuitId;
            }
            set
            {
                m_circuitId = value;
            }
        }

        public double Resistance
        {
            get
            {
                return m_resistance;
            }
            set
            {
                m_resistance = value;
            }
        }

        public double Reactance
        {
            get
            {
                return m_reactance;
            }
            set
            {
                m_reactance = value;
            }
        }

        public double Charging
        {
            get
            {
                return m_charging;
            }
            set
            {
                m_charging = value;
            }
        }

        public double RatingA
        {
            get
            {
                return m_ratingA;
            }
            set
            {
                m_ratingA = value;
            }
        }

        public double RatingB
        {
            get
            {
                return m_ratingB;
            }
            set
            {
                m_ratingB = value;
            }
        }

        public double RatingC
        {
            get
            {
                return m_ratingC;
            }
            set
            {
                m_ratingC = value;
            }
        }

        public double FromSideShuntConductance
        {
            get
            {
                return m_fromSideShuntConductance;
            }
            set
            {
                m_fromSideShuntConductance = value;
            }
        }

        public double FromSideShuntSusceptance
        {
            get
            {
                return m_fromSideShuntSusceptance;
            }
            set
            {
                m_fromSideShuntSusceptance = value;
            }
        }

        public double ToSideShuntConductance
        {
            get
            {
                return m_toSideShuntConductance;
            }
            set
            {
                m_toSideShuntConductance = value;
            }
        }

        public double ToSideShuntSusceptance
        {
            get
            {
                return m_toSideShuntSusceptance;
            }
            set
            {
                m_toSideShuntSusceptance = value;
            }
        }

        public int Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        public int MeteredEndBus
        {
            get
            {
                return m_meteredEndBus;
            }
            set
            {
                m_meteredEndBus = value;
            }
        }

        public double Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        public int FirstOwner
        {
            get
            {
                return m_firstOwner;
            }
            set
            {
                m_firstOwner = value;
            }
        }

        public double FirstOwnerFractionalOwnership
        {
            get
            {
                return m_firstOwnerFractionalOwnership;
            }
            set
            {
                m_firstOwnerFractionalOwnership = value;
            }
        }

        public int SecondOwner
        {
            get
            {
                return m_secondOwner;
            }
            set
            {
                m_secondOwner = value;
            }
        }

        public double SecondOwnerFractionalOwnership
        {
            get
            {
                return m_secondOwnerFractionalOwnership;
            }
            set
            {
                m_secondOwnerFractionalOwnership = value;
            }
        }

        public int ThirdOwner
        {
            get
            {
                return m_thirdOwner;
            }
            set
            {
                m_thirdOwner = value;
            }
        }

        public double ThirdOwnerFractionalOwnership
        {
            get
            {
                return m_thirdOwnerFractionalOwnership;
            }
            set
            {
                m_thirdOwnerFractionalOwnership = value;
            }
        }

        public int FourthOwner
        {
            get
            {
                return m_fourthOwner;
            }
            set
            {
                m_fourthOwner = value;
            }
        }

        public double FourthOwnerFractionalOwnership
        {
            get
            {
                return m_fourthOwnerFractionalOwnership;
            }
            set
            {
                m_fourthOwnerFractionalOwnership = value;
            }
        }

        public bool IsBreaker
        {
            get
            {
                if (m_circuitId.Contains("@"))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsSwitch
        {
            get
            {
                if (m_circuitId.Contains("*"))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsLineSegment
        {
            get
            {
                if (!IsBreaker && !IsSwitch)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// 
        /// </summary>
        public Branch()
        {
            // do nothing yet
        }

        #endregion

        #region [ Static Methods ]

        public static Branch Parse(string line)
        {
            Branch branch = new Branch();
            string[] data = line.Trim('\n').Split(',');
            branch.FromBusNumber = Convert.ToInt32(data[0]);
            branch.ToBusNumber = Convert.ToInt32(data[1]);
            branch.CircuitId = data[2].Trim('\'');
            branch.Resistance = Convert.ToDouble(data[3]);
            branch.Reactance = Convert.ToDouble(data[4]);
            branch.Charging = Convert.ToDouble(data[5]);
            if (data[6] == "********")
            {
                branch.RatingA = 0.0;
            }
            else
            {
                branch.RatingA = Convert.ToDouble(data[6]);
            }
            if (data[7] == "********")
            {
                branch.RatingA = 0.0;
            }
            else
            {
                branch.RatingA = Convert.ToDouble(data[7]);
            }
            if (data[8] == "********")
            {
                branch.RatingA = 0.0;
            }
            else
            {
                branch.RatingA = Convert.ToDouble(data[8]);
            }
            branch.FromSideShuntConductance = Convert.ToDouble(data[9]);
            branch.FromSideShuntSusceptance = Convert.ToDouble(data[10]);
            branch.ToSideShuntConductance = Convert.ToDouble(data[11]);
            branch.ToSideShuntSusceptance = Convert.ToDouble(data[12]);
            branch.Status = Convert.ToInt32(data[13]);
            branch.MeteredEndBus = Convert.ToInt32(data[14]);
            branch.Length = Convert.ToDouble(data[15]);
            branch.FirstOwner = Convert.ToInt32(data[16]);
            branch.FirstOwnerFractionalOwnership = Convert.ToDouble(data[17]);

            if (data.Count() >= 20)
            {
                branch.SecondOwner = Convert.ToInt32(data[18]);
                branch.SecondOwnerFractionalOwnership = Convert.ToDouble(data[19]);
                branch.ThirdOwner = 0;
                branch.ThirdOwnerFractionalOwnership = 0.0;
                branch.FourthOwner = 0;
                branch.FourthOwnerFractionalOwnership = 0.0;
            }
            else if (data.Count() >= 22)
            {
                branch.ThirdOwner = Convert.ToInt32(data[20]);
                branch.ThirdOwnerFractionalOwnership = Convert.ToDouble(data[21]);
                branch.FourthOwner = 0;
                branch.FourthOwnerFractionalOwnership = 0.0;
            }
            else if (data.Count() >= 24)
            {
                branch.FourthOwner = Convert.ToInt32(data[22]);
                branch.FourthOwnerFractionalOwnership = Convert.ToDouble(data[23]);
            }
            return branch;
        }


        #endregion
    }
}
