﻿using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    public class ServiceProxy : IDisposable
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ServiceProxy>();

        private readonly string _baseAddress;

        private readonly string _apiUrl;

        private readonly HttpClient _client;

        private bool _disposedValue;


        public ServiceProxy()
        {
            _baseAddress = ConfigurationManager.AppSettings["ThingAppraiserServiceBaseAddress"];
            _apiUrl = ConfigurationManager.AppSettings["ThingAppraiserServiceApiUrl"];

            _logger.Info($"ThingAppraiser service url: {_baseAddress}");

            _client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public async Task<ProcessingResponse> SendPostRequest(RequestParams requestParams)
        {
            _logger.Info("Service method 'PostInitialRequest' is called.");

            using (var response = await _client.PostAsJsonAsync(_apiUrl, requestParams))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                    return result;
                }
                return null;
            }
        }

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