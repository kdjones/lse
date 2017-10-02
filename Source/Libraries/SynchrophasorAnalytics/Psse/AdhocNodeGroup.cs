using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Psse
{
    public class AdhocBusGroup
    {
        private List<Bus> m_buses;

        public List<Bus> Buses
        {
            get
            {
                return m_buses;
            }
            set
            {
                m_buses = value;
            }
        }

        public AdhocBusGroup(List<Bus> buses)
        {
            m_buses = buses;
        }

        public static AdhocBusGroup Merge(AdhocBusGroup group1, AdhocBusGroup group2) 
        {
            List<Bus> buses = group1.Buses;
            
            foreach (Bus bus in group2.Buses)
            {
                if (!buses.Contains(bus))
                {

                    buses.Add(bus);
                }
            }
            
            return new AdhocBusGroup(buses);
        }

        public static bool CheckOverlap(AdhocBusGroup group1, AdhocBusGroup group2)
        {
            foreach (Bus bus in group1.Buses)
            {
                if (group2.Buses.Contains(bus))
                {
                    return true;
                }
            }
            return false;
        }

        public string ShortString()
        {
            string shortString = "";

            foreach (Bus bus in m_buses)
            {
                shortString += bus.ShortString() + ",";
            }

            return shortString.Trim(',');
        }

        public bool Contains(int busNumber)
        {
            foreach (Bus bus in m_buses)
            {
                if (bus.Number == busNumber)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
