//******************************************************************************************************
//  SequencedMeasurementSnapshotPathSet.cs
//
//  Copyright © 2014, Kevin D. Jones.  All Rights Reserved.
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
//  08/09/2014 - Kevin D. Jones
//      Created original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Testing
{
    /// <summary>
    /// A sequence of directory paths mapping to a sequence of measurement snapshots
    /// </summary>
    [Serializable()]
    public class SequencedMeasurementSnapshotPathSet
    {
        #region [ Private Members ]

        private List<MeasurementSnapshotPath> m_measurementSnapshotPaths;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A list of path names pointing towards measurement snapshot files
        /// </summary>
        [XmlArray("MeasurementSnapshotPaths")]
        public List<MeasurementSnapshotPath> MeasurementSnapshotPaths
        {
            get
            {
                return m_measurementSnapshotPaths;
            }
            set
            {
                m_measurementSnapshotPaths = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Designated constructor
        /// </summary>
        public SequencedMeasurementSnapshotPathSet()
            :this(new List<MeasurementSnapshotPath>())
        {

        }

        /// <summary>
        /// Constructor for <see cref="LinearStateEstimator.Testing.SequencedMeasurementSnapshotPathSet"/> class.
        /// </summary>
        /// <param name="measurementSnapshotPaths">A list of path names pointing towards measurement snapshot files</param>
        public SequencedMeasurementSnapshotPathSet(List<MeasurementSnapshotPath> measurementSnapshotPaths)
        {
            m_measurementSnapshotPaths = measurementSnapshotPaths;
        }

        #endregion

        #region [ Xml Serialization/Deserialization Methods ]

        /// <summary>
        /// Creates a new <see cref="LinearStateEstimator.Testing.SequencedMeasurementSnapshotPathSet"/> by deserializing the configuration file from the specified location.
        /// </summary>
        /// <param name="pathName">The location of the file including the file name.</param>
        /// <returns>A new <see cref="LinearStateEstimator.Testing.SequencedMeasurementSnapshotPathSet"/> based on the configuration file.</returns>
        public static SequencedMeasurementSnapshotPathSet DeserializeFromXml(string pathName)
        {
            try
            {
                // Create an empy SequencedMeasurementSnapshotSet object reference.
                SequencedMeasurementSnapshotPathSet sequencedMeasurementSnapshotSet = null;

                // Create an XmlSerializer with the type of SequencedMeasurementSnapshotSet.
                XmlSerializer deserializer = new XmlSerializer(typeof(SequencedMeasurementSnapshotPathSet));

                // Read the data in from the file.
                StreamReader reader = new StreamReader(pathName);

                // Cast the deserialized data as a SequencedMeasurementSnapshotSet object.
                sequencedMeasurementSnapshotSet = (SequencedMeasurementSnapshotPathSet)deserializer.Deserialize(reader);

                // Close the connection.
                reader.Close();

                return sequencedMeasurementSnapshotSet;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the Sequenced Measurement Snapshot Set from the Configuration File: " + exception.ToString());
            }
        }

        /// <summary>
        /// Serialized the <see cref="LinearStateEstimator.Testing.SequencedMeasurementSnapshotPathSet"/> to the specified file.
        /// </summary>
        /// <param name="pathName">The directory name included the file name of the desired location for Xml Serialization.</param>
        public void SerializeToXml(string pathName)
        {
            try
            {
                // Create an XmlSerializer with the type of SequencedMeasurementSnapshotSet
                XmlSerializer serializer = new XmlSerializer(typeof(SequencedMeasurementSnapshotPathSet));

                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(pathName);

                // Serialize this instance of NetworkModel
                serializer.Serialize(writer, this);

                // Close the connection
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the Sequenced Measurement Snapshot Set to the Configuration File: " + exception.ToString());
            }
        }

        #endregion
    }
}
