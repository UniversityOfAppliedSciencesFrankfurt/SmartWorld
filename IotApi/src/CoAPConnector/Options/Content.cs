﻿using System;
using System.Collections.Generic;
using System.Text;
using CoAPConnector;

namespace CoAPConnector.Options
{
    /// <summary>
    /// Internet media types are identified by a string, such as
    /// "application/xml" [RFC2046].  In order to minimize the overhead of using these media types to 
    /// indicate the format of payloads, a sub-registry for a subset of Internet media types
    /// are used in CoAP and are assigned a numeric identifier. The name of the sub-registry is "CoAP
    /// Content-Formats", within the "CoRE Parameters" registry.
    /// <para>See section 12.3 of [RFC7252]</para>
    /// </summary>
    public enum ContentFormatType
    {
        /// <summary>
        /// text/plain; charset=utf-8
        /// <para>[RFC2046] [RFC3676] [RFC5147]</para>
        /// </summary>
        TextPlain = 0,
        /// <summary>
        /// application/link-format
        /// <para>See [rfc6690]</para>
        /// </summary>
        ApplicationLinkFormat = 40,
        /// <summary>
        /// application/xml
        /// <para>[RFC3023]</para>
        /// </summary>
        ApplicationXml = 41,
        /// <summary>
        /// application/octet-stream
        /// <para>See [RFC2045] and [RFC2046]</para>
        /// </summary>
        ApplicationOctetStream = 42,
        /// <summary>
        /// application/exi
        /// <para>See [REC-exi-20140211]</para>
        /// </summary>
        ApplicationExi = 47,
        /// <summary>
        /// application/json
        /// <para>See [RFC7159]</para>
        /// </summary>
        ApplicationJson = 50,
        /// <summary>
        /// applicaiton/cbor
        /// <para>See [RFC7049]</para>
        /// </summary>
        ApplicationCbor = 60,
    }

    /// <summary>
    /// The Content-Format Option indicates the representation format of the message payload. The representation format is given as a numeric
    /// Content-Format identifier that is defined in the "CoAP Content-Formats" registry (Section 12.3 of [RFC7252]). 
    /// <para>See section 5.10.3 of [RFC7252]</para>
    /// </summary>
    public class ContentFormat : CoapOption
    {
        public ContentFormatType MediaType
        {
            get
            {
                return (ContentFormatType)ValueUInt;
            }
            set
            {
                ValueUInt = (uint)value;
            }
        }

        public ContentFormat() : base(optionNumber: RegisteredOptionNumber.ContentFormat, maxLength: 2, type: OptionType.UInt)
        {
            MediaType = ContentFormatType.TextPlain;
        }

        public ContentFormat(ContentFormatType type) : this()
        {
            MediaType = type;
        }
    }

    /// <summary>
    /// The CoAP Accept option can be used to indicate which Content-Format is acceptable to the m_client.
    /// The representation format is given as a numeric Content-Format identifier that is defined in the 
    /// "CoAP Content-Formats" registry (Section 12.3 of [RFC7252]).
    /// <para>See section 5.10.4 of [RFC7252]</para>
    /// </summary
    public class Accept : CoapOption
    {
        public ContentFormatType MediaType
        {
            get
            {
                return (ContentFormatType)ValueUInt;
            }
            set
            {
                ValueUInt = (uint)value;
            }
        }

        public Accept() : base(optionNumber: RegisteredOptionNumber.Accept, maxLength: 2, type: OptionType.UInt)
        {
            MediaType = ContentFormatType.TextPlain;
        }

        public Accept(ContentFormatType type) : this()
        {
            MediaType = type;
        }
    }

    /// <summary>
    /// The Max-Age Option indicates the maximum time a response may be
    /// cached before it is considered not fresh(see Section 5.6.1 of [RFC7252]).
    /// <para>The option value is an integer number of seconds between 0 and
    /// 2**32-1 inclusive(about 136.1 years). A default value of 60 seconds
    /// is assumed in the absence of the option in a response.</para>
    /// <para>The value is intended to be current at the time of transmission.
    /// Servers that provide resources with strict tolerances on the value of
    /// Max-Age SHOULD update the value before each retransmission.  (See also Section 5.7.1. of [RFC7252])</para>
    /// <para>See section 5.10.5 of [RFC7252]</para>
    /// </summary>
    public class MaxAge : CoapOption
    {
        public MaxAge() : base(optionNumber: RegisteredOptionNumber.MaxAge, maxLength: 4, type: OptionType.UInt, defaultValue: 60u)
        {
            ValueUInt = 0u;
        }

        public MaxAge(uint value) : this()
        {
            ValueUInt = value;
        }
    }

    /// <summary>
    /// An entity-tag is intended for use as a resource-local identifier for
    /// differentiating between representations of the same resource that
    /// vary over time.It is generated by the server providing the
    /// resource, which may generate it in any number of ways including a
    /// version, checksum, hash, or time.An endpoint receiving an entity-
    /// tag MUST treat it as opaque and make no assumptions about its content
    /// or structure.  (Endpoints that generate an entity-tag are encouraged
    /// to use the most compact representation possible, in particular in
    /// regards to clients and intermediaries that may want to store multiple
    /// ETag values.)
    /// <para>See section 5.10.6 of [RFC7252]</para>
    /// </summary>
    /// Todo: Implement ETag request/response semantics as descripbed in section 5.10.6.1 and 5.10.6.2 of [RFC7252]
    public class ETag : CoapOption
    {
        public ETag() : base(optionNumber: RegisteredOptionNumber.ETag, minLength: 1, maxLength: 8, isRepeatable: true, type: OptionType.Opaque)
        {
            ValueOpaque = null;
        }

        public ETag(byte[] value) : this()
        {
            ValueOpaque = value;
        }
    }

    /// <summary>
    /// The Size1 option provides size information about the resource
    /// representation in a request.The option value is an integer number
    /// of bytes.Its main use is with block-wise transfers [BLOCK].  In the
    /// present specification, it is used in 4.13 responses (Section 5.9.2.9)
    /// to indicate the maximum size of request entity that the server is
    /// able and willing to handle.
    /// <para>See section 5.10.9 of [RFC7252]</para>
    /// </summary>
    public class Size1 : CoapOption
    {
        public Size1() : base(optionNumber: RegisteredOptionNumber.Size1, maxLength: 4, type: OptionType.UInt)
        {
            ValueUInt = 9u;
        }

        public Size1(uint value) : this()
        {
            ValueUInt = value;
        }
    }
}
