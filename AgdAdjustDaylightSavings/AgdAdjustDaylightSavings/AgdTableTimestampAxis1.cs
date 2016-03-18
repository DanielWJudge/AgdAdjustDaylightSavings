using ServiceStack.DataAnnotations;

namespace AgdAdjustDaylightSavings
{
    [Alias("data")]
    public class AgdTableTimestampAxis1
    {
        [Alias("dataTimestamp")]
        public long TimestampTicks { get; set; }

        [Alias("axis1")]
        public double Axis1Counts { get; set; }
    }
}