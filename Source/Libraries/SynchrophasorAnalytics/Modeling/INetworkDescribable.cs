//******************************************************************************************************
//  INetworkDescribable.cs
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

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Defines basic descriptive properties common to most objects in the <see cref="LinearStateEstimator.Modeling"/> and <see cref="LinearStateEstimator.Measurements"/> namespace to define descriptive meta data for network components.
    /// </summary>
    public interface INetworkDescribable
    {
        /// <summary>
        /// A unique integer identifier for the object.
        /// </summary>
        int InternalID
        {
            get;
            set;
        }

        /// <summary>
        /// A descriptive number for the object.
        /// </summary>
        int Number
        {
            get;
            set;
        }

        /// <summary>
        /// A descriptive acronym for the object.
        /// </summary>
        string Acronym
        {
            get;
            set;
        }

        /// <summary>
        /// A descriptive name for the object.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A description of the object.
        /// </summary>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the object.
        /// </summary>
        string ElementType
        {
            get;
        }
    }
}

