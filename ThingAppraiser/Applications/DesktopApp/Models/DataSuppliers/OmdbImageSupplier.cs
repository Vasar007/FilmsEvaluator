﻿using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal class OmdbImageSupplier : IImageSupplier
    {
        public OmdbImageSupplier()
        {
        }

        #region IImageSupplier Implementation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            if (!(data is OmdbMovieInfo movieInfo))
            {
                throw new ArgumentException("Data handler has invalid type.", nameof(data));
            }

            return movieInfo.PosterPath;
        }

        #endregion
    }
}