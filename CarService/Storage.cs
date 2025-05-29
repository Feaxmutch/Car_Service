namespace CarService
{
    public class Storage
    {
        private List<IDetail> _details;

        public Storage(List<IDetail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            if (DetailUtilits.HaveBrokenDetails(details))
            {
                throw new ArgumentException("Storage can't have broken details");
            }

            _details = details;
        }

        public bool HaveDetail(Detail sample)
        {
            ArgumentNullException.ThrowIfNull(sample);

            return DetailUtilits.TryGetDetail(_details, sample, out IDetail _, true);
        }

        public IDetail GiveDetail(Detail sample)
        {
            ArgumentNullException.ThrowIfNull(sample);

            if (DetailUtilits.TryGetDetail(_details, sample, out IDetail detail, true) == false)
            {
                throw new ArgumentException("Storage not have detail");
            }

            _details.Remove(detail);
            return detail;
        }
    }
}
