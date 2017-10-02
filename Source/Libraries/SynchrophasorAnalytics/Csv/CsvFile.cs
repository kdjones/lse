//******************************************************************************************************
//  CsvFile.cs
//
//  Copyright © 2015, Kevin D. Jones.  All Rights Reserved.
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
//  06/01/2015 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Csv
{
    public class CsvFile
    {
        private int m_numberOfRows;
        private int m_numberOfColumns;
        private string[,] m_rawCommaSeparatedValues;
        private string m_pathName;

        public int NumberOfRows
        {
            get
            {
                return m_numberOfRows;
            }
        }

        public int NumberOfColumns
        {
            get
            {
                return m_numberOfColumns;
            }
        }

        public string[,] Data
        {
            get
            {
                return m_rawCommaSeparatedValues;
            }
        }

        public string PathName
        {
            get
            {
                return m_pathName;
            }
        }


        public CsvFile(string pathName)
        {
            m_pathName = pathName;
            Initialize();
            Parse();
        }

        private void Initialize()
        {
            m_numberOfColumns = 0;
            m_numberOfRows = 0;

            // Read sample data from CSV file to determine the dimensions of the data.
            using (CsvFileReader reader = new CsvFileReader(m_pathName))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    m_numberOfRows++;
                    m_numberOfColumns = 0;
                    foreach (string s in row)
                    {
                        m_numberOfColumns++;
                    }
                }
            }
        }

        private void Parse()
        {
            int rowIndex = 0;
            int columnIndex = 0;
            m_rawCommaSeparatedValues = new string[m_numberOfRows, m_numberOfColumns];
            using (CsvFileReader reader = new CsvFileReader(m_pathName))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    columnIndex = 0;
                    foreach (string s in row)
                    {
                        m_rawCommaSeparatedValues[rowIndex, columnIndex] = s;
                        columnIndex++;
                    }
                    rowIndex++;
                }
            }

        }

    }
}
