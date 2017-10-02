//******************************************************************************************************
//  StatusWord.cs
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
//  07/01/2012 - Kevin D. Jones
//       Generated original version of source code.
//  06/01/2013 - Kevin D. Jones
//       Added XMl Serialization
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Measurements
{
    #region [ Enumerations ]

    /// <summary>
    /// Numerically select the bit inside of the Status Flag using this enumeration
    /// </summary>
    /// <seealso cref="StatusWord"/>
    [Serializable()]
    public enum StatusWordBit
    {    
        DataDiscarded,
        DataIsValid,
        SynchronizationIsValid,
        DataSortingType,
        DeviceError,
         
        /// <summary>
        /// <b>DataValid</b> bit. The most significant bit of the status word.
        /// </summary>
        [XmlEnum("DataValid")]
        DataValid,

        /// <summary>
        /// The <b>PMUError</b> bit
        /// </summary>
        [XmlEnum("PMUError")]
        PMUError,

        /// <summary>
        /// The <b>PMUSync</b> bit
        /// </summary>
        [XmlEnum("PMUSync")]
        PMUSync,

        /// <summary>
        /// The <b>DataSorting</b> bit
        /// </summary>
        [XmlEnum("DataSorting")]
        DataSorting,

        /// <summary>
        /// The <b>PMUTriggerDetected</b> bit
        /// </summary>
        [XmlEnum("PMUTriggerDetected")]
        PMUTriggerDetected,

        /// <summary>
        /// The <b>ConfigurationChangedRecently</b> bit
        /// </summary>
        [XmlEnum("ConfigurationChangedRecently")]
        ConfigurationChangedRecently,

        /// <summary>
        /// The <b>Security_3</b> bit. The most significant bit reserved for security purposes. 
        /// </summary>
        [XmlEnum("SecurityThree")]
        SecurityThree,

        /// <summary>
        /// The <b>Security_2</b> bit. The second most significant bit reserved for security purposes.
        /// </summary>
        [XmlEnum("SecurityTwo")]
        SecurityTwo,

        /// <summary>
        /// The <b>Security_1</b> bit. The second least significant bit reserved for security purposes.
        /// </summary>
        [XmlEnum("SecurityOne")]
        SecurityOne,
        
        /// <summary>
        /// The <b>Security_0</b> bit. The least significant bit reserved for security purposes.
        /// </summary>
        [XmlEnum("SecurityZero")]
        SecurityZero,

        /// <summary>
        /// The <b>UnlockedTimePeriod_1</b> bit. The most significant bit reserved for determining the time since loss of synchronization.
        /// </summary>
        [XmlEnum("UnlockedTimePeriodOne")]
        UnlockedTimePeriodOne,

        /// <summary>
        /// The <b>UnlockedTimePeriod_0</b> bit. The least significant bit reserved for determining the time since loss of synchronization.
        /// </summary>
        [XmlEnum("UnlockedTimePeriodZero")]
        UnlockedTimePeriodZero,

        /// <summary>
        /// The <b>TriggerReason_3</b> bit. The most significant user-definable bit.
        /// </summary>
        [XmlEnum("TriggerReasonThree")]
        TriggerReasonThree,

        /// <summary>
        /// The <b>TriggerReason_2</b> bit. The second most significant user-definable bit.
        /// </summary>
        [XmlEnum("TriggerReasonTwo")]
        TriggerReasonTwo,

        /// <summary>
        /// The <b>TriggerReason_1</b> bit. The second least significant user-definable bit.
        /// </summary>
        [XmlEnum("TriggerReasonOne")]
        TriggerReasonOne,

        /// <summary>
        /// The <b>TriggerReason_0</b> bit. The least significant user-definable bit.
        /// </summary>
        [XmlEnum("TriggerReasonZero")]
        TriggerReasonZero   
    }

    #endregion
}
