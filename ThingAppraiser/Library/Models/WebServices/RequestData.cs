﻿using System.Collections.Generic;
using ThingAppraiser.Models.Configuration;

namespace ThingAppraiser.Models.WebService
{
    public sealed class RequestData
    {
        public List<string> ThingNames { get; set; }

        public ConfigurationXml ConfigurationXml { get; set; }


        public RequestData()
        {
        }
    }
}
