//******************************************************************************************************
//  BreakerStatusBit.cs
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
//  07/20/2011 - Kevin D. Jones
//       Generated original version of source code.
//  06/01/2013 - Kevin D. Jones
//       Modified to include Xml Serialization
//  06/23/2014 - Kevin D. Jones
//       Added more enumerations to create a 16 bit word
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// This enumeration specifies the bit mask applied to the digital word in the data stream which contains the breaker status information. The convention comes from SEL PMUs.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Measurements.BreakerStatus"/>
    [Serializable()]
    public enum BreakerStatusBit
    {
        /// <summary>
        /// The <b>PSV64</b> bit. The least significant bit.
        /// </summary>
        [XmlEnum("PSV64")]
        PSV64,

        /// <summary>
        /// The <b>PSV63</b> bit.
        /// </summary>
        [XmlEnum("PSV63")]
        PSV63,

        /// <summary>
        /// The <b>PSV62</b> bit.
        /// </summary>
        [XmlEnum("PSV62")]
        PSV62,

        /// <summary>
        /// The <b>PSV61</b> bit.
        /// </summary>
        [XmlEnum("PSV61")]
        PSV61,

        /// <summary>
        /// The <b>PSV60</b> bit.
        /// </summary>
        [XmlEnum("PSV60")]
        PSV60,

        /// <summary>
        /// The <b>PSV59</b> bit.
        /// </summary>
        [XmlEnum("PSV59")]
        PSV59,

        /// <summary>
        /// The <b>PSV58</b> bit.
        /// </summary>
        [XmlEnum("PSV58")]
        PSV58,

        /// <summary>
        /// The <b>PSV57</b> bit.
        /// </summary>
        [XmlEnum("PSV57")]
        PSV57,

        /// <summary>
        /// The <b>PSV56</b> bit.
        /// </summary>
        [XmlEnum("PSV56")]
        PSV56,

        /// <summary>
        /// The <b>PSV55</b> bit.
        /// </summary>
        [XmlEnum("PSV55")]
        PSV55,

        /// <summary>
        /// The <b>PSV54</b> bit.
        /// </summary>
        [XmlEnum("PSV54")]
        PSV54,

        /// <summary>
        /// The <b>PSV53</b> bit.
        /// </summary>
        [XmlEnum("PSV53")]
        PSV53,

        /// <summary>
        /// The <b>PSV52</b> bit.
        /// </summary>
        [XmlEnum("PSV52")]
        PSV52,

        /// <summary>
        /// The <b>PSV51</b> bit.
        /// </summary>
        [XmlEnum("PSV51")]
        PSV51,

        /// <summary>
        /// The <b>PSV50</b> bit.
        /// </summary>
        [XmlEnum("PSV50")]
        PSV50,

        /// <summary>
        /// The <b>PSV49</b> bit. The most significant bit.
        /// </summary>
        [XmlEnum("PSV49")]
        PSV49
    }
}
