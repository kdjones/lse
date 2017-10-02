using MeasurementSampler.Model.ECA;

namespace MeasurementSampler.Model.LSE
{
    public partial class _InputMeta
    {
        public _DigitalsMeta Digitals { get; set; }
        public _StatusWordsMeta StatusWords { get; set; }
        public _PhasorCollectionMeta VoltagePhasors { get; set; }
        public _PhasorCollectionMeta CurrentPhasors { get; set; }
    }
}
