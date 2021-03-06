using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionSender
        : ITransmissionSender<EventData[]>
    {
        private readonly EventHubClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionSender"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmissionSender(EventHubClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc />
        public async Task SendAsync(IAsyncEnumerable<EventData[]> batches, CancellationToken cancellationToken)
        {
            if (batches == null)
            {
                throw new ArgumentNullException(nameof(batches));
            }

            await foreach(EventData[] batch in batches.WithCancellation(cancellationToken))
            {
                await _client.SendAsync(batch);
            }
        }
    }
}
