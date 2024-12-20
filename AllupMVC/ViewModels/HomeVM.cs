using AllupMVC.Models;

namespace AllupMVC.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; }
        public List<Product> Products { get; set; }

        public string Search {  get; set; }
    }
}
