 namespace CarService
{
    public class DetailFactory
    {
        private readonly string _name;
        private readonly int _price;

        public DetailFactory(string name, int price)
        {
            _name = name;
            _price = price;
        }

        public Detail Create()
        {
            return new Detail(_name, _price);
        }
    }
}
