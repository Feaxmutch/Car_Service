namespace CarService
{
    public class Car
    {
        private readonly List<Detail> _details;

        public Car(List<Detail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            if (DetailUtilits.IsContainsSimilarDetails(new(details)))
            {
                throw new ArgumentException("Car can have only unique details");
            }

            _details = details;
            TakeRandomDamage();
            IsChanged = false;
        }

        public IReadOnlyList<IDetail> Details => _details;

        public bool IsChanged { get; private set; }

        public bool IsBroken()
        {
            return _details.Where(detail => detail.IsBroken == true).Count() > 0;
        }

        private void TakeRandomDamage()
        {
            float damageChance = 0.35f;

            foreach (var detail in _details)
            {
                if (UserUtilits.GetRandomBoolean(damageChance))
                {
                    detail.TakeDamage();
                }
            }

            if (IsBroken() == false)
            {
                UserUtilits.GetRandomObject(_details).TakeDamage();
            }
        }

        public void ReplaceDetail(int index, Detail detail)
        {
            ArgumentNullException.ThrowIfNull(detail);
            UserUtilits.HandleIndexExeptions<IDetail>(Details, index);

            if (_details[index].Equals(detail, true) == false)
            {
                throw new ArgumentException("incorrect detail for replace");
            }

            _details[index] = detail;
            IsChanged = true;
        }
    }
}
