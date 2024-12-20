using AllupMVC.ViewModels;

namespace AllupMVC.Services.Interfaces
{
    public interface IBasketService
    {
        Task<List<BasketitemVM>> GetBasketAsync();
    }
}
