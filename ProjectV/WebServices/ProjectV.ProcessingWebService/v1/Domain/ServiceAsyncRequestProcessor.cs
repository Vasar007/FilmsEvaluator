﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectV.IO.Input.WebService;
using ProjectV.IO.Output.WebService;
using ProjectV.Models.Internal;
using ProjectV.Models.WebService;
using ProjectV.TaskService;
using ProjectV.TmdbService;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public sealed class ServiceAsyncRequestProcessor : IServiceRequestProcessor
    {
        public ServiceAsyncRequestProcessor()
        {
        }

        #region IServiceRequestProcessor Implementation

        public async Task<ProcessingResponse> ProcessRequest(RequestData requestData)
        {
            var inputTransmitter = new InputTransmitterAsync(requestData.ThingNames);
            var outputTransmitter = new OutputTransmitterAsync();

            var simpleTask = new SimpleTask(
                id:               Guid.NewGuid(),
                executionsNumber: 1,
                delayTime:        TimeSpan.Zero
            );

            ServiceStatus status = (
                await simpleTask.ExecuteAsync(requestData, inputTransmitter, outputTransmitter)
            ).Single();

            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results =
                outputTransmitter.GetResults();

            var response = new ProcessingResponse
            {
                Metadata = new ResponseMetadata
                {
                    CommonResultsNumber = results.Sum(rating => rating.Count),
                    CommonResultCollectionsNumber = results.Count,
                    ResultStatus = status,
                    OptionalData = CreateOptionalData()
                },
                RatingDataContainers = results
            };
            return response;
        }

        #endregion

        private IReadOnlyDictionary<string, IOptionalData> CreateOptionalData()
        {
            var result = new Dictionary<string, IOptionalData>();
            if (TmdbServiceConfiguration.HasValue)
            {
                result.Add(nameof(TmdbServiceConfiguration),
                           TmdbServiceConfiguration.Configuration);
            }

            return result;
        }
    }
}
