namespace AllupMVC.Services.Interfaces
{
    public interface ILayoutServices
    {
         Task<Dictionary<string, string>> GetSettingAsync();
    }
}
