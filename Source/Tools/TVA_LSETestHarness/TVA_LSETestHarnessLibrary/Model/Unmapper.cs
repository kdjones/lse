// COMPILER GENERATED CODE
// THIS WILL BE OVERWRITTEN AT EACH GENERATION
// EDIT AT YOUR OWN RISK

using System;
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
    public class Unmapper : UnmapperBase
    {
        #region [ Constructors ]

        public Unmapper(Framework framework, MappingCompiler mappingCompiler)
            : base(framework, mappingCompiler, SystemSettings.OutputMapping)
        {
            Algorithm.Output.CreateNew = () => new Algorithm.Output()
            {
                OutputData = FillOutputData(),
                OutputMeta = FillOutputMeta()
            };
        }

        #endregion

        #region [ Methods ]

        public TVA_LSETestHarness.Model.ECA.PhasorCollection FillOutputData()
        {
            TypeMapping outputMapping = MappingCompiler.GetTypeMapping(OutputMapping);
            return FillECAPhasorCollection(outputMapping);
        }

        public TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta FillOutputMeta()
        {
            TypeMapping outputMeta = MappingCompiler.GetTypeMapping(OutputMapping);
            return FillECA_PhasorCollectionMeta(outputMeta);
        }

        public IEnumerable<IMeasurement> Unmap(TVA_LSETestHarness.Model.ECA.PhasorCollection outputData, TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta outputMeta)
        {
            List<IMeasurement> measurements = new List<IMeasurement>();
            TypeMapping outputMapping = MappingCompiler.GetTypeMapping(OutputMapping);

            Reset();
            CollectFromECAPhasorCollection(measurements, outputMapping, outputData, outputMeta);

            return measurements;
        }

        private TVA_LSETestHarness.Model.ECA.PhasorCollection FillECAPhasorCollection(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA.PhasorCollection obj = new TVA_LSETestHarness.Model.ECA.PhasorCollection();

            {
                // Initialize TVA_LSETestHarness.Model.ECA.Phasor UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrameTime(arrayMapping);

                List<TVA_LSETestHarness.Model.ECA.Phasor> list = new List<TVA_LSETestHarness.Model.ECA.Phasor>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(FillECAPhasor(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopWindowFrameTime(arrayMapping);
            }

            return obj;
        }

        private TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta FillECA_PhasorCollectionMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta obj = new TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta();

            {
                // Initialize TVA_LSETestHarness.Model.ECA._PhasorMeta UDT array for "Phasors" field
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                PushWindowFrameTime(arrayMapping);

                List<TVA_LSETestHarness.Model.ECA._PhasorMeta> list = new List<TVA_LSETestHarness.Model.ECA._PhasorMeta>();
                int count = GetUDTArrayTypeMappingCount(arrayMapping);

                for (int i = 0; i < count; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    list.Add(FillECA_PhasorMeta(nestedMapping));
                }

                obj.Phasors = list.ToArray();
                PopWindowFrameTime(arrayMapping);
            }

            return obj;
        }

        private TVA_LSETestHarness.Model.ECA.Phasor FillECAPhasor(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA.Phasor obj = new TVA_LSETestHarness.Model.ECA.Phasor();

            

            return obj;
        }

        private TVA_LSETestHarness.Model.ECA._PhasorMeta FillECA_PhasorMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            TVA_LSETestHarness.Model.ECA._PhasorMeta obj = new TVA_LSETestHarness.Model.ECA._PhasorMeta();

            {
                // Initialize meta value structure to "Magnitude" field
                FieldMapping fieldMapping = fieldLookup["Magnitude"];
                obj.Magnitude = CreateMetaValues(fieldMapping);
            }

            {
                // Initialize meta value structure to "Angle" field
                FieldMapping fieldMapping = fieldLookup["Angle"];
                obj.Angle = CreateMetaValues(fieldMapping);
            }

            return obj;
        }

        private void CollectFromECAPhasorCollection(List<IMeasurement> measurements, TypeMapping typeMapping, TVA_LSETestHarness.Model.ECA.PhasorCollection data, TVA_LSETestHarness.Model.ECA._PhasorCollectionMeta meta)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);

            {
                // Convert values from TVA_LSETestHarness.Model.ECA.Phasor UDT array for "Phasors" field to measurements
                ArrayMapping arrayMapping = (ArrayMapping)fieldLookup["Phasors"];
                int dataLength = data.Phasors.Length;
                int metaLength = meta.Phasors.Length;

                if (dataLength != metaLength)
                    throw new InvalidOperationException($"Values array length ({dataLength}) and MetaValues array length ({metaLength}) for field \"Phasors\" must be the same.");

                PushWindowFrameTime(arrayMapping);

                for (int i = 0; i < dataLength; i++)
                {
                    TypeMapping nestedMapping = GetUDTArrayTypeMapping(arrayMapping, i);
                    CollectFromECAPhasor(measurements, nestedMapping, data.Phasors[i], meta.Phasors[i]);
                }

                PopWindowFrameTime(arrayMapping);
            }
        }

        private void CollectFromECAPhasor(List<IMeasurement> measurements, TypeMapping typeMapping, TVA_LSETestHarness.Model.ECA.Phasor data, TVA_LSETestHarness.Model.ECA._PhasorMeta meta)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);

            {
                // Convert value from "Magnitude" field to measurement
                FieldMapping fieldMapping = fieldLookup["Magnitude"];
                IMeasurement measurement = MakeMeasurement(meta.Magnitude, (double)data.Magnitude);
                measurements.Add(measurement);
            }

            {
                // Convert value from "Angle" field to measurement
                FieldMapping fieldMapping = fieldLookup["Angle"];
                IMeasurement measurement = MakeMeasurement(meta.Angle, (double)data.Angle);
                measurements.Add(measurement);
            }
        }

        #endregion
    }
}
