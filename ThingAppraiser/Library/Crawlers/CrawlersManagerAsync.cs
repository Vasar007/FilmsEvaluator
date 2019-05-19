﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerAsync : IManager<CrawlerAsync>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CrawlersManagerAsync>();

        private readonly List<CrawlerAsync> _crawlersAsync = new List<CrawlerAsync>();

        private readonly bool _outputResults;


        public CrawlersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerAsync> Implementation

        public void Add(CrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        public bool Remove(CrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
        }

        #endregion

        public async Task<bool> CollectAllResponses(BufferBlock<string> entitiesQueue,
            IDictionary<Type, BufferBlock<BasicInfo>> responsesQueues, DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>(_crawlersAsync.Count);
            var consumers = new List<BufferBlock<string>>(_crawlersAsync.Count);

            foreach (CrawlerAsync crawlerAsync in _crawlersAsync)
            {
                var consumer = new BufferBlock<string>(options);
                var responseQueue = new BufferBlock<BasicInfo>(options);

                responsesQueues.Add(crawlerAsync.TypeId, responseQueue);
                producers.Add(crawlerAsync.GetResponse(consumer, responseQueue, _outputResults));
                consumers.Add(consumer);
            }

            Task<bool[]> taskStatuses = Task.WhenAll(producers);
            Task taskConsumers = Task.WhenAll(consumers.Select(consumer => consumer.Completion));

            await Task.WhenAll(SplitQueue(entitiesQueue, consumers), taskConsumers, taskStatuses);

            bool[] statuses =  await taskStatuses;
            foreach (BufferBlock<BasicInfo> responseQueue in responsesQueues.Values)
            {
                responseQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info("Crawlers have finished work.");
                return true;
            }

            _logger.Info("Crawlers have not received any data.");
            return false;
        }

        private async Task SplitQueue(BufferBlock<string> entitiesQueue,
            IList<BufferBlock<string>> consumers)
        {
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string entity = await entitiesQueue.ReceiveAsync();

                await Task.WhenAll(
                    consumers.Select(async consumer => await consumer.SendAsync(entity))
                );
            }

            foreach (BufferBlock<string> consumer in consumers)
            {
                consumer.Complete();
            }
        }
    }
}
