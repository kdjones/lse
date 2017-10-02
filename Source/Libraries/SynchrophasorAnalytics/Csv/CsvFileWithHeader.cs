//******************************************************************************************************
//  CsvFileWithHeader.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SynchrophasorAnalytics.Csv
{
    public class CsvFileWithHeader : CsvFile
    {
        private List<Dictionary<string, string>> m_structuredData;
        private List<string> m_parameters;
        private string m_title;

        public List<Dictionary<string, string>> StructuredData
        {
            get
            {
                return m_structuredData;
            }
        }

        public List<string> Parameters
        {
            get
            {
                return m_parameters;
            }
        }

        public string Type
        {
            get
            {
                return m_title;
            }
        }

        public CsvFileWithHeader(string pathName)
            : base(pathName)
        {
            m_parameters = new List<string>();
            ParseHeader();
            ParseTitle();
            ParseDataIntoDictionary();
        }

        private void ParseHeader()
        {
            for (int i = 0; i < base.NumberOfColumns; i++)
            {
                m_parameters.Add(base.Data[0, i].Trim());
            }
        }

        private void ParseTitle()
        {
            string[] title = base.PathName.Split('\\');

            m_title = title[title.Count() - 1];
            m_title = m_title.Split('.')[0];
        }

        private void ParseDataIntoDictionary()
        {
            m_structuredData = new List<Dictionary<string, string>>();

            for (int i = 1; i < base.NumberOfRows; i++)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();

                for (int j = 0; j < base.NumberOfColumns; j++)
                {
                    row.Add(m_parameters[j], base.Data[i, j].Trim());
                }

                m_structuredData.Add(row);
            }
        }
        public string ToXml(string pathName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<?xml version=\"1.0\" standalone=\"yes\"?>{0}", Environment.NewLine);
            stringBuilder.AppendFormat("<" + m_title.ToUpper() + "s>{0}", Environment.NewLine);

            for (int i = 1; i < base.NumberOfRows; i++)
            {
                string s = "     <" + m_title.ToUpper() + " ";
                for (int j = 1; j < base.NumberOfColumns; j++)
                {
                    s += m_parameters[j].TrimStart('%').Replace('/', '_') + "=\"" + base.Data[i, j].Trim().Replace('&', 'n') + "\" ";
                }
                s += "/>";
                stringBuilder.AppendFormat(s + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendFormat("</" + m_title.ToUpper() + "s>{0}", Environment.NewLine);
            File.WriteAllText(pathName + "/" + m_title.ToUpper() + ".xml", stringBuilder.ToString());
            return stringBuilder.ToString();
        }
    }
}
