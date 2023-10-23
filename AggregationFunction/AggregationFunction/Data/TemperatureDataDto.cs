using System;

namespace AggregationFunction.Data;

internal class TemperatureDataDto
{
    public DateTime DateTime { get; set; }
    public double Temperature { get; set; }
    public string Location { get; set; }

}
