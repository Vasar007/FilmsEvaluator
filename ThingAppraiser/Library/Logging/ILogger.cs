﻿using System;

namespace ThingAppraiser.Logging
{
    public interface ILogger
    {
        void PrintHeader(string starterMessage);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Warn(Exception ex, string message);
        void Error(string message);
        void Error(Exception ex, string message);
    }
}
