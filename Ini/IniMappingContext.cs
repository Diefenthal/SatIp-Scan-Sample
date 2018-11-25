namespace SatIp
{
    public class IniMappingContext
    {
        public IniMappingContext(string source, IniMapping mapping)
        {
            Source = source;
            Mapping = mapping;
        }

        public string Source { get; set; }

        public IniMapping Mapping { get; set; }
    }
}