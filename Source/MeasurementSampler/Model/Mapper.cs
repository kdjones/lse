using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ECAClientFramework;
using ECAClientUtilities;
using ECACommonUtilities;
using ECACommonUtilities.Model;
using GSF.TimeSeries;
using MeasurementSampler.Model.ECA;
using MeasurementSampler.Model.LSE;

namespace MeasurementSampler.Model
{
    public class Mapper : MapperBase
    {
        #region [ Members ]

        private readonly Unmapper m_unmapper;
        private IAnalytic m_analytic;

        #endregion

        public IAnalytic Analytic
        {
            get
            {
                return m_analytic;
            }
            set
            {
                m_analytic = value;
            }
        }

        #region [ Constructor ] 

        public Mapper(Framework framework, object analytic)
            : base(framework, SystemSettings.InputMapping)
        {
            m_unmapper = new Unmapper(framework, MappingCompiler);
            Unmapper = m_unmapper;
            m_analytic = analytic as IAnalytic;
        }

        #endregion
      

        //public override void Map(IDictionary<MeasurementKey, IMeasurement> measurements)
        //{
        //    SignalLookup.UpdateMeasurementLookup(measurements);
        //    TypeMapping inputMapping = MappingCompiler.GetTypeMapping(InputMapping);
            
        //    Phasor inputData = CreateECAPhasor(inputMapping);
        //    KeyIndex = 0;
        //    _PhasorMeta inputMeta = CreateECA_PhasorMeta(inputMapping);
        //    Analytic.InputData = inputData;
        //    Analytic.InputMeta = inputMeta;
        //    Analytic.TakeSample();
        //    (Analytic.Host as IAlgorithmHost).SpecialStatus = $"Processing {inputData.Magnitude}";
        //    Algorithm.Output algorithmOutput = Algorithm.Execute(inputData, inputMeta);
        //    Subscriber.SendMeasurements(m_unmapper.Unmap(algorithmOutput.OutputData, algorithmOutput.OutputMeta));
        //}
      
        #region [ Methods ]

        public override void Map(IDictionary<MeasurementKey, IMeasurement> measurements)
        {
            SignalLookup.UpdateMeasurementLookup(measurements);
            TypeMapping inputMapping = MappingCompiler.GetTypeMapping(InputMapping);

            Reset();
            Input inputData = CreateLSEInput(inputMapping);
            Reset();
            _InputMeta inputMeta = CreateLSE_InputMeta(inputMapping);
            Analytic.InputData = inputData;
            Analytic.InputMeta = inputMeta;
            Analytic.Execute();
            Algorithm.Output algorithmOutput = Algorithm.Execute(inputData, inputMeta);
            Subscriber.SendMeasurements(m_unmapper.Unmap(algorithmOutput.OutputData, algorithmOutput.OutputMeta));
        }


