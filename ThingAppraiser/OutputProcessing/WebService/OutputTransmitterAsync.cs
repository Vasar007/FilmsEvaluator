﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output.WebService
{
    public class OutputTransmitterAsync : IOutputterAsync, IOutputterBase, ITagable
    {
        private List<List<RatingDataContainer>> _transmittingResults;

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "OutputTransmitter";

        #endregion


        public OutputTransmitterAsync()
        {
        }

        public List<List<RatingDataContainer>> GetResults()
        {
            if (_transmittingResults is null)
            {
                _transmittingResults = new List<List<RatingDataContainer>>();
            }
            return _transmittingResults ;
        }

        #region IOutputter Implementation

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> SaveResults(List<List<RatingDataContainer>> results,
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            string storageName)
        {
            StorageName = storageName;

            _transmittingResults = results;
            return true;
        }

        #endregion
    }
}