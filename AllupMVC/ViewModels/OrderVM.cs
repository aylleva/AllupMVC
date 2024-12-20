namespace AllupMVC.ViewModels
{
    public class OrderVM
    {
        public string Address {  get; set; }
        public string Number { get; set; }

        public List<BasketinOrdersVM>? BasketinOrders { get; set; }
    }
}
