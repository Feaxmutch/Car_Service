namespace CarService
{
    public class Detail : IDetail
    {
        public Detail(string name, int price)
        {
            Name = name;
            Price = price;
            IsBroken = false;
        }

        public string Name { get; }

        public int Price { get; }

        public bool IsBroken { get; private set; }

        public void TakeDamage()
        {
            IsBroken = true;
        }

        public Detail Clone()
        {
            Detail detail = new(Name, Price);

            if (IsBroken)
            {
                detail.TakeDamage();
            }

            return detail;
        }

        public bool Equals(Detail detail, bool isIgnoreDamage = false)
        {
            bool isEquals = (detail == null) == false && detail.Name == Name && detail.Price == Price;

            if (isIgnoreDamage == false && isEquals)
            {
                isEquals = detail.IsBroken == IsBroken;
            }

            return isEquals;
        }
    }
}
