﻿using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input.File
{
    public interface IFileReaderAsync
    {
        Task ReadFile(BufferBlock<string> queue, string filename);

        Task ReadCsvFile(BufferBlock<string> queue, string filename);
    }
}