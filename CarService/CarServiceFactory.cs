using System;
using System.Collections.Generic;

namespace CarService
{
    public class CarServiceFactory
    {
        private readonly StorageFactory _storageFactory;
        private readonly int _detailsCount;

        public CarServiceFactory(List<DetailFactory> detailFactorys, int detailsCount)
        {
            ArgumentNullException.ThrowIfNull(detailFactorys);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(detailsCount);

            _storageFactory = new(detailFactorys);
            _detailsCount = detailsCount;
        }

        public CarService Create(int startMoney, int fixedPenality, float dynamicPenalityMultiplyer, int workPrice)
        {
            Storage storage = _storageFactory.Create(_detailsCount);
            return new CarService(storage, startMoney, fixedPenality, dynamicPenalityMultiplyer, workPrice);
        }
    }
}
