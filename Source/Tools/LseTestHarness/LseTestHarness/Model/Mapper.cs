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

namespace LseTestHarness.Model
{
    [CompilerGenerated]
    public class Mapper : MapperBase
    {
        #region [ Constructors ]

        public Mapper(Framework framework)
            : base(framework, "LSEDemo")
        {
        }

        #endregion

        #region [ Methods ]

        public override void Map(IDictionary<MeasurementKey, IMeasurement> measurements)
        {
            SignalLookup.UpdateMeasurementLookup(measurements);
            TypeMapping inputMapping = MappingCompiler.GetTypeMapping(InputMapping);

            LseTestHarness.Model.ECA.PhasorCollection inputData = CreateECAPhasorCollection(inputMapping);
            KeyIndex = 0;
            LseTestHarness.Model.ECA._PhasorCollectionMeta inputMeta = CreateECA_PhasorCollectionMeta(inputMapping);
            MainWindow.WriteMessage("Executing..");
            Algorithm.Output algorithmOutput = Algorithm.Execute(inputData, inputMeta);

            // TODO: Later versions will publish output to the openECA server
        }

        private LseTestHarness.Model.ECA.PhasorCollection CreateECAPhasorCollection(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            LseTestHarness.Model.ECA.PhasorCollection obj = new LseTestHarness.Model.ECA.PhasorCollection();

            {
                // Create LseTestHarness.Model.ECA.Phasor UDT array for "Phasors" field
                PushCurrentFrame();
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];

                List<LseTestHarness.Model.ECA.Phasor> list = new List<LseTestHarness.Model.ECA.Phasor>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(CreateECAPhasor(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopCurrentFrame();
            }

            return obj;
        }

        private LseTestHarness.Model.ECA._PhasorCollectionMeta CreateECA_PhasorCollectionMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            LseTestHarness.Model.ECA._PhasorCollectionMeta obj = new LseTestHarness.Model.ECA._PhasorCollectionMeta();

            {
                // Create LseTestHarness.Model.ECA._PhasorMeta UDT array for "Phasors" field
                PushCurrentFrame();
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];

                List<LseTestHarness.Model.ECA._PhasorMeta> list = new List<LseTestHarness.Model.ECA._PhasorMeta>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(CreateECA_PhasorMeta(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopCurrentFrame();
            }

            return obj;
        }

        private LseTestHarness.Model.ECA.Phasor CreateECAPhasor(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            LseTestHarness.Model.ECA.Phasor obj = new LseTestHarness.Model.ECA.Phasor();

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

        private LseTestHarness.Model.ECA._PhasorMeta CreateECA_PhasorMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            LseTestHarness.Model.ECA._PhasorMeta obj = new LseTestHarness.Model.ECA._PhasorMeta();

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
