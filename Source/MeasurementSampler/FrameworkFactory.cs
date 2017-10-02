using ECAClientFramework;
using MeasurementSampler.Model;

namespace MeasurementSampler
{
    public static class FrameworkFactory
    {
        public static Framework Create(object analytic)
        {
            return new Framework(framework => new Mapper(framework, analytic));
        }
    }
}
