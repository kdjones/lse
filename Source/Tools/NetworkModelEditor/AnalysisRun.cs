using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkModelEditor
{
    [Serializable()]
    public class AnalysisRun
    {
        #region [ Private Members ]

        private string m_modelFile;
        private string m_sampleFile;
        private List<AnalysisReport> m_reports;

        #endregion

        #region [ Public Properties ]

        [XmlElement("Model")]
        public string ModelFile
        {
            get
            {
                return m_modelFile;
            }
            set
            {
                m_modelFile = value;
            }
        }

        [XmlElement("MeasurementSample")]
        public string SampleFile
        {
            get
            {
                return m_sampleFile;
            }
            set
            {
                m_sampleFile = value;
            }
        }

        [XmlArray("Reports")]
        public List<AnalysisReport> Reports
        {
            get
            {
                return m_reports;
            }
            set
            {
                m_reports = value;
            }
        }

        #endregion

        #region [ Constructor ]

        public AnalysisRun()
        {
            m_reports = new List<AnalysisReport>();
        }

        #endregion

    }
}
