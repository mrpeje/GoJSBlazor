namespace GoJsWrapper.Models.DTO
{
    public class Port
    {
        public PortType PortType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
    }
    public enum PortType
    {
        Input,
        Output
    }
}
