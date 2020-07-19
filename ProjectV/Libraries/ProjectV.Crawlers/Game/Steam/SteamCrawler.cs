﻿using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Communication;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.SteamService;
using ProjectV.SteamService.Models;

namespace ProjectV.Crawlers.Game.Steam
{
    /// <summary>
    /// Concrete crawler for Steam service.
    /// </summary>
    public sealed class SteamCrawler : ICrawler, ICrawlerBase, IDisposable, ITagable, ITypeId
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SteamCrawler>();

        /// <summary>
        /// Adapter class to make a calls to Steam API.
        /// </summary>
        private readonly ISteamApiClient _steamApiClient;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(SteamCrawler);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(SteamGameInfo);

        #endregion


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to Steam service.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="apiKey" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> presents empty strings or contains only whitespaces.
        /// </exception>
        public SteamCrawler(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _steamApiClient = SteamApiClientFactory.CreateClient(apiKey);
        }

        #region ICrawler Implementation

        /// <inheritdoc />
        public IReadOnlyList<BasicInfo> GetResponse(IReadOnlyList<string> entities,
            bool outputResults)
        {
            if (SteamAppsStorage.IsEmpty)
            {
                SteamBriefInfoContainer steamApps = _steamApiClient.GetAppListAsync().Result;
                SteamAppsStorage.FillStorage(steamApps);
            }

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            foreach (string game in entities)
            {
                int? appId = SteamAppsStorage.TryGetAppIdByName(game);

                if (!appId.HasValue)
                {
                    string message = $"{game} was not find in Steam responses storage.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                var response = _steamApiClient.TryGetSteamAppAsync(
                    appId.Value, SteamCountryCode.Russia, SteamResponseLanguage.English
                ).Result;

                if (response is null)
                {
                    string message = $"{game} was not processed.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {response} from \"{Tag}\".");
                }

                searchResults.Add(response);
            }
            return searchResults.ToList();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Releases resources of TMDb client.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _steamApiClient.Dispose();

            _disposed = true;
        }

        #endregion
    }
}