        private Input CreateLSEInput(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            Input obj = new Input();

            {
                // Create Digitals UDT for "Digitals" field
                FieldMapping fieldMapping = fieldLookup["Digitals"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.Digitals = CreateECADigitals(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create StatusWords UDT for "StatusWords" field
                FieldMapping fieldMapping = fieldLookup["StatusWords"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.StatusWords = CreateECAStatusWords(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create PhasorCollection UDT for "VoltagePhasors" field
                FieldMapping fieldMapping = fieldLookup["VoltagePhasors"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.VoltagePhasors = CreateECAPhasorCollection(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create PhasorCollection UDT for "CurrentPhasors" field
                FieldMapping fieldMapping = fieldLookup["CurrentPhasors"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.CurrentPhasors = CreateECAPhasorCollection(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            return obj;
        }

        private _InputMeta CreateLSE_InputMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _InputMeta obj = new _InputMeta();

            {
                // Create _DigitalsMeta UDT for "Digitals" field
                FieldMapping fieldMapping = fieldLookup["Digitals"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.Digitals = CreateECA_DigitalsMeta(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create _StatusWordsMeta UDT for "StatusWords" field
                FieldMapping fieldMapping = fieldLookup["StatusWords"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.StatusWords = CreateECA_StatusWordsMeta(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create _PhasorCollectionMeta UDT for "VoltagePhasors" field
                FieldMapping fieldMapping = fieldLookup["VoltagePhasors"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.VoltagePhasors = CreateECA_PhasorCollectionMeta(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            {
                // Create _PhasorCollectionMeta UDT for "CurrentPhasors" field
                FieldMapping fieldMapping = fieldLookup["CurrentPhasors"];
                TypeMapping nestedMapping = GetTypeMapping(fieldMapping);

                PushRelativeFrame(fieldMapping);
                obj.CurrentPhasors = CreateECA_PhasorCollectionMeta(nestedMapping);
                PopRelativeFrame(fieldMapping);
            }

            return obj;
        }

        private Digitals CreateECADigitals(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            Digitals obj = new Digitals();

            {
                // Create double array for "Values" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Values"];

                List<double> list = new List<double>();
                int count = GetArrayMeasurementCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    IMeasurement measurement = GetArrayMeasurement(i);
                    list.Add((double)measurement.Value);
                }

                obj.Values = list.ToArray();
            }

            return obj;
        }

        private _DigitalsMeta CreateECA_DigitalsMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _DigitalsMeta obj = new _DigitalsMeta();

            {
                // Create MetaValues array for "Values" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Values"];

                List<MetaValues> list = new List<MetaValues>();
                int count = GetArrayMeasurementCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    IMeasurement measurement = GetArrayMeasurement(i);
                    list.Add(GetMetaValues(measurement));
                }

                obj.Values = list.ToArray();
            }

            return obj;
        }

        private StatusWords CreateECAStatusWords(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            StatusWords obj = new StatusWords();

            {
                // Create double array for "Values" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Values"];

                List<double> list = new List<double>();
                int count = GetArrayMeasurementCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    IMeasurement measurement = GetArrayMeasurement(i);
                    list.Add((double)measurement.Value);
                }

                obj.Values = list.ToArray();
            }

            return obj;
        }

        private _StatusWordsMeta CreateECA_StatusWordsMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _StatusWordsMeta obj = new _StatusWordsMeta();

            {
                // Create MetaValues array for "Values" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Values"];

                List<MetaValues> list = new List<MetaValues>();
                int count = GetArrayMeasurementCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    IMeasurement measurement = GetArrayMeasurement(i);
                    list.Add(GetMetaValues(measurement));
                }

                obj.Values = list.ToArray();
            }

            return obj;
        }

        private PhasorCollection CreateECAPhasorCollection(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            PhasorCollection obj = new PhasorCollection();

            {
                // Create Phasor UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrame(arrayMapping);

                List<Phasor> list = new List<Phasor>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(CreateECAPhasor(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopWindowFrame(arrayMapping);
            }

            return obj;
        }

        private _PhasorCollectionMeta CreateECA_PhasorCollectionMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _PhasorCollectionMeta obj = new _PhasorCollectionMeta();

            {
                // Create _PhasorMeta UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrame(arrayMapping);

                List<_PhasorMeta> list = new List<_PhasorMeta>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(CreateECA_PhasorMeta(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopWindowFrame(arrayMapping);
            }

            return obj;
        }

        private Phasor CreateECAPhasor(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            Phasor obj = new Phasor();

            {
                // Assign double value to "Magnitude" field
                FieldMapping fieldMapping = fieldLookup["Magnitude"];
                IMeasurement measurement = GetMeasurement(fieldMapping);
                obj.Magnitude = (double)measurement.Value;
            }

            {
                // Assign double value to "Angle" field
                FieldMapping fieldMapping = fieldLookup["Angle"];
                IMeasurement measurement = GetMeasurement(fieldMapping);
                obj.Angle = (double)measurement.Value;
            }

            return obj;
        }

        private _PhasorMeta CreateECA_PhasorMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _PhasorMeta obj = new _PhasorMeta();

            {
                // Assign MetaValues value to "Magnitude" field
                FieldMapping fieldMapping = fieldLookup["Magnitude"];
                IMeasurement measurement = GetMeasurement(fieldMapping);
                obj.Magnitude = GetMetaValues(measurement);
            }

            {
                // Assign MetaValues value to "Angle" field
                FieldMapping fieldMapping = fieldLookup["Angle"];
                IMeasurement measurement = GetMeasurement(fieldMapping);
                obj.Angle = GetMetaValues(measurement);
            }

            return obj;
        }

        #endregion

    }
}
