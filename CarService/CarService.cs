namespace CarService
{
    public class CarService
    {
        private readonly Queue<Car> _cars;
        private readonly Storage _storage;
        public CarService(Storage storage, int startMoney, int fixedPenality, float dynamicPenalityMultiplyer, int workPrice)
        {
            ArgumentNullException.ThrowIfNull(storage);
            ArgumentOutOfRangeException.ThrowIfNegative(startMoney);
            ArgumentOutOfRangeException.ThrowIfNegative(fixedPenality);
            ArgumentOutOfRangeException.ThrowIfNegative(dynamicPenalityMultiplyer);
            ArgumentOutOfRangeException.ThrowIfNegative(workPrice);

            _storage = storage;
            _cars = new();
            Money = startMoney;
            FixedPenality = fixedPenality;
            DynamicPenalityMultiplyer = dynamicPenalityMultiplyer;
            WorkPrice = workPrice;
        }

        public int Money { get; private set; }

        public Car CurrentCar => _cars.Peek();

        public int CarsCount => _cars.Count;

        public int FixedPenality { get; }

        public float DynamicPenalityMultiplyer { get; }

        public int WorkPrice { get; }

        public void TakeCars(List<Car> cars)
        {
            ArgumentNullException.ThrowIfNull(cars);

            _cars.EnqueueRange(cars);
        }

        public void ReplaceDetail(int index)
        {
            UserUtilits.HandleIndexExeptions(CurrentCar.Details, index);
            Detail sample = CurrentCar.Details[index].Clone();

            if (HaveDetail(index) == false)
            {
                throw new ArgumentException($"Storage is not have detail \"{sample.Name}\"");
            }

            if (sample.IsBroken)
            {
                Money += sample.Price + WorkPrice;
            }

            CurrentCar.ReplaceDetail(index, _storage.GiveDetail(sample).Clone());
        }

        public void EndRepair()
        {
            if (CurrentCar.IsBroken())
            {
                int penality = default;

                if (CurrentCar.IsChanged)
                {
                    penality = GetDynamicPenality();
                }
                else
                {
                    penality = FixedPenality;
                }

                Money -= penality;
            }

            _cars.Dequeue();
        }

        public bool HaveDetail(int index)
        {
            UserUtilits.HandleIndexExeptions(CurrentCar.Details, index);
            return _storage.HaveDetail(CurrentCar.Details[index].Clone());
        }

        public int GetDynamicPenality()
        {
            int penality = default;

            for (int i = 0; i < CurrentCar.Details.Count; i++)
            {
                if (CurrentCar.Details[i].IsBroken == true)
                {
                    penality += (int)(CurrentCar.Details[i].Price * DynamicPenalityMultiplyer);
                }
            }

            return penality;
        }
    }
}
