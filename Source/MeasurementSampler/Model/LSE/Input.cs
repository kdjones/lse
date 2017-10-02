using MeasurementSampler.Model.ECA;

namespace MeasurementSampler.Model.LSE
{
    public partial class Input
    {
        public Digitals Digitals { get; set; }
        public StatusWords StatusWords { get; set; }
        public PhasorCollection VoltagePhasors { get; set; }
        public PhasorCollection CurrentPhasors { get; set; }
    }
}
