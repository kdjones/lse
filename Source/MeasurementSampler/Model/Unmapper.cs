using System;
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
    public class Unmapper : UnmapperBase
    {
        #region [ Constrcutors ]

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

        public NullOutput FillOutputData()
        {
            TypeMapping outputMapping = MappingCompiler.GetTypeMapping(OutputMapping);
            return FillLSENullOutput(outputMapping);
        }

        public _NullOutputMeta FillOutputMeta()
        {
            TypeMapping outputMeta = MappingCompiler.GetTypeMapping(OutputMapping);
            return FillLSE_NullOutputMeta(outputMeta);
        }

        public IEnumerable<IMeasurement> Unmap(NullOutput outputData, _NullOutputMeta outputMeta)
        {
            List<IMeasurement> measurements = new List<IMeasurement>();
            TypeMapping outputMapping = MappingCompiler.GetTypeMapping(OutputMapping);

            Reset();
            CollectFromLSENullOutput(measurements, outputMapping, outputData, outputMeta);

            return measurements;
        }

        private NullOutput FillLSENullOutput(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            NullOutput obj = new NullOutput();



            return obj;
        }

        private _NullOutputMeta FillLSE_NullOutputMeta(TypeMapping typeMapping)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);
            _NullOutputMeta obj = new _NullOutputMeta();

            {
                // Initialize meta value structure to "Value" field
                FieldMapping fieldMapping = fieldLookup["Value"];
                obj.Value = CreateMetaValues(fieldMapping);
            }

            return obj;
        }

        private void CollectFromLSENullOutput(List<IMeasurement> measurements, TypeMapping typeMapping, NullOutput data, _NullOutputMeta meta)
        {
            Dictionary<string, FieldMapping> fieldLookup = typeMapping.FieldMappings.ToDictionary(mapping => mapping.Field.Identifier);

            {
                // Convert value from "Value" field to measurement
                FieldMapping fieldMapping = fieldLookup["Value"];
                IMeasurement measurement = MakeMeasurement(meta.Value, Convert.ToDouble(data.Value));
                measurements.Add(measurement);
            }
        }

        #endregion
    }
}
