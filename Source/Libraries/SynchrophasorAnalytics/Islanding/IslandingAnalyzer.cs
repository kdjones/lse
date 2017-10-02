//******************************************************************************************************
//  IslandingAnalyzer.cs
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
//  05/01/2011 - Rui Sun
//       Developed original algorithms
//  06/01/2013 - Kevin D. Jones
//       Translated original version of source code in C#.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearStateEstimator.Islanding
{
    public class IslandingAnalyzer
    {
        #region [ Private Members ]

        /// <summary>
        /// Indicates an islanding condition
        /// islandingFlag = 0 --> no islanding condition
        /// islandingFlag = 1 --> islanding condition
        /// </summary>
        private IslandingFlag m_islandingFlag;

        /// <summary>
        /// Indicates the general location of the island within the system
        /// locationFlag = 0 --> no islanding condition
        /// locationFlag = 1 --> Yorktown
        /// locationFlag = 2 --> Chesterfield
        /// locationFlag = 3 --> Chesapeake
        /// locationFlag = 4 --> undetermined
        /// </summary>
        private LocationFlag m_locationFlag;

        /// <summary>
        /// Indicates the severity of the islanding condition
        /// severityFlag = 0 --> least severe
        /// severityFlag = 1 --> |
        /// severityFlag = 2 --> | 
        /// severityFlag = 3 --> |
        /// severityFlag = 4 --> |
        /// severityFlag = 5 --> most severe
        /// </summary>
        private SeverityFlag m_severityFlag;

        /// <summary>
        /// Indicates stability
        /// stabilityFlag = 0 --> unstable condition
        /// stabilityFlag = 1 --> stable condition
        /// </summary>
        private StabilityFlag m_stabilityFlag;

        #region betaPMU

        private double[] m_betaPMU = new double[]
                { 3.709790679843130,
                  7.268065320072130,
                  1.530521704201800,
                 -2.623737170569830,
                 -4.476884644939420,
                 -9.827231822150610,
                 -5.514564380500030,
                 -7.850164086128540,
                -74.153084193795000,
                 -2.261113848026390,
                  1.393770184784030,
                -13.191553000325400,
                  1.940171220961420,
                 -2.213533708564200,
                 -2.377332531248580,
                 -1.801676266248240,
                  0.711837667198347,
                 -3.895273655902250,
                  8.732219688102600,
                  2.846142283003260,
                 17.497300819023700,
                  3.037260942094290,
                  2.395401354895850,
                 -3.288551448232980,
                 -7.910334327045880,
                  2.269538909985710,
                  4.040076226436440,
                  2.108803113419440,
                 16.088468826586800,
                -17.955491220150300,
                  5.897114464914990};

        #endregion

        #region gammaPMU

        private double[] m_gammaPMU = new double[]
                {-35.44550731271150,
                  70.40690495361840,
                   6.49320004247941,
                  19.28206673175220,
                  28.41396183299310,
                  53.85953288142030,
                  15.87849687028580,
                 -17.96526739786970,
                -394.06307594398900,
                 -11.44044114602400,
                   4.50235275261085,
                 -76.59397462395260,
                   9.31565457643540,
                  -9.00328578528878,
                  -9.98116060730294,
                   8.19437362686518,
                  -3.90848831484305,
                 -44.17543617328580,
                  48.91000702439540,
                 -19.93292080418210,
                 143.55945957419000,
                 -14.57237644472970,
                  18.94611989660980,
                 -11.24155599011120,
                  23.24591535319030,
                  14.20834384054330,
                  13.63741083539140,
                   7.41791328019912,
                  42.06451992382610,
                  90.32891048950130,
                  33.80618721903670};

        #endregion

        #region sampleData1

         private double[] m_sampleData1 = new double[]
               {1.047181845,  -30.54109573,
                1.05101788,   -8.507529259,
                1.060820937,  -28.56294823,
                1.04742527,   -26.97224617,
                1.04218781,   -34.62270737,
                1.046632051,  -28.82861900,
                1.064833641,  -23.59498787,
                1.046730876,  -13.99713898,
                1.041501164,  -25.53259087,
                1.055955648,  -33.75431824,
                1.043054104,  -16.22282982,
                1.045874596,  -27.44652748,
                1.046850443,  -10.84301662,
                1.043785453,  -33.72787094,
                1.054216146,  -27.23223877,
                1.059294701,  -28.94595909,
                1.050872087,  -29.42087936,
                1.042490363,  -16.96411133,
                1.049288392,  -25.62197685,
                1.042630196,  -34.16925049,
                1.044454813,  -33.46392822,
                1.054928303,  -29.57026291,
                1.051221609,  -28.84642601,
                1.047539949,  -34.95106125,
                1.047458053,  -12.68457413,
                1.054765701,  -33.36843109,
                1.055962682,  -32.20571518,
               10.262758070,  -6.705732346,
               -8.684511519,   6.891298318,
               -5.366800706,   2.831129418,
               15.649872570,  -1.716772909,
               12.945252410,  -1.917660982,
                5.131541811,  -2.781270128,
                1.722674871,  -1.208356380,
               -2.650685672,   0.840770963,
               -4.751105456,   3.218564450,
               -3.892947831,   1.038872055,
               -3.918874517,   2.871154173,
               -4.500720307,   3.603321181,
               -6.308460043,   2.929535363,
               -5.493085920,   1.360610561,
               -5.991454415,   1.395860313,
                5.048585244,  -1.269635432,
                5.495614331,  -1.384262284,
              -12.958979650,   4.879641603,
               -5.841177757,   3.708394703,
                8.773841679,  -4.357153594,
               -8.169725546,   5.012629802,
                6.854057029,  -2.642450116,
               -8.507324657,   6.132101480,
               -2.679909997,   1.090482131,
                1.590803205,  -1.448683266,
               -7.990412053,   3.912237400,
               -4.730430936,   1.959118739,
               -3.567390199,   3.102097035,
               -4.935849944,   1.112574938,
                6.574328253,  -2.542232916,
               -5.567246580,   3.878166740};
        
        #endregion

        #region sampleData2

        private double[] m_sampleData2 = new double[]
               {1.045590878,	-35.28979874,	
                1.050000000,	-13.07366657,	
                1.053416252,	-33.10101318,
                1.038967848,	-32.24499893,	
                1.040886045,	-39.34505463,	
                1.044533610,	-33.63461304,
                1.060000000,	-28.06739998,	
                1.050000000,	-18.62854576,
                1.036540627,	-30.56658554,
                1.035545707,	-41.33987427,	
                1.040704846,	-20.98943329,	
                1.043370247,	-32.29271698,	
                1.050000000,	-15.39574814,	
                1.040000000,	-38.42164230,
                1.049432874,	-31.93556404,	
                1.049688101,	-33.74631882,	
                1.049334049,	-34.15362167,
                1.041518450,	-21.49394608,	
                1.047101736,	-30.40000725,	
                1.041315198,	-38.90560532,
                1.043183804,	-38.21012497,	
                1.041952729,	-34.80680466,	
                1.039542794,	-33.73726273,
                1.050000000,	-39.61502457,	
                1.050000000,	-17.27807426,	
                1.046673298,	-35.75669479,
                1.046283484,	-35.75588226,
                9.654336591,	-7.456213879,	
               -8.187207740,	 7.683859785,	
               -4.982563309,	 3.071939532,	
               15.521905540, 	-3.059524040,
               12.653516330,	-2.952616680,	
                3.472196123,	-2.396962601,	
                2.475051824,	-2.619871334,	
               -2.117620846,	 1.125343533,
               -4.446594779,	 3.742103177,	
               -4.076491341,     2.154976414,	
               -3.549397538,	 3.084247677,	
               -4.331086132,	 4.064897718,
               -5.908594239,	 3.243354030,	
               -5.515827765,	 1.913764478,	
               -6.005559250,	 1.979001557,
                5.198005958,	-1.897841006,	
                5.048281910,	-1.237317420,	
              -12.731702290,	 6.299224086,
               -8.012876373,	 7.050184563,	
                8.287849979,	-4.886175758,	
               -7.965482131,	 5.92420324,
                6.470167375,	-3.036615764,	
               -7.873465808,	 6.699774484,	
               -2.331488880,	 1.460232386,
                2.325270439,	-2.846826238,
               -7.575289253,	 4.427862382,
               -4.484303910,	 2.301650675,	
               -3.232291314,	 3.357526297,	
               -6.475958787,	 3.149315584,	
                4.107286311,	-1.308098472,	
               -0.00000000345,   0.0000000015};

        #endregion

        #region [ sampleData3 ]

        private double[] m_sampleData3 = new double[] 
        {   1.047578812,	-21.37148666 ,	
            1.051334381,	  0.453873605,	
            1.064140439,	-18.68592072 ,	
            1.049549699,	-17.28919029 ,	
            1.042568684,	-25.52101517 ,	
            1.047110915,	-19.57871246 ,	
            1.067377448,	-13.92346668 ,	
            1.047425985,	 -4.951334953,	
            1.042846322,	-16.06853676 ,	
            1.060254574,	-23.44671631 ,	
            1.044049382,	 -7.022650242,	
            1.046467662,	-18.13953972 ,	
            1.047211766,	 -1.899745464,	
            1.04428041 ,	-24.6588974	 ,	
            1.05622375 ,	-17.61889648 ,	
            1.062892437,	-18.99660492 ,	
            1.051315188,	-20.26005173 ,	
            1.043305874,	 -8.108581543,	
            1.049950361,	-16.29979324 ,	
            1.042951703,	-25.04843903 ,	
            1.044705749,	-24.33545685 ,	
            1.058933616,	-19.50194931 ,	
            1.055263638,	-18.77795982 ,	
            1.048059106,	-25.93098259 ,	
            1.048037648,	 -3.693002939,	
            1.059227467,	-22.88665771 ,	
            1.060651541,	-21.77960587 ,	
           11.32422791 ,	 -5.051262434,	
           -9.564319704,	  5.404741774,	
           -6.005893965,	  2.033589756,	
           15.62061935 ,      0.793486945,	
           13.19358959 ,	  0.142512915,	
            4.774096432,	 -1.426411501,	
            1.558178914,	 -0.704423336,	
           -2.27230068 ,	  0.090527041,	
           -5.025594513,	  2.269212819,	
           -3.459096308,	  0.112247523,	
           -4.538391566,	  2.269286408,	
           -4.81853118 ,	  2.810786928,	
           -6.969057077,	  1.985474197,	
           -5.412301662,	  0.449959599,	
           -5.943089135,	  0.404782772,	
            4.83595763 ,	 -0.395004513,	
            6.066654503,	 -0.70062162 ,	
          -13.21039865 ,	  2.628364682,	
           -6.063494155,	  2.471087372,	
            9.62606213 ,	 -3.020031594,	
           -8.949816872,	  3.666049376,	
            7.460521895,	 -1.637509262,	
           -9.591501493,	  4.780031283,	
           -2.351833675,	  0.373454275,	
            1.469359564,	 -0.964715489,	
           -8.88610615 ,	  2.715181597,	
           -5.158276368,	  1.199381568,	
           -4.083466657,	  2.50009935 ,	
           -5.069064029,	  0.251678395,	
            6.263491718,	 -1.038041082,	
           -5.875401952,	  2.769441973};

        #endregion

        #region [ sampleData4 ]

        private double[] m_sampleData4 = new double[] 
        {   1.046222806	,	-49.80978775,	
            1.050194621	,	-27.27812195,	
            1.058162332	,	-48.69137955,	
            1.042794466	,	-47.44650269,	
            1.041197419	,	-53.75266266,	
            1.045533299	,	-48.28301239,	
            1.062309861	,	-43.37700653,	
            1.044968128	,	-32.97055054,	
            1.038197517	,	-45.5479126	,	
            1.053541541	,	-54.38742065,	
            1.040536046	,	-35.58088303,	
            1.044557333	,	-47.0334816	,	
            1.04591918	,	-29.56806946,	
            1.042525053	,	-52.77883148,	
            1.05204761	,	-47.17342758,	
            1.056455016	,	-49.18721771,	
            1.049828529	,	-48.6621666	,	
            1.040501475	,	-35.51888275,	
            1.048132181	,	-45.17534256,	
            1.041786671	,	-53.34348679,	
            1.043782115	,	-52.66000748,	
            1.051874638	,	-49.99686432,	
            1.047990441	,	-49.31281662,	
            1.046213031	,	-53.89364243,	
            1.045981169	,	-31.53136444,	
            1.053096175	,	-54.09704208,	
            1.054001331	,	-52.81868744,	
            7.285593505	,	-9.447404843,	
            -6.133546557,	9.526897315	,	
            -3.685315204,	3.951567666 ,	
            14.4401225	,	-6.898124985,	
            11.37701609	,	-5.899253639,	
            4.365041427	,	-4.93433352	,	
            1.572572029	,	-2.20178058	,	
            -2.426751866,	2.005842953	,	
            -3.633794398,	4.904520975	,	
            -4.048195089,	3.339635027	,	
            -2.398316541,	3.682143494	,	
            -3.466538949,	5.21164512	,	
            -4.532295806,	4.349441756	,	
            -5.231041503,	3.416032336	,	
            -5.640414646,	3.589438576	,	
            5.095293649	,	-3.428172988,	
            4.008718952	,	-2.067993722,	
            -11.25795969,	9.582768673	,	
            -4.437193628,	5.750598352	,	
            6.438928057	,	-6.509382268,	
            -6.103821091,	7.607331762	,	
            5.186431046	,	-4.238238796,	
            -5.683057916,	8.241203539	,	
            -2.588653075,	2.49048726	,	
            1.364570821	,	-2.377219128,	
            -5.72287738	,	5.812287009	,	
            -3.532522916,	3.219830159	,	
            -2.208100706,	4.027665181	,	
            -4.10083728 ,	2.517800133	,	
            5.549620876	,	-4.800525203,	
            -4.338137258,	6.069329864	};

        #endregion

        #region [ sampleData5 ]

        private double[] m_sampleData5 = new double[] 
        {   1.041898727	,	-106.3912659	,	
            1.046584845	,	-82.23707581	,	
            1.056182742	,	-106.600914	    ,	
            1.034459352	,	-107.5536499	,	
            1.036816597	,	-109.9147263	,	
            1.04069376	,	-105.4635086	,	
            1.058457494	,	-100.4318771	,	
            1.037768841	,	-88.59883881	,	
            1.026004553	,	-104.6847458	,	
            1.050296903	,	-113.2676849	,	
            1.030751944	,	-92.49295807	,	
            1.039055586	,	-104.6424942	,	
            1.041662455	,	-84.38134003	,	
            1.037235498	,	-108.6878967	,	
            1.046183586	,	-105.3494034	,	
            1.053624749	,	-107.4078369	,	
            1.045286179	,	-105.1449966	,	
            1.033118963	,	-89.83649445	,	
            1.043694496	,	-102.560463	    ,	
            1.037899613	,	-109.6452332	,	
            1.04049933	,	-109.0352631	,	
            1.0479635	,	-108.7303543	,	
            1.04348743	,	-108.262001	    ,	
            1.040750504	,	-109.4666977	,	
            1.039923429	,	-86.74835968	,	
            1.050443172	,	-112.971405	    ,	
            1.051081896	,	-111.5547867	,	
            -3.498947685	,	-10.36874983	,	
            4.666161933	    ,	11.28116528	,	
            0.711579521	    ,	3.186939954	,	
            2.279447367	,	-16.75570232	,	
            1.345165995	,	-11.90347997	,	
            -2.407877599	,	-7.522778553   	,	
            -1.707582689	,	-4.056359004	,	
            -0.141511718	,	2.683118768	,	
            2.250208892	    ,	6.670383839	,	
            0.88879537	    ,	7.846524094	,	
            1.543454735	    ,	2.515187018	,	
            2.541845974	    ,	7.387030775	,	
            0.576590889	    ,	4.075351146	,	
            -0.043174923	,	7.958818216	,	
            -0.127938026	,	8.222924736	,	
            -0.240312033	,	-9.044410385	,	
            2.303121482	,	-0.373169283	,	
            2.672068747	    ,	17.6902184	,	
            2.624278148	    ,	6.977037653	,	
            -1.246975977	,	-7.031148291	,	
            3.664042816	    ,	10.26513736	,	
            -0.075339809	,	-4.72473568	    ,	
            3.419660262	    ,	7.883614055	,	
            1.148507052	    ,	4.852759581	,	
            -1.968416423	,	-3.976594468	,	
            1.325841994	,	5.870865725	,	
            0.759651612	    ,	3.66747689	,	
            2.193408652	    ,	3.528892816	,	
            -0.591802177	,	3.455108937	,	
            -0.927930863	,	-6.858109882	,	
            3.125605868	    ,	7.598511316	};

        #endregion

        #region [ sampleData6 ]

        private double[] m_sampleData6 = new double[] 
        {   1.043816686	,	-95.71839142	,	
            1.047714949	,	-71.90848541	,	
            1.059676409	,	-95.7988205	    ,	
            1.039089322	,	-96.4179306	    ,	
            1.038529992	,	-99.32361603	,	
            1.04306674	,	-94.67034912	,	
            1.061196804	,	-89.792099	    ,	
            1.039988279	,	-78.13521576	,	
            1.031383991	,	-93.50071716	,	
            1.053326964	,	-102.3655014	,	
            1.033995509	,	-81.77949524	,	
            1.041805267	,	-93.7638092	    ,	
            1.043009162	,	-74.08016205	,	
            1.039078355	,	-98.14762878	,	
            1.050389767	,	-94.35411835	,	
            1.057173371	,	-96.56912994	,	
            1.04718709	,	-94.49336243	,	
            1.034988046	,	-79.62234497	,	
            1.045999765	,	-91.73230743	,	
            1.039552331	,	-99.02646637	,	
            1.042034864	,	-98.40209961	,	
            1.051554203	,	-97.83132172	,	
            1.047222495	,	-97.33470154	,	
            1.04251647	,	-98.99213409	,	
            1.041736126	,	-76.36589813	,	
            1.053222775	,	-102.0869217	,	
            1.054090619	,	-100.6770172	,	
            -1.589995101	,	-11.03176855	,	
            2.479899652	    ,	11.76624012	,	
            0.212515717	    ,	3.691752034	,	
            5.321081803	,	-15.85647359	,	
            3.579454763	,	-11.59850209	,	
            -0.976568572	,	-7.740075306	,	
            -0.886368476	,	-4.122729675	,	
            -0.545037487	,	3.013974075	,	
            0.925700069	    ,	6.80816615	,	
            -0.523385499	,	8.034900107	,	
            1.05622034	    ,	3.057818728	,	
            1.154408427	    ,	7.413630413	,	
            -0.083873326	,	4.547438811	,	
            -1.484944062	,	7.47251365	,	
            -1.609574023	,	7.74871974	,	
            1.430926499	    ,	-8.363214049	,	
            1.898754491	,	-0.802310722	,	
            -0.852710389	,	17.24634505	,	
            1.311310068	    ,	7.3582528	,	
            -0.048052986	,	-7.543993518	,	
            1.534457735	    ,	10.53708225	,	
            0.72339996	    ,	-5.047055092	,	
            1.938259358	    ,	8.659437982	,	
            0.018816925	    ,	4.700539078	,	
            -1.158450064	,	-4.093251954	,	
            0.268203854	,	6.438679288	,	
            0.037004658	    ,	3.940953206	,	
            1.473881466	    ,	3.977911518	,	
            -1.162517956	,	3.476441041	,	
            0.290709371	    ,	-7.023662518	,	
            1.704214469	    ,	8.023932195	};

        #endregion

        #endregion

        #region [ Properties ]


        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData1
        {
            get 
            { 
                return m_sampleData1; 
            }
        }

        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData2
        {
            get 
            { 
                return m_sampleData2; 
            }
        }

        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData3
        {
            get
            {
                return m_sampleData3;
            }
        }

        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData4
        {
            get
            {
                return m_sampleData4;
            }
        }

        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData5
        {
            get
            {
                return m_sampleData5;
            }
        }

        /// <summary>
        /// Getter method for the sample data
        /// </summary>
        /// <returns>
        /// Returns a Matrix object of a column
        /// matrix with the sample data
        /// </returns>
        public double[] SampleData6
        {
            get
            {
                return m_sampleData6;
            }
        }

        /// <summary>
        /// Getter method for the islanding flag
        /// </summary>
        /// <returns>
        /// Returns an int representing an islanding
        /// or non-islanding case
        /// </returns>
        public IslandingFlag Islanding
        {
            get 
            { 
                return m_islandingFlag; 
            }
        }

        /// <summary>
        /// Getter method for the location flag
        /// </summary>
        /// <returns>
        /// Returns an int representing the location
        /// of the detected island
        /// </returns>
        public LocationFlag Location
        {
            get 
            { 
                return m_locationFlag; 
            }
        }

        /// <summary>
        /// Getter method for stability flag
        /// </summary>
        /// <returns>
        /// Returns an int representing the
        /// stability or instability of the
        /// islanding event.
        /// </returns>
        public StabilityFlag Stability
        {
            get 
            { 
                return m_stabilityFlag; 
            }
        }

        /// <summary>
        /// Getter method for the severity flag
        /// </summary>
        /// <returns>
        /// Returns an int representing the
        /// severity of the islanding event.
        /// </returns>
        public SeverityFlag Severity
        {
            get 
            { 
                return m_severityFlag; 
            }
        }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Constructor method to initialize flags
        /// </summary>
        public IslandingAnalyzer()
        {
            m_islandingFlag = IslandingFlag.NoIslandingConditionExists;
            m_locationFlag = LocationFlag.NoIslandingCondition;
            m_severityFlag = SeverityFlag.LowSeverity;
            m_stabilityFlag = StabilityFlag.Stable;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Prepares the data for the decistion tree
        /// </summary>
        /// <param name="PMUData">
        /// A column matrix inside a Matrix object that contains the voltages and current flows from the latest state calculation
        /// </param>
        /// <returns>
        /// Returns a Matrix object which is a column matrix of processed data ready for the decision tree
        /// </returns>
        public double[] varCovariance(double[] PMUData)
        {
            double[] convertedPMUData = new double[85];
            
            for (int i = 0; i < 54; i++)
            {
                /// Include the original voltage data
                convertedPMUData[i] = PMUData[i];
            }

            for (int j = 0; j < 31; j++)
            {
                /// Convert the current flows into usable data
                convertedPMUData[j + 54] = 1 / (1 + m_betaPMU[j] * m_betaPMU[j]) *
                     (m_gammaPMU[j] - PMUData[(2 * j ) + 54] - m_betaPMU[j] * PMUData[(2 * j + 1) + 54]);
            }

            return convertedPMUData;
        }

        /// <summary>
        /// Islanding Detection and Location Judgement
        /// 
        /// Decision Tree Splitting Points Data
        /// 
        /// DT_1 Islanding Judgement: Full loop DVP
        /// 
        /// DT_1 Location: Yorktown
        /// 
        /// DT_1 Location: Chesterfield
        /// 
        /// DT_1 Location: Chesapeake
        /// </summary>
        /// <param name="data">
        /// A column matrix of processed data 
        /// prepared for the decision tree
        /// </param>
        public void islandingJudgment(double[] data)
        {
            if ((data[50] <= 1.052513 && data[8] > 1.027590 && data[84] > -0.503139 &&
                (data[52] <= 1.052598 || (data[52] > 1.052598 && data[4] > 1.057792)))
                || (data[50] > 1.052513 && ((data[4] <= 1.058394 && data[63] <= 0.062944)
                || (data[4] > 1.058394 && data[0] <= 1.047624))))
            {
                m_islandingFlag = IslandingFlag.IslandingConditionExists;

                if (m_islandingFlag == IslandingFlag.IslandingConditionExists)
                {

                    /// Location determination
                    if (data[55]>-0.169610 && data[84]<=0.547558 && 
                        ((data[70]<=-4.106833 && data[0]>1.045549 && 
                        (data[69]>-0.801558||(data[69]<=-0.801558 && 
                        data[0]<=1.045994)))||(data[70]>-4.106833 && 
                        data[61]>0.03 && data[63]<=0.462647 && data[70]<=-3.876851)))
                    {
                        /// Yorktown
                        m_locationFlag = LocationFlag.Yorktown;
                    }
                    else if (data[55] <= -0.17 && data[8] <= 1.04)
                    {
                        /// Chesterfield
                        m_locationFlag = LocationFlag.Chesterfield;
                    }
                    else if (data[55]>-0.169010 && (data[84]>0.547558 || 
                        (data[84]<=0.547558 && data[70]>-4.106833 && 
                        data[61]>0.032730 && data[63]>0.462647 && data[0]<=1.044399)))
                    {
                        /// Chesapeake
                        m_locationFlag = LocationFlag.Chesapeake;
                    }
                    else
                    {
                        /// Undetermined
                        m_locationFlag = LocationFlag.Undetermined;
                    }
                }
            }
            else
            {
                m_islandingFlag = IslandingFlag.NoIslandingConditionExists;
                m_locationFlag = LocationFlag.NoIslandingCondition;
            }

        }

        /// <summary>
        /// Use the data to determine the severity and stability of the islanding condition.
        /// </summary>
        /// <param name="data">Data processed by <see cref="IslandingAnalyzer.varCovariance"/></param>
        public void severityRanking(double[] data)
        {
            if (m_locationFlag == LocationFlag.Chesterfield) /// Chesterfield
            {
                if ((data[83]<=-0.112008 && (data[70]>-3.419310  || (data[70]<=-3.419310 &&
                   ((data[55]<=-0.313085 &&  data[80]>-0.101498) || (data[55]>-0.313085 &&
                    (data[42]>1.045836   && (data[42]<=-1.045836 &&  data[80]>-0.109491 &&
                     data[63]>0.823080)))))) || ((data[83]>-0.10491 && data[8]>1.035872) &&
                   ((data[77]<=0.392381  && data[82]>-0.089507) || (data[77]>0.392381 && data[82]>-0.077481)))))
                {
                    m_stabilityFlag = StabilityFlag.Stable;
                }
                else
                {
                    m_stabilityFlag = StabilityFlag.Unstable;
                    m_severityFlag = SeverityFlag.MostSevere;
                }

                if (m_stabilityFlag == StabilityFlag.Stable)
                {
                    if (data[70]<=-3.359188 && ((data[83]<=-0.108398 && data[74]>-0.140426 &&
                        data[83]>-0.115381  && (data[62]>0.019962 || (data[62]<=0.019962 &&
                        data[61]>0.209909))) || (data[83]>-0.115381 && data[70]>-3.921112 &&
                        data[70]<=-3.787080 && data[61]>0.077426)))
                    {
                        m_severityFlag = SeverityFlag.MediumSeverity;
                    }
                    else if (data[70] > -3.359188)
                    {
                        m_severityFlag = SeverityFlag.MediumSeverity;
                    } 
                    else if (data[70]<=-3.359188 && data[83]>-0.108398 && (data[70]<=-3.921112 || 
                            (data[70]>-3.921112  && ((data[70]<=-3.787080 && data[61]<=0.077426) ||
                            (data[70]>-3.787080  && data[46]<=1.043135)))))
                    {
                        m_severityFlag = SeverityFlag.MediumHighSeverity;
                    }
                    
                    else if (data[70]<=-3.359188 && data[83]<=-0.108398 && data[74]>-0.140426 && 
                           ((data[83]<=-0.115381 && data[77]>0.430919) || (data[83]>-0.115381 && 
                             data[62]<=0.019962  && data[61]<=0.209909)))
                    {
                        m_severityFlag = SeverityFlag.HighSeverity;
                    }
                    else
                    {
                        m_severityFlag = SeverityFlag.MostSevere;
                    }
                }

            }

            if (m_locationFlag == LocationFlag.Chesapeake) ///Chesapeake
            {                
                if (data[83]<=0.268115 && data[84]>0.480919 && (data[59]<=0.243917 ||
                    (data[59]>0.243917 && (data[14]>1.047064 || (data[14]<=1.047064 && 
                    ((data[4]<=1.058651 && (data[2]>1.050963 || (data[2]<=1.050963 &&
                    data[82]<=0.262220 && data[50]<=1.045673))) || (data[4]>1.059237 && 
                    data[54]>-2.548871)))))))
                {
                    m_stabilityFlag = StabilityFlag.Stable;
                }
                else
                {
                    m_severityFlag = SeverityFlag.MostSevere;
                }

                if (m_stabilityFlag == StabilityFlag.Stable)
                {
                    if (data[40]<=1.043518 && data[59]>0.271239 && data[50]>1.046591) 
                    {
                        m_severityFlag = SeverityFlag.MediumSeverity;
                    }
                    else
                    {
                        m_severityFlag = SeverityFlag.HighSeverity;
                    }
                }


            }

            if (m_locationFlag == LocationFlag.Yorktown) /// Yorktown
            {
                m_severityFlag = SeverityFlag.MediumSeverity;
            }
        }

        #endregion 
    }
}
