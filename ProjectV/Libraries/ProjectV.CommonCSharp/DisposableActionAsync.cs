﻿using System;
using System.Threading.Tasks;
using Acolyte.Assertions;

namespace ProjectV
{
    public sealed class DisposableActionAsync : IAsyncDisposable
    {
        private readonly Func<Task> _onDisposeActionAsync;


        public DisposableActionAsync(
            Func<Task> onDisposeActionAsync)
        {
            _onDisposeActionAsync = onDisposeActionAsync.ThrowIfNull(nameof(onDisposeActionAsync));
        }

        #region IAsyncDisposable Implementation

        private bool _disposed;

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await _onDisposeActionAsync();

            _disposed = true;
        }

        #endregion
    }
}
