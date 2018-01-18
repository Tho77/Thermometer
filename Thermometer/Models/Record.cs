using Dapper.Contrib.Extensions;
using System;

namespace Thermometer.Models
{
    [Table("thermometer")]
    public class Record
    {
        public int Id { get; set; }
        public double? SensorHumidity { get; set; }
        public double? SensorTemp { get; set; }
        public double? OutsideTemp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
