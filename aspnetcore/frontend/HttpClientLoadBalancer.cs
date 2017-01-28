using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend
{
    public class HttpClientLoadBalancer : IDisposable
    {
        private readonly HttpClient[] _clients;
        private readonly int[] _queuedRequests;

        public HttpClientLoadBalancer(int clients)
        {
            _clients = new HttpClient[clients];
            for (var i=0; i < _clients.Length; i++)
            {
                _clients[i] = new HttpClient();
            }

            _queuedRequests = new int[clients];
        }

        private int ShortestQueue()
        {
            var shortestQueue = 0;
            var shortestQueueLength = _queuedRequests[0];

            for (var i = 1; i < _clients.Length; i++)
            {
                var queueLength = _queuedRequests[i];
                if (queueLength < shortestQueueLength)
                {
                    shortestQueue = i;
                    shortestQueueLength = queueLength;
                }
            }

            return shortestQueue;
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _clients[ShortestQueue()].PostAsync(requestUri, content);
        }

        public void Dispose()
        {
            for (var i=0; i < _clients.Length; i++)
            {
                _clients[i].Dispose();
            } 
        }
    }
}
