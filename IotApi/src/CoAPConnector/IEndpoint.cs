﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoAPConnector
{
    public class CoapPayload
    {
        public virtual int MessageId { get; set; }
        public virtual byte[] Payload { get; set; }
        public virtual ICoapEndpoint m_Endpoint { get; set; }
    }

    // Provided by Application Layer
    public interface ICoapEndpoint
    {
        /// <summary>
        /// Gets if this enpoint used for Multicast.
        /// </summary>
        /// <remarks>
        /// Multicast endpoitns do not acknolweged received confirmables.
        /// </remarks>
        bool IsMulticast { get; }

        /// <summary>
        /// Called by [Service] to send a <see cref="CoapPayload.Payload"/> to the specified <see cref="CoapPayload.m_Endpoint"/> using the transport layer provided by the Application Layer
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task SendAsync(CoapPayload payload);

        /// <summary>
        /// Called by [service] to receive data from the transport layer
        /// </summary>
        /// <returns></returns>
        Task<CoapPayload> ReceiveAsync();
    }
}
