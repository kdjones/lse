//******************************************************************************************************
//  StatusWordSerializeConditions.cs
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
    [Serializable()]
    public partial class StatusWord
    {
        #region [ Private Members ]

        private bool m_serializeData;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A flag for the XmlSerializer that indicates whether to serialize the raw data as well as the configuration.
        /// Set <b>true</b> to include data in XmlSerialization. Set <b>false</b> to just serialize the configuration.
        /// </summary>
        [XmlIgnore]
        public bool ShouldSerializeData
        {
            get
            {
                return m_serializeData;
            }
            set
            {
                m_serializeData = value;
            }
        }

        #endregion

        #region [ XML ShouldSerializePropertyName() Methods ]

        /// <summary>
        /// A conditional serialization method for the <see cref="TriggerReason_0"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="TriggerReason_0"/> property.</returns>
        public bool ShouldSerializeTriggerReason_0()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="TriggerReason_1"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="TriggerReason_1"/> property.</returns>
        public bool ShouldSerializeTriggerReason_1()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="TriggerReason_2"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="TriggerReason_2"/> property.</returns>
        public bool ShouldSerializeTriggerReason_2()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="TriggerReason_3"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="TriggerReason_3"/> property.</returns>
        public bool ShouldSerializeTriggerReason_3()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="UnlockedTimePeriod_0"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="UnlockedTimePeriod_0"/> property.</returns>
        public bool ShouldSerializeUnlockedTimePeriod_0()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="UnlockedTimePeriod_1"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="UnlockedTimePeriod_1"/> property.</returns>
        public bool ShouldSerializeUnlockedTimePeriod_1()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="Security_0"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="Security_0"/> property.</returns>
        public bool ShouldSerializeSecurity_0()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="Security_1"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="Security_1"/> property.</returns>
        public bool ShouldSerializeSecurity_1()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="Security_2"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="Security_2"/> property.</returns>
        public bool ShouldSerializeSecurity_2()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="Security_3"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="Security_3"/> property.</returns>
        public bool ShouldSerializeSecurity_3()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="ConfigurationChangedRecently"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="ConfigurationChangedRecently"/> property.</returns>
        public bool ShouldSerializeConfigurationChangedRecently()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="PMUTriggerDetected"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="PMUTriggerDetected"/> property.</returns>
        public bool ShouldSerializePMUTriggerDetected()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="DataSorting"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="DataSorting"/> property.</returns>
        public bool ShouldSerializeDataSorting()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="PMUSync"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="PMUSync"/> property.</returns>
        public bool ShouldSerializePMUSync()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="PMUError"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="PMUError"/> property.</returns>
        public bool ShouldSerializePMUError()
        {
            return m_serializeData;
        }

        /// <summary>
        /// A conditional serialization method for the <see cref="DataValid"/> property.
        /// </summary>
        /// <returns>A flag representing whether to serialize the <see cref="DataValid"/> property.</returns>
        public bool ShouldSerializeDataValid()
        {
            return m_serializeData;
        }

        #endregion
    }
}
