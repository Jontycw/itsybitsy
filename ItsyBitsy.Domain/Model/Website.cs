﻿using System;
using System.Collections.Generic;
using System.Text;

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
    }
}