using MeasurementSampler.Model.ECA;
using MeasurementSampler.Model.LSE;

namespace MeasurementSampler
{
    public interface IAnalytic
    {
        Input InputData
        {
            get;
            set;
        }

        _InputMeta InputMeta
        {
            get;
            set;
        }

        object Host
        {
            get;
        }

        void Execute();
    }
}
