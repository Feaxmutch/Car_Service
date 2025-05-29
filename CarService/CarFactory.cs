namespace CarService
{
    public class CarFactory
    {
        private readonly List<DetailFactory> _factoriys;

        public CarFactory(List<DetailFactory> detailFactorys)
        {
            ArgumentNullException.ThrowIfNull(detailFactorys);

            _factoriys = detailFactorys;
        }

        public Car Create()
        {
            List<Detail> details = new();

            foreach (var factory in _factoriys)
            {
                details.Add(factory.Create());
            }

            return new Car(details);
        }

        public List<Car> Create(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            List<Car> cars = new();

            for (int i = 0; i < count; i++)
            {
                cars.Add(Create());
            }

            return cars;
        }
    }
}
