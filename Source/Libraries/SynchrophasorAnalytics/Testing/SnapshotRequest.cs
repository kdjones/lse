//******************************************************************************************************
//  SnapshotRequest.cs
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
//  07/25/2013 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynchrophasorAnalytics.Testing
{
    /// <summary>
    /// A request to be sent to the snapshot manager with a specified date and time of when to take a snapshot in the future.
    /// </summary>
    public class SnapshotRequest
    {
        #region [ Private Members ]

        private bool m_isComplete;
        private DateTime m_requestTime;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A boolean flag which indicates whether the snapshot has yet to be completed or not.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return m_isComplete;
            }
            set
            {
                m_isComplete = value;
            }
        }
        
        /// <summary>
        /// The time when a snapshot should be taken.
        /// </summary>
        public DateTime RequestTime
        {
            get
            {
                return m_requestTime;
            }
            set
            {
                m_requestTime = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The default constructor for the class. Sets the request time to DateTime.Now.
        /// </summary>
        public SnapshotRequest()
            :this(DateTime.Now)
        {
        }

        /// <summary>
        /// The designated constructor for the class. Requires specifing a datetime.
        /// </summary>
        /// <param name="requestTime">The time when the snapshot should be taken.</param>
        public SnapshotRequest(DateTime requestTime)
        {
            m_requestTime = requestTime;
            m_isComplete = false;
        }

        #endregion

    }
}
