// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Emitter.Network.Native;
using Emitter.Network.Threading;

namespace Emitter.Network
{
    internal interface IAsyncDisposable
    {
        Task DisposeAsync();
    }

    /// <summary>
    /// Base class for listeners in Emitter. Listens for incoming connections
    /// </summary>
    internal abstract class Listener : ListenerContext, IListener, IAsyncDisposable
    {
        private bool _closed;

        protected Listener(ServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        /// <summary>
        /// Gets the listening socket handle.
        /// </summary>
        protected UvStreamHandle ListenSocket
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the listener in listening state on a particular end-point.
        /// </summary>
        /// <param name="address">End-point to listen to.</param>
        /// <param name="thread">The thread to use for listening.</param>
        public Task ListenAsync(ServiceAddress address, EventThread thread)
        {
            ServerAddress = address;
            Thread = thread;
            ConnectionManager = new ConnectionManager(thread);

            var tcs = new TaskCompletionSource<int>(this);

            Thread.Post(state =>
            {
                var tcs2 = (TaskCompletionSource<int>)state;
                try
                {
                    var listener = ((Listener)tcs2.Task.AsyncState);
                    listener.ListenSocket = listener.CreateListenSocket();
                    tcs2.SetResult(0);
                }
                catch (Exception ex)
                {
                    tcs2.SetException(ex);
                }
            }, tcs);

            return tcs.Task;
        }

        /// <summary>
        /// Creates the socket used to listen for incoming connections
        /// </summary>
        protected abstract UvStreamHandle CreateListenSocket();

        protected static void ConnectionCallback(UvStreamHandle stream, int status, Exception error, object state)
        {
            var listener = (Listener)state;

            if (error != null)
            {
                //listener.Log.LogError(0, error, "Listener.ConnectionCallback");
            }
            else if (!listener._closed)
            {
                listener.OnConnection(stream, status);
            }
        }

        /// <summary>
        /// Handles an incoming connection
        /// </summary>
        /// <param name="listenSocket">Socket being used to listen on</param>
        /// <param name="status">Connection status</param>
        protected abstract void OnConnection(UvStreamHandle listenSocket, int status);

        protected virtual void DispatchConnection(UvStreamHandle socket)
        {
            var connection = new Connection(this, socket);
            connection.Start();
        }

        public virtual async Task DisposeAsync()
        {
            // Ensure the event loop is still running.
            // If the event loop isn't running and we try to wait on this Post
            // to complete, then EmitterEngine will never be disposed and
            // the exception that stopped the event loop will never be surfaced.
            if (Thread.FatalError == null && ListenSocket != null)
            {
                await Thread.PostAsync(state =>
                {
                    var listener = (Listener)state;
                    listener.ListenSocket.Dispose();

                    listener._closed = true;

                    listener.ConnectionManager.WalkConnectionsAndClose();
                }, this).ConfigureAwait(false);

                await ConnectionManager.WaitForConnectionCloseAsync().ConfigureAwait(false);

                await Thread.PostAsync(state =>
                {
                    var writeReqPool = ((Listener)state).WriteReqPool;
                    while (writeReqPool.Count > 0)
                    {
                        writeReqPool.Dequeue().Dispose();
                    }
                }, this).ConfigureAwait(false);
            }

            Memory.Dispose();
            ListenSocket = null;
        }
    }
}