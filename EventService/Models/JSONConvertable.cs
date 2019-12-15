namespace EventService.Models
{
    public interface IJSONConvertable
    {
         public string ConvertToJson(string command);
    }
}