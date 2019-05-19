﻿using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Represents class which uses Fuzzy Logic Controller to calculate rating.
    /// </summary>
    public class FuzzyTmdbAppraiser : MoviesAppraiser
    {
        /// <summary>
        /// Fuzzy Controller which binds .NET interface with MATLAB module.
        /// </summary>
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override string Tag { get; } = "FuzzyTmdbAppraiser";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Fuzzy logic rating";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public FuzzyTmdbAppraiser()
        {
        }

        #region MoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>This appraiser uses MATLAB module to calculate ratings.</remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawDataContainer" /> contains instances of invalid type for this 
        /// appraiser.
        /// </exception>
        public override ResultList GetRatings(RawDataContainer rawDataContainer, bool outputResults)
        {
            CheckRatingId();

            var ratings = new ResultList();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            // Check if list have proper type.
            if (!rawData.All(e => e is TmdbMovieInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeId.FullName}"
                );
            }

            var converted = rawData.Select(e => (TmdbMovieInfo) e);
            foreach (TmdbMovieInfo entityInfo in converted)
            {
                double ratingValue = _fuzzyController.CalculateRating(
                    entityInfo.VoteCount, entityInfo.VoteAverage, entityInfo.ReleaseDate.Year,
                    entityInfo.Popularity, entityInfo.Adult ? 1 : 0
                );

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}
