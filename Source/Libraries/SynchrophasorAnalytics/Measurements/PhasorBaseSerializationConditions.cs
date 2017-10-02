//******************************************************************************************************
//  PhasorBaseSerializationConditions.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Measurements
{
    public partial class PhasorBase
    {
        #region [ Private Members ]

        private bool m_shouldSerializeData;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A flag for the XmlSerializer that indicates whether to serialize the raw data as well as the configuration. Set 'true' to include data in XmlSerialization. Set 'false' to just serialize the configuration.
        /// </summary>
        [XmlIgnore()]
        public bool ShouldSerializeData
        {
            get 
            { 
                return m_shouldSerializeData; 
            }
            set 
            { 
                m_shouldSerializeData = value; 
            }
        }

        #endregion

        #region [ XML ShouldSerializePropertyName() Methods ]

        /// <summary>
        /// A conditional serialization method for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> property.</returns>
        public bool ShouldSerializeMagnitude()
        {
            return m_shouldSerializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> property.</returns>
        public bool ShouldSerializeAngleInDegrees()
        {
            return m_shouldSerializeData;
        }

        #endregion
    }
}
