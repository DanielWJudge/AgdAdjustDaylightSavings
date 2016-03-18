using ServiceStack.DataAnnotations;

namespace AgdAdjustDaylightSavings
{
    [Alias("settings")]
    public class AgdSettings
    {
        [Alias("settingID")]
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        [Alias("settingName")]
        [StringLength(64)]
        public string Name { get; set; }

        [Alias("settingValue")]
        [StringLength(8192)]
        public string Value { get; set; }
    }
}