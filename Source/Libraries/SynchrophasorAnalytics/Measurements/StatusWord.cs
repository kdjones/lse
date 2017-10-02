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
//       Added INetworkDescribable interface and XMl Serialization
//  07/20/2013 - Kevin D. Jones
//       Added m_binaryValue member to add Getter method to BinaryValue property.
//  07/07/2014 - Kevin D. Jones
//       Added Guid
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// The <see cref="LinearStateEstimator.Measurements.StatusWord"/> class represents a C37.118 Status word from a single PMU device
    /// </summary>
    /// <remarks>This class contains a property for each flag inside of the C37.118 status word.</remarks>
    /// <seealso cref="StatusWordBit"/>
    public partial class StatusWord : INetworkDescribable, IClearable
    {
        #region [ Constants ]

        /// <summary>
        /// The maximum value that the  status word can have. It is equivalent to 111111111111111111111 in binary.
        /// </summary>
        private static int MAXIMUM_VALUE = 2097151;

        private static int DEFAULT_INTERNAL_ID = 0;
        private static int DEFAULT_NUMBER = 0;
        private static string DEFAULT_DESCRIPTION = "Undefined";
        private static string DEFAULT_MEASUREMENT_KEY = "Undefined";

        #endregion        
         
        #region [ Private Members ]

        private Guid m_uniqueId;
        private int m_internalID;
        private int m_number;
        private string m_description;
        private bool m_enabled;
        private bool m_triggerReasonZero;
        private bool m_triggerReasonOne;
        private bool m_triggerReasonTwo;
        private bool m_triggerReasonThree;
        private bool m_unlockedTimePeriodZero;
        private bool m_unlockedTimePeriodOne;
        private bool m_securityZero;
        private bool m_securityOne;
        private bool m_securityTwo;
        private bool m_securityThree;
        private bool m_configurationChangedRecently;
        private bool m_pmuTriggerDetected;
        private bool m_dataSorting;
        private bool m_pmuSync;
        private bool m_pmuError;
        private bool m_dataValid;
        private bool m_deviceError;
        private bool m_dataSortingType;
        private bool m_synchronizationIsValid;
        private bool m_dataIsValid;
        private bool m_dataDiscarded;
        private string m_inputMeasurementKey;
        private bool m_statusWordWasReported;
        private double m_binaryValue;
        private double m_previousBinaryValue;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A statistically unique identifier for the instance of the class.
        /// </summary>
        [XmlAttribute("Uid")]
        public Guid UniqueId
        {
            get
            {
                if (m_uniqueId == Guid.Empty)
                {
                    m_uniqueId = Guid.NewGuid();
                }
                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        /// <summary>
        /// The unique integer identifier for each instance of a <see cref="StatusWord"/>.
        /// </summary>
        [XmlAttribute("ID")]
        public int InternalID
        {
            get 
            { 
                return m_internalID; 
            }
            set 
            { 
                m_internalID = value; 
            }
        }

        /// <summary>
        /// A descriptive number for the <see cref="StatusWord"/>.
        /// </summary>
        [XmlAttribute("Number")]
        public int Number
        {
            get 
            { 
                return m_number; 
            }
            set 
            {
                m_number = value;
            }
        }

        /// <summary>
        /// A descriptive acronym for the <see cref="StatusWord"/>. Will always return 'STWD'.
        /// </summary>
        [XmlIgnore()]
        public string Acronym
        {
            get 
            { 
                return "STWD"; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// A descriptive name for the <see cref="StatusWord"/>. Will always return 'Status Word: + InputMeasurementKey'.
        /// </summary>
        [XmlIgnore()]
        public string Name
        {
            get 
            { 
                return "Status Word: " + m_inputMeasurementKey; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// A description of the <see cref="StatusWord"/>.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description
        {
            get 
            { 
                return m_description; 
            }
            set 
            { 
                m_description = value; 
            }
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        [XmlIgnore()]
        public string ElementType
        {
            get 
            { 
                return this.GetType().ToString(); 
            }
        }

        /// <summary>
        /// A flag that represents whether the measurement is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool IsEnabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        /// <summary>
        /// A flag that represents whether the <see cref="StatusWord"/> was reported for the most recent frame.
        /// </summary>
        [XmlIgnore()]
        public bool StatusWordWasReported
        {
            get
            {
                return m_statusWordWasReported;
            }
            set
            {
                m_statusWordWasReported = value;
            }
        }

        /// <summary>
        /// The raw value from the C37.118 stream
        /// </summary>
        [XmlIgnore()]
        public double BinaryValue
        {
            get
            {
                return m_binaryValue;
            }
            set
            {
                m_statusWordWasReported = true;
                if (value < MAXIMUM_VALUE && value >= 0)
                {
                    m_previousBinaryValue = m_binaryValue;
                    m_binaryValue = value;
                    ParseStatusFlag(Convert.ToInt32(value)); 
                }
                else 
                { 
                    ParseStatusFlag(MAXIMUM_VALUE); 
                }
            }
        }

        /// <summary>
        /// Indicates whether the latest measurement is different than the previous once, indicating a change in the structure of the network representation.
        /// </summary>
        [XmlIgnore()]
        public bool HasReportedChange
        {
            get
            {
                return (m_binaryValue != m_previousBinaryValue);
            }
        }

        /// <summary>
        /// Input measurement key
        /// </summary>
        [XmlAttribute("Key")]
        public string Key
        {
            get
            {
                return m_inputMeasurementKey;
            }
            set
            {
                m_inputMeasurementKey = value;
            }
        }

        /// <summary>
        ///  TriggerReason_0 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("TR0")]
        public bool TriggerReason_0
        {
            get 
            { 
                return m_triggerReasonZero; 
            }
            set 
            { 
                m_triggerReasonZero = value; 
            }
        }

        /// <summary>
        ///  TriggerReason_1 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("TR1")]
        public bool TriggerReason_1
        {
            get 
            { 
                return m_triggerReasonOne; 
            }
            set 
            { 
                m_triggerReasonOne = value; 
            }
        }

        /// <summary>
        ///  TriggerReason_2 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("TR2")]
        public bool TriggerReason_2
        {
            get 
            { 
                return m_triggerReasonTwo; 
            }
            set 
            { 
                m_triggerReasonTwo = value; 
            }
        }

        /// <summary>
        ///  TriggerReason_3 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("TR3")]
        public bool TriggerReason_3
        {
            get 
            { 
                return m_triggerReasonThree; 
            }
            set 
            { 
                m_triggerReasonThree = value; 
            }
        }

        /// <summary>
        ///  UnlockedTimePeriod_0 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("UTP0")]
        public bool UnlockedTimePeriod_0
        {
            get 
            { 
                return m_unlockedTimePeriodZero; 
            }
            set 
            { 
                m_unlockedTimePeriodZero = value; 
            }
        }

        /// <summary>
        ///  UnlockedTimePeriod_1 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("UTP1")]
        public bool UnlockedTimePeriod_1
        {
            get 
            { 
                return m_unlockedTimePeriodOne; 
            }
            set 
            { 
                m_unlockedTimePeriodOne = value; 
            }
        }

        /// <summary>
        ///  Security_0 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("S0")]
        public bool Security_0
        {
            get { return m_securityZero; }
            set { m_securityZero = value; }
        }

        /// <summary>
        ///  Security_1 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("S1")]
        public bool Security_1
        {
            get 
            { 
                return m_securityOne; 
            }
            set 
            { 
                m_securityOne = value; 
            }
        }

        /// <summary>
        ///  Security_2 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("S2")]
        public bool Security_2
        {
            get 
            { 
                return m_securityTwo; 
            }
            set 
            { 
                m_securityTwo = value; 
            }
        }

        /// <summary>
        ///  Security_3 bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("S3")]
        public bool Security_3
        {
            get 
            { 
                return m_securityThree; 
            }
            set 
            { 
                m_securityThree = value; 
            }
        }

        /// <summary>
        ///  ConfigurationChangedRecently bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("CCR")]
        public bool ConfigurationChangedRecently
        {
            get 
            { 
                return m_configurationChangedRecently; 
            }
            set 
            { 
                m_configurationChangedRecently = value; 
            }
        }

        /// <summary>
        ///  PMUTriggerDetected bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("PMUTD")]
        public bool PMUTriggerDetected
        {
            get 
            { 
                return m_pmuTriggerDetected; 
            }
            set 
            { 
                m_pmuTriggerDetected = value; 
            }
        }

        /// <summary>
        ///  Data Sorting bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("DS")]
        public bool DataSorting
        {
            get 
            { 
                return m_dataSorting; 
            }
            set 
            { 
                m_dataSorting = value; 
            }
        }

        /// <summary>
        /// PMUSync bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("PMUS")]
        public bool PMUSync
        {
            get 
            { 
                return m_pmuSync; 
            }
            set 
            { 
                m_pmuSync = value; 
            }
        }

        /// <summary>
        /// PMUError bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("PMUE")]
        public bool PMUError
        {
            get 
            { 
                return m_pmuError; 
            }
            set 
            { 
                m_pmuError = value; 
            }
        }

        /// <summary>
        /// DataValid bit in the C37.118 Status Word
        /// </summary>
        [XmlAttribute("DV")]
        public bool DataValid
        {
            get 
            { 
                return m_dataValid; 
            }
            set 
            { 
                m_dataValid = value; 
            }
        }

        [XmlAttribute("CDE")]
        public bool DeviceError
        {
            get
            {
                return m_deviceError;
            }
            set
            {
                m_deviceError = value;
            }

        }

        [XmlAttribute("CDST")]
        public bool DataSortingType
        {
            get
            {
                return m_dataSortingType;
            }
            set
            {
                m_dataSortingType = value;
            }
        }

        [XmlAttribute("CSIV")]
        public bool SynchronizationIsValid
        {
            get
            {
                return m_synchronizationIsValid;
            }
            set
            {
                m_synchronizationIsValid = value;
            }
        }

        [XmlAttribute("CDIV")]
        public bool DataIsValid
        {
            get
            {
                return m_dataIsValid;
            }
            set
            {
                m_dataIsValid = value;
            }
        }

        [XmlAttribute("CDD")]
        public bool DataDiscarded
        {
            get
            {
                return m_dataDiscarded;
            }
            set
            {
                m_dataDiscarded = value;
            }
        }
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values. Initializes a new instance of the <see cref="LinearStateEstimator.Measurements.StatusWord"/> class with all properties assigned a default value of false.
        /// </summary>
        public StatusWord()
            : this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_DESCRIPTION, DEFAULT_MEASUREMENT_KEY)
        {
        }

        /// <summary>
        /// A constructor which initializes a new instance of the <see cref="LinearStateEstimator.Measurements.StatusWord"/> class with all properties required by the <see cref="INetworkDescribable"/> interface and its <see cref="LinearStateEstimator.Measurements.StatusWord.Key"/>.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="inputMeasurementKey">The input measurement key for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        public StatusWord(int internalID, int number, string description, string inputMeasurementKey)
            :this(internalID, number, description, inputMeasurementKey, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StatusFlag class with the parameters required by the <see cref="INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="inputMeasurementKey">The input measurement key for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="value">A double containing the value of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        public StatusWord(int internalID, int number, string description, string inputMeasurementKey, double value)
            : this(internalID, number, description, inputMeasurementKey, Convert.ToInt32(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the StatusFlag class with the parameters required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and an actual value.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="inputMeasurementKey">The input measurement key for the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        /// <param name="value">A integer containing the value of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</param>
        public StatusWord(int internalID, int number, string description, string inputMeasurementKey, Int32 value)
        {
            m_internalID = internalID;
            m_number = number;
            m_description = description;
            m_inputMeasurementKey = inputMeasurementKey;

            if (value < MAXIMUM_VALUE && value >= 0) { ParseStatusFlag(value); }
            else { ParseStatusFlag(MAXIMUM_VALUE); }
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Parses the <see cref="StatusWord"/> into boolean flags for each of the bits in the <see cref="StatusWord"/>
        /// </summary>
        /// <param name="value">The raw integer value of the <see cref="StatusWord"/></param>
        private void ParseStatusFlag(Int32 value)
        {
            string flags = AddPrefixOfZerosIfNeeded(Convert.ToString(value, 2));

                         m_triggerReasonZero = CheckBit(flags[(Int32)StatusWordBit.TriggerReasonZero]);
                          m_triggerReasonOne = CheckBit(flags[(Int32)StatusWordBit.TriggerReasonOne]);
                          m_triggerReasonTwo = CheckBit(flags[(Int32)StatusWordBit.TriggerReasonTwo]);
                        m_triggerReasonThree = CheckBit(flags[(Int32)StatusWordBit.TriggerReasonThree]);
                    m_unlockedTimePeriodZero = CheckBit(flags[(Int32)StatusWordBit.UnlockedTimePeriodZero]);
                     m_unlockedTimePeriodOne = CheckBit(flags[(Int32)StatusWordBit.UnlockedTimePeriodOne]);
                              m_securityZero = CheckBit(flags[(Int32)StatusWordBit.SecurityZero]);
                               m_securityOne = CheckBit(flags[(Int32)StatusWordBit.SecurityOne]);
                               m_securityTwo = CheckBit(flags[(Int32)StatusWordBit.SecurityTwo]);
                             m_securityThree = CheckBit(flags[(Int32)StatusWordBit.SecurityThree]);
              m_configurationChangedRecently = CheckBit(flags[(Int32)StatusWordBit.ConfigurationChangedRecently]);
                        m_pmuTriggerDetected = CheckBit(flags[(Int32)StatusWordBit.PMUTriggerDetected]);
                               m_dataSorting = CheckBit(flags[(Int32)StatusWordBit.DataSorting]);
                                   m_pmuSync = CheckBit(flags[(Int32)StatusWordBit.PMUSync]);
                                  m_pmuError = CheckBit(flags[(Int32)StatusWordBit.PMUError]);
                                 m_dataValid = CheckBit(flags[(Int32)StatusWordBit.DataValid]);
                               m_deviceError = CheckBit(flags[(Int32)StatusWordBit.DeviceError]);
                           m_dataSortingType = CheckBit(flags[(Int32)StatusWordBit.DataSortingType]);
                    m_synchronizationIsValid = CheckBit(flags[(Int32)StatusWordBit.SynchronizationIsValid]);
                               m_dataIsValid = CheckBit(flags[(Int32)StatusWordBit.DataIsValid]);
                             m_dataDiscarded = CheckBit(flags[(Int32)StatusWordBit.DataDiscarded]);
        }

        /// <summary>
        /// Checks the char at each of the bits in the StatusWord
        /// </summary>
        /// <param name="bit">The bit in question</param>
        /// <returns>A boolean flag representing whether the bit is 0 or 1.</returns>
        private bool CheckBit(char bit)
        {
            if (bit.Equals('0')) 
            { 
                return false; 
            }
            else if (bit.Equals('1')) 
            { 
                return true; 
            }
            else 
            { 
                return true; 
            }
        }

        /// <summary>
        /// Adds zeros to the beginning of a string representation of the 
        /// status word if the length of the status word is less than maximum
        /// </summary>
        /// <param name="flags">The raw string representation of the StatusWord</param>
        /// <returns>A string representation of the StatusWord with zeros as a prefix up to the maximum length.</returns>
        private string AddPrefixOfZerosIfNeeded(string flags)
        {
            // Add leading zeros to make it sixteen characters long
            string prefixOfZeros = "";
            for (int i = 0; i < (21 - flags.Length); i++) 
            { 
                prefixOfZeros += "0"; 
            }
            return (prefixOfZeros + flags);
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Returns the state of the specified bit. These values can also be accessed by their name with the class properties. This method would be used with the user is not sure of the bit name but knows its position in the string.
        /// </summary>
        /// <param name="bit">The desired bit specified using an enumeration. </param>
        /// <returns>A bool representing the state of the specified bit</returns>
        public bool GetFlagOfBit(StatusWordBit bit)
        {
            switch ((int)bit)
            {
                case 0: return m_deviceError;
                case 1: return m_dataSortingType;
                case 2: return m_synchronizationIsValid;
                case 3: return m_dataIsValid;
                case 4: return m_dataDiscarded;
                case 5: return m_dataValid;
                case 6: return m_pmuError;
                case 7: return m_pmuSync;
                case 8: return m_dataSorting;
                case 9: return m_pmuTriggerDetected;
                case 10: return m_configurationChangedRecently;
                case 11: return m_securityThree;
                case 12: return m_securityTwo;
                case 13: return m_securityOne;
                case 14: return m_securityZero;
                case 15: return m_unlockedTimePeriodOne;
                case 16: return m_unlockedTimePeriodZero;
                case 17: return m_triggerReasonThree;
                case 18: return m_triggerReasonTwo;
                case 19: return m_triggerReasonOne;
                case 20: return m_triggerReasonZero;
                default: return false;
            }
        }

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</returns>
        public override string ToString()
        {
            return "StatusWord," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_description + "," + m_enabled.ToString() + "," + m_inputMeasurementKey.ToString();
        }

        /// <summary>
        /// A descriptive string representation of the class instance specific to the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.
        /// </summary>
        /// <returns>A descriptive string representation of the class instance specific to the <see cref="LinearStateEstimator.Measurements.StatusWord"/>.</returns>
        public string ToStatusString()
        {
            string binaryString  = Convert.ToInt16(DataDiscarded).ToString() + " ";
                   binaryString += Convert.ToInt16(DataIsValid).ToString() + " ";
                   binaryString += Convert.ToInt16(SynchronizationIsValid).ToString() + " ";
                   binaryString += Convert.ToInt16(DataSortingType).ToString() + " ";
                   binaryString += Convert.ToInt16(DeviceError).ToString() + " ";
                   binaryString += Convert.ToInt16(m_dataValid).ToString() + " ";
                   binaryString += Convert.ToInt16(m_pmuError).ToString() + " ";
                   binaryString += Convert.ToInt16(m_pmuSync).ToString() + " ";
                   binaryString += Convert.ToInt16(m_dataSorting).ToString() + " ";
                   binaryString += Convert.ToInt16(m_pmuTriggerDetected).ToString() + " ";
                   binaryString += Convert.ToInt16(m_configurationChangedRecently).ToString() + " ";
                   binaryString += Convert.ToInt16(m_securityThree).ToString() + " ";
                   binaryString += Convert.ToInt16(m_securityTwo).ToString() + " ";
                   binaryString += Convert.ToInt16(m_securityOne).ToString() + " ";
                   binaryString += Convert.ToInt16(m_securityZero).ToString() + " ";
                   binaryString += Convert.ToInt16(m_unlockedTimePeriodOne).ToString() + " ";
                   binaryString += Convert.ToInt16(m_unlockedTimePeriodZero).ToString() + " ";
                   binaryString += Convert.ToInt16(m_triggerReasonThree).ToString() + " ";
                   binaryString += Convert.ToInt16(m_triggerReasonTwo).ToString() + " ";
                   binaryString += Convert.ToInt16(m_triggerReasonOne).ToString() + " ";
                   binaryString += Convert.ToInt16(m_triggerReasonZero).ToString();

            return "-STWD- ID: " + m_internalID.ToString() + " Value: " + binaryString;
        }

        /// <summary>
        /// A verbose descriptive string representation of the class instance.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the class instance.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Status Word --------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat($"      InternalID: {InternalID}{Environment.NewLine}");
            stringBuilder.AppendFormat($"          Number: {Number}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     Description: {Description}{Environment.NewLine}");
            stringBuilder.AppendFormat($"             Key: {Key}{Environment.NewLine}");
            stringBuilder.AppendFormat($"         Enabled: {IsEnabled}{Environment.NewLine}");
            stringBuilder.AppendFormat($"    Was Reported: {StatusWordWasReported}{Environment.NewLine}");
            stringBuilder.AppendFormat($"    Binary Value: {BinaryValue}{Environment.NewLine}");
            stringBuilder.AppendFormat($"  TriggerReason0: {TriggerReason_0}{Environment.NewLine}");
            stringBuilder.AppendFormat($"  TriggerReason1: {TriggerReason_1}{Environment.NewLine}");
            stringBuilder.AppendFormat($"  TriggerReason2: {TriggerReason_2}{Environment.NewLine}");
            stringBuilder.AppendFormat($"  TriggerReason3: {TriggerReason_3}{Environment.NewLine}");
            stringBuilder.AppendFormat($"UnlockedTimePer0: {UnlockedTimePeriod_0}{Environment.NewLine}");
            stringBuilder.AppendFormat($"UnlockedTimePer1: {UnlockedTimePeriod_1}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       Security0: {Security_0}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       Security1: {Security_1}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       Security2: {Security_2}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       Security3: {Security_3}{Environment.NewLine}");
            stringBuilder.AppendFormat($"ConfgChangdRecnt: {ConfigurationChangedRecently}{Environment.NewLine}");
            stringBuilder.AppendFormat($"PMUTriggrDetectd: {PMUTriggerDetected}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     DataSorting: {DataSorting}{Environment.NewLine}");
            stringBuilder.AppendFormat($"         PMUSync: {PMUSync}{Environment.NewLine}");
            stringBuilder.AppendFormat($"        PMUError: {PMUError}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       DataValid: {DataValid}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     DeviceError: {DeviceError}{Environment.NewLine}");
            stringBuilder.AppendFormat($" DataSortingType: {DataSortingType}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     SyncIsValid: {SynchronizationIsValid}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     DataIsValid: {DataIsValid}{Environment.NewLine}");
            stringBuilder.AppendFormat($"   DataDiscarded: {DataDiscarded}{Environment.NewLine}");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Overridden to prevent compilation warnings
        /// </summary>
        /// <returns>An integer hash code dictated by the base class</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="LinearStateEstimator.Measurements.StatusWord"/> object
        /// </summary>
        /// <returns>A deep copy of the <see cref="LinearStateEstimator.Measurements.StatusWord"/> object</returns>
        public StatusWord Copy()
        {
            return (StatusWord)this.MemberwiseClone();
        }

        /// <summary>
        /// Clears the values of the <see cref="LinearStateEstimator.Measurements.StatusWord"/> to default and lowers the <see cref="LinearStateEstimator.Measurements.StatusWord.StatusWordWasReported"/> flag.
        /// </summary>
        public void ClearValues()
        {
            ParseStatusFlag(0);
            m_statusWordWasReported = false;
        }

        public void Keyify(string rootKey)
        {
            Key = $"{rootKey}.Status";
        }

        public void Unkeyify()
        {
            Key = DEFAULT_MEASUREMENT_KEY;
        }
        #endregion

    }
}