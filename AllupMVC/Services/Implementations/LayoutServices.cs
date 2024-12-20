using AllupMVC.DAL;
using AllupMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.Services.Implementations
{
    public class LayoutServices : ILayoutServices
    {
        private readonly AppDBContext _context;

        public LayoutServices(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingAsync()
        {
            Dictionary<string, string> settings =await _context.Settings.ToDictionaryAsync(s=>s.Key, s=>s.Value);
            return settings;
        }
    }
}
