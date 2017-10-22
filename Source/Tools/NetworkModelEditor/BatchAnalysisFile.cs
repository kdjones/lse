using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace NetworkModelEditor
{
    [Serializable()]
    public class BatchAnalysisFile
    {
        #region [ Private Members ]

        private List<AnalysisRun> m_analyses;

        #endregion

        #region [ Public Properties ]

        [XmlArray("LseAnalyses")]
        public List<AnalysisRun> Analyses
        {
            get
            {
                return m_analyses;
            }
            set
            {
                m_analyses = value;
            }
        }

        #endregion

        #region [ Constructor ]

        public BatchAnalysisFile()
        {
            m_analyses = new List<AnalysisRun>();
        }

        #endregion

        public void SerializeToXml(string pathName)
        {
            try
            {
                // Create an XmlSerializer with the type of Network
                XmlSerializer serializer = new XmlSerializer(typeof(BatchAnalysisFile));

                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(pathName);

                // Serialize this instance of NetworkMeasurements
                serializer.Serialize(writer, this);

                // Close the connection
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the Batch Analysis File: " + exception.ToString());
            }
        }

        public static BatchAnalysisFile DeserializeFromXml(string pathName)
        {
            try
            {
                // Create an empy NetworkMeasurements object reference.
                BatchAnalysisFile batch = null;

                // Create an XmlSerializer with the type of NetworkMeasurements.
                XmlSerializer deserializer = new XmlSerializer(typeof(BatchAnalysisFile));

                // Read the data in from the file.
                StreamReader reader = new StreamReader(pathName);

                // Cast the deserialized data as a NetworkMeasurements object.
                batch = (BatchAnalysisFile)deserializer.Deserialize(reader);

                // Close the connection.
                reader.Close();

                return batch;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the Batch Analysis File: " + exception.ToString());
            }
        }

    }
}
