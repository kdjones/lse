//******************************************************************************************************
//  MeasurementSnapshotPath.cs
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
    /// Contains a string value which represents the directory path of a measurement snapshot
    /// </summary>
    [Serializable()]
    public class MeasurementSnapshotPath
    {
        #region [ Private Members ]

        private string m_path;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The directory where the measurement snapshot is stored.
        /// </summary>
        [XmlAttribute("Value")]
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

        #region [ Constructors ]

        /// <summary>
        /// Designated constructor
        /// </summary>
        public MeasurementSnapshotPath()
        {
        }

        /// <summary>
        /// Constructor for the Measurement Snapshot Path class. Initializes with a defined path name.
        /// </summary>
        /// <param name="path">The directory where the measuremtn snapshot is stored.</param>
        public MeasurementSnapshotPath(string path)
        {
            m_path = path;
        }

        #endregion
    }
}
