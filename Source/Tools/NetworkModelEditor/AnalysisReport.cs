using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkModelEditor
{
    [Serializable()]
    public class AnalysisReport
    {
        #region [ Private Members ]

        private AnalysisReportType m_type;
        private string m_path;

        #endregion

        #region [ Public Properties ]

        [XmlAttribute("Type")]
        public AnalysisReportType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        [XmlAttribute("Path")]
        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }

        #endregion

        #region [ Constructor ]

        public AnalysisReport()
        {
            m_type = AnalysisReportType.ObservedBuses;
        }

        #endregion

    }
}
