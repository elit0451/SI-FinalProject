namespace CreationService.Models
{
    public interface IJSONConvertable
    {
        public string ConvertToJson(string command);
    }
}