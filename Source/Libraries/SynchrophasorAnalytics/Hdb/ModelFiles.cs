using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Hdb
{
    [Serializable()]
    public class ModelFiles
    {
        private string m_areaFile;
        private string m_circuitBreakerFile;
        private string m_circuitBreakerExtensionFile;
        private string m_companyFile;
        private string m_divisionFile;
        private string m_lineSegmentFile;
        private string m_lineSegmentExtensionFile;
        private string m_nodeFile;
        private string m_nodeExtensionFile;
        private string m_shuntFile;
        private string m_shuntExtensionFile;
        private string m_stationFile;
        private string m_transformerFile;
        private string m_transformerExtensionFile;
        private string m_parentTransformerFile;
        private string m_transformerTapFile;
        private string m_transmissionLineFile;

        [XmlElement("area")]
        public string AreaFile
        {
            get
            {
                return m_areaFile;
            }
            set
            {
                m_areaFile = value;
            }
        }

        [XmlElement("cb")]
        public string CircuitBreakerFile
        {
            get
            {
                return m_circuitBreakerFile;
            }
            set
            {
                m_circuitBreakerFile = value;
            }
        }

        [XmlElement("cb_extension")]
        public string CircuitBreakerExtensionFile
        {
            get
            {
                return m_circuitBreakerExtensionFile;
            }
            set
            {
                m_circuitBreakerExtensionFile = value;
            }
        }

        [XmlElement("co")]
        public string CompanyFile
        {
            get
            {
                return m_companyFile;
            }
            set
            {
                m_companyFile = value;
            }
        }

        [XmlElement("dv")]
        public string DivisionFile
        {
            get
            {
                return m_divisionFile;
            }
            set
            {
                m_divisionFile = value;
            }
        }

        [XmlElement("ln")]
        public string LineSegmentFile
        {
            get
            {
                return m_lineSegmentFile;
            }
            set
            {
                m_lineSegmentFile = value;
            }
        }

        [XmlElement("ln_extension")]
        public string LineSegmentExtensionFile
        {
            get
            {
                return m_lineSegmentExtensionFile;
            }
            set
            {
                m_lineSegmentExtensionFile = value;
            }
        }

        [XmlElement("nd")]
        public string NodeFile
        {
            get
            {
                return m_nodeFile;
            }
            set
            {
                m_nodeFile = value;
            }
        }

        [XmlElement("nd_extension")]
        public string NodeExtensionFile
        {
            get
            {
                return m_nodeExtensionFile;
            }
            set
            {
                m_nodeExtensionFile = value;
            }
        }

        [XmlElement("cp")]
        public string ShuntFile
        {
            get
            {
                return m_shuntFile;
            }
            set
            {
                m_shuntFile = value;
            }
        }

        [XmlElement("cp_extension")]
        public string ShuntExtensionFile
        {
            get
            {
                return m_shuntExtensionFile;
            }
            set
            {
                m_shuntExtensionFile = value;
            }
        }

        [XmlElement("st")]
        public string StationFile
        {
            get
            {
                return m_stationFile;
            }
            set
            {
                m_stationFile = value;
            }
        }

        [XmlElement("xf")]
        public string TransformerFile
        {
            get
            {
                return m_transformerFile;
            }
            set
            {
                m_transformerFile = value;
            }
        }

        [XmlElement("xf_extension")]
        public string TransformerExtensionFile
        {
            get
            {
                return m_transformerExtensionFile;
            }
            set
            {
                m_transformerExtensionFile = value;
            }
        }

        [XmlElement("xfmr")]
        public string ParentTransformerFile
        {
            get
            {
                return m_parentTransformerFile;
            }
            set
            {
                m_parentTransformerFile = value;
            }
        }

        [XmlElement("tapty")]
        public string TransformerTapFile
        {
            get
            {
                return m_transformerTapFile;
            }
            set
            {
                m_transformerTapFile = value;
            }
        }

        [XmlElement("line")]
        public string TransmissionLineFile
        {
            get
            {
                return m_transmissionLineFile;
            }
            set
            {
                m_transmissionLineFile = value;
            }
        }

        public ModelFiles()
        {
        }
        
        public override string ToString()
        {
            string modelFileString = "";
            modelFileString += $"                     Area: {AreaFile}\n";
            modelFileString += $"          Circuit Breaker: {CircuitBreakerFile}\n";
            modelFileString += $"Circuit Breaker Extension: {CircuitBreakerExtensionFile}\n";
            modelFileString += $"                  Company: {CompanyFile}\n";
            modelFileString += $"                 Division: {DivisionFile}\n";
            modelFileString += $"             Line Segment: {LineSegmentFile}\n";
            modelFileString += $"   Line Segment Extension: {LineSegmentExtensionFile}\n";
            modelFileString += $"                     Node: {NodeFile}\n";
            modelFileString += $"           Node Extension: {NodeExtensionFile}\n";
            modelFileString += $"                    Shunt: {ShuntFile}\n";
            modelFileString += $"          Shunt Extension: {ShuntExtensionFile}\n";
            modelFileString += $"                  Station: {StationFile}\n";
            modelFileString += $"              Transformer: {TransformerFile}\n";
            modelFileString += $"    Transformer Extension: {TransformerExtensionFile}\n";
            modelFileString += $"       Parent Transformer: {TransformerFile}\n";
            modelFileString += $"                      Tap: {TransformerTapFile}\n";
            modelFileString += $"        Transmission Line: {TransmissionLineFile}\n";
            return modelFileString;
        }

        #region [ Xml Serialization/Deserialization methods ]

        public static ModelFiles DeserializeFromXml(string pathName)
        {
            try
            {
                // Create an empy ModelFiles object reference.
                ModelFiles modelFiles = null;

                // Create an XmlSerializer with the type of ModelFiles.
                XmlSerializer deserializer = new XmlSerializer(typeof(ModelFiles));

                // Read the data in from the file.
                StreamReader reader = new StreamReader(pathName);

                // Cast the deserialized data as a ModelFiles object.
                modelFiles = (ModelFiles)deserializer.Deserialize(reader);

                // Close the connection.
                reader.Close();

                return modelFiles;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the ModelFiles from the Configuration File: " + exception.ToString());
            }
        }

        public void SerializeToXml(string pathName)
        {
            try
            {
                // Create an XmlSerializer with the type of ModelFiles
                XmlSerializer serializer = new XmlSerializer(typeof(ModelFiles));

                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(pathName);

                // Serialize this instance of ModelFiles
                serializer.Serialize(writer, this);

                // Close the connection
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the ModelFiles to the Configuration File: " + exception.ToString());
            }
        }

        #endregion

    }
}
