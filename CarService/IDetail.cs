namespace CarService
{
    public interface IDetail
    {
        public string Name { get; }

        public int Price { get; }

        public bool IsBroken { get; }

        public Detail Clone();
    }
}
