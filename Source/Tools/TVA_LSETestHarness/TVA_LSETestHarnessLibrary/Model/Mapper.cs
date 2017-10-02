// COMPILER GENERATED CODE
// THIS WILL BE OVERWRITTEN AT EACH GENERATION
// EDIT AT YOUR OWN RISK

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ECAClientFramework;
using ECAClientUtilities;
using ECACommonUtilities;
using ECACommonUtilities.Model;
using GSF.TimeSeries;

namespace TVA_LSETestHarness.Model
{
    [CompilerGenerated]
    public class Mapper : MapperBase
    {
        #region [ Members ]

        // Fields
        private readonly Unmapper m_unmapper;

        #endregion

        #region [ Constructors ]

        public Mapper(Framework framework)
            : base(framework, SystemSettings.InputMapping)
        {
            m_unmapper = new Unmapper(framework, MappingCompiler);
            Unmapper = m_unmapper;
        }

        #endregion

        #region [ Methods ]

        public override void Map(IDictionary<MeasurementKey, IMeasurement> measurements)
        {
            SignalLookup.UpdateMeasurementLookup(measurements);
            TypeMapping inputMapping = MappingCompiler.GetTypeMapping(InputMapping);

            TVA_LSETestHarness.Model.ECA.PhasorCollection inputData = CreateECAPhasorCollection(inputMapping);
            KeyIndex = 0;
            TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta inputMeta = CreateECA_PhasorCollectionMeta(inputMapping);

            Algorithm.Output algorithmOutput = Algorithm.Execute(inputData, inputMeta);
            Subscriber.SendMeasurements(m_unmapper.Unmap(algorithmOutput.OutputData, algorithmOutput.OutputMeta));
        }

        private TVA_LSETestHarness.Model.ECA.PhasorCollection CreateECAPhasorCollection(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA.PhasorCollection obj = new TVA_LSETestHarness.Model.ECA.PhasorCollection();

            {
                // Create TVA_LSETestHarness.Model.ECA.Phasor UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrame(arrayMapping);

                List<TVA_LSETestHarness.Model.ECA.Phasor> list = new List<TVA_LSETestHarness.Model.ECA.Phasor>();
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

        private TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta CreateECA_PhasorCollectionMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta obj = new TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta();

            {
                // Create TVA_LSETestHarness.Model.ECA._PhasorMeta UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrame(arrayMapping);

                List<TVA_LSETestHarness.Model.ECA._PhasorMeta> list = new List<TVA_LSETestHarness.Model.ECA._PhasorMeta>();
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

        private TVA_LSETestHarness.Model.ECA.Phasor CreateECAPhasor(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA.Phasor obj = new TVA_LSETestHarness.Model.ECA.Phasor();

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

        private TVA_LSETestHarness.Model.ECA._PhasorMeta CreateECA_PhasorMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA._PhasorMeta obj = new TVA_LSETestHarness.Model.ECA._PhasorMeta();

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
