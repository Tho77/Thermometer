using System.Data;
using Dapper.Contrib.Extensions;
using Thermometer.Models;

namespace Thermometer.Repository
{
    public class ThermometerRepo : IThermometerRepo
    {
        private readonly IDbConnection _db;

        public ThermometerRepo(IDbConnection db)
        {
            _db = db;
        }

        public void Insert(Record data)
        {
            _db.Insert(data);
        }
    }
}
