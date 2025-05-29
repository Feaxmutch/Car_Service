using System;
using System.Collections.Generic;

namespace CarService
{
    internal class DetailUtilits
    {
        public static bool TryGetDetail(ICollection<IDetail> details, IDetail sample, out IDetail result, bool isIgnoreDamage = false)
        {
            ArgumentNullException.ThrowIfNull(details);
            ArgumentNullException.ThrowIfNull(sample);

            result = default;

            foreach (var detail in details)
            {
                if (detail.Clone().Equals(sample.Clone(), isIgnoreDamage))
                {
                    result = detail;
                    return true;
                }
            }

            return false;
        }

        public static bool IsContainsSimilarDetails(List<IDetail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            for (int i = 0; i < details.Count; i++)
            {
                if (details.FindAll(detail => detail.Equals(details[i])).Count > 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HaveBrokenDetails(List<IDetail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            bool isHave = false;

            foreach (var detail in details)
            {
                if (detail.IsBroken)
                {
                    isHave = true;
                    break;
                }
            }

            return isHave;
        }
    }
}
