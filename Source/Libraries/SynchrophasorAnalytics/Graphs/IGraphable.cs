//******************************************************************************************************
//  IGraphable.cs
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
//  06/14/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// The interface for classes which can be represented as graph data structures.
    /// </summary>
    public interface IGraphable
    {
        /// <summary>
        /// A method which initializes the adjacencly list representation of the data structure.
        /// </summary>
        void InitializeAdjacencyList();

        /// <summary>
        /// A method which resolves directly connected adjacencies into a single vertex.
        /// </summary>
        void ResolveAdjacencies();
    }
}
