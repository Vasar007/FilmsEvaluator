﻿using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Provides methods to create service classes (async) instances with parameters from XML 
    /// config.
    /// </summary>
    public sealed class ServiceAsyncBuilderForXmlConfig : ServiceBuilderBase
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ServiceAsyncBuilderForXmlConfig>();


        /// <summary>
        /// Creates service (async) builder to interact with XML config.
        /// </summary>
        public ServiceAsyncBuilderForXmlConfig()
        {
        }

        /// <summary>
        /// Creates message (sequential) handler instance depend on parameter value (could be get 
        /// from config).
        /// </summary>
        /// <param name="messageHandlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="messageHandlerElement" /> isn't specified in XML config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandlerElement" /> or its attribute with handler name is 
        /// <c>null</c>.
        /// </exception>
        public Communication.IMessageHandler CreateMessageHandler(XElement messageHandlerElement)
        {
            messageHandlerElement.ThrowIfNull(nameof(messageHandlerElement));
            var handlerElement = messageHandlerElement.Attribute(_messageHandlerTypeParameterName);
            handlerElement.ThrowIfNull(nameof(handlerElement));

            switch (handlerElement.Value)
            {
                case _consoleMessageHandlerParameterName:
                {
                    var messageHandlerParametersElement = XDocumentParser.FindSubelement(
                        messageHandlerElement, _setUnicodeParameterName
                    );
                    var setUnicode = XDocumentParser.GetElementValue<bool>(
                        messageHandlerParametersElement
                    );

                    return new Communication.ConsoleMessageHandler(setUnicode);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(messageHandlerElement), messageHandlerElement,
                        "Couldn't recognize message handler type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{handlerElement.Value}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates inputter (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="inputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Input.IInputterAsync CreateInputter(XElement inputterElement)
        {
            inputterElement.ThrowIfNull(nameof(inputterElement));

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.IFileReaderAsync fileReader = CreateFileReaderAsync(fileReaderName);

                    return new IO.Input.LocalFileReaderAsync(fileReader);
                }

                case _googleDriveParameterName:
                {
                    throw new NotImplementedException("Now GoogleDrive isn't supported.");
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(inputterElement), inputterElement,
                        "Couldn't recognize input type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{inputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates crawler (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="crawlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlerElement" /> is <c>null</c>.
        /// </exception>
        public Crawlers.CrawlerAsync CreateCrawler(XElement crawlerElement)
        {
            crawlerElement.ThrowIfNull(nameof(crawlerElement));

            switch (crawlerElement.Name.LocalName)
            {
                case _tmdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _tmdbApiKeyParameterName
                    );
                    var maxRetryCount = XDocumentParser.GetAttributeValue<int>(
                        crawlerElement, _tmdbMaxRetryCountParameterName
                    );

                    return new Crawlers.TmdbCrawlerAsync(apiKey, maxRetryCount);
                }

                case _omdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _omdbApiKeyParameterName
                    );

                    return new Crawlers.OmdbCrawlerAsync(apiKey);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _steamApiKeyParameterName
                    );

                    return new Crawlers.SteamCrawlerAsync(apiKey);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(crawlerElement), crawlerElement,
                        "Couldn't recognize crawler type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{crawlerElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates appraiser (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="appraiserElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiserElement" /> is <c>null</c>.
        /// </exception>
        public Appraisers.AppraiserAsync CreateAppraiser(XElement appraiserElement)
        {
            appraiserElement.ThrowIfNull(nameof(appraiserElement));

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTmdbParameterName:
                {
                    return new Appraisers.TmdbAppraiserAsync();
                }

                case _fuzzyAppraiserTmdbParameterName:
                {
                    return new Appraisers.FuzzyTmdbAppraiserAsync();
                }

                case _appraiserOmdbParameterName:
                {
                    return new Appraisers.OmdbAppraiserAsync();
                }

                case _steamAppraiserParameterName:
                {
                    return new Appraisers.SteamAppraiserAsync();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(appraiserElement), appraiserElement,
                        "Couldn't recognize appraiser type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{appraiserElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates outputter (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="outputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="outputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Output.IOutputterAsync CreateOutputter(XElement outputterElement)
        {
            outputterElement.ThrowIfNull(nameof(outputterElement));

            switch (outputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.CLocalFileWriterAsync();
                }

                case _googleDriveParameterName:
                {
                    throw new NotImplementedException("Now GoogleDrive isn't supported.");
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(outputterElement), outputterElement,
                        "Couldn't recognize output type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{outputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }
    }
}