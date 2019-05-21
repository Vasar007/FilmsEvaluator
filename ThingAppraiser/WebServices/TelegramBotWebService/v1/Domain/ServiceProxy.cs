﻿using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Logging;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public class ServiceProxy : IServiceProxy, IDisposable
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ServiceProxy>();

        private readonly ServiceSettings _settings;

        private readonly HttpClient _client;

        private bool _disposedValue;


        public ServiceProxy(IOptions<ServiceSettings> settings)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));

            _logger.Info("ThingAppraiser service url: " +
                         $"{_settings.ThingAppraiserServiceBaseAddress}");

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.ThingAppraiserServiceBaseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        #region IServiceProxy Implementation

        public async Task<ProcessingResponse> SendPostRequest(RequestParams requestParams)
        {
            _logger.Info("Service method 'PostInitialRequest' is called.");

            using (var response = await _client.PostAsJsonAsync(
                       _settings.ThingAppraiserServiceApiUrl, requestParams
                   ))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                    return result;
                }
                return null;
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                _client.Dispose();
            }
        }

        #endregion
    }
}