using Thermometer.Models;

namespace Thermometer.Repository
{
    public interface IThermometerRepo
    {
        void Insert(Record data);
    }
}
