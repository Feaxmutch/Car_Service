using System;
using System.Collections.Generic;

namespace CarService
{
    public class StorageFactory
    {
        private readonly List<DetailFactory> _factorys;

        public StorageFactory(List<DetailFactory> detailSamples)
        {
            _factorys = detailSamples;
        }

        public Storage Create(int detailsCount)
        {
            List<IDetail> details = new();

            for (int i = 0; i < detailsCount; i++)
            {
                foreach (var factory in _factorys)
                {
                    details.Add(factory.Create());
                }
            }

            return new Storage(details);
        }
    }
}
