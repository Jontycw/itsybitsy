using System;

namespace ItsyBitsy.Domain
{
    public class Website
    {
        public Website(Data.Website website)
        {
            if (website == default)
                throw new ArgumentNullException($"{nameof(website)} is null when creating domain website.");

            Id = website.Id;
            Seed = new Uri(website.Seed);
        }

        public int Id { get; }
        public Uri Seed { get; }

        public override string ToString()
        {
            return Seed.ToString();
        }
    }
}
