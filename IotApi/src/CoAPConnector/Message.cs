﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoAPConnector;

namespace CoAPConnector
{
    /// <summary>
    /// <see cref="CoapMessage.Type"/>
    /// </summary>
    public enum CoapMessageType
    {
        Confirmable = 0,
        NonConfirmable = 1,
        Acknowledgement = 2,
        Reset = 3,
    }

    /// <summary>
    /// Class pages used to indicate if a <see cref="CoapMessageCode"/> value is a Request, or a Response or an error.
    /// </summary>
    public enum CoapMessageCodeClass
    {
        Request = 0,
        Success = 200,
        ClientError = 400,
        ServerError = 500
    }

    /// <summary>
    /// Response Codes
    /// <para>See section 5.9 of [RFC7252] and section 12.1 of [RFC7252]</para>
    /// </summary>
    public enum CoapMessageCode
    {
        None = 0,
        // 0.xx Request
        Get = 1,
        Post = 2,
        Put = 3,
        Delete = 4,
        // 2.xx Success
        Created = 201,
        Deleted = 202,
        Valid = 203,
        Changed = 204,
        Content = 205,
        // 4.xx Client Error
        BadRequest = 400,
        Unauthorized = 401,
        BadOption = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        UnsupportedContentFormat = 415,
        // 5.xx Server Error
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        ProxyingNotSupported = 505
    }

    public class CoapMessageFormatException : Exception
    {
        public CoapMessageFormatException() : base() { }
        public CoapMessageFormatException(string message) : base(message) { }
        public CoapMessageFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CoapMessage
    {

        private int _version = 1;
        /// <summary>
        /// Gets or sets the protocol version. 
        /// As of [RFC7252], only version 1 is supported. any other value is reserved.
        /// </summary>
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (value != 1)
                    throw new ArgumentException("Only version 1 is supported");
                _version = value;
            }
        }

        /// <summary>
        /// Gets or Sets if the message should be responded to by the server. 
        /// <para>When set to <see cref="CoapMessageType.Reset"/>, this message indicates that a <see cref="CoapMessageType.Confirmable"/> message was rejected by the server endpoint.</para>
        /// <para>When set to <see cref="CoapMessageType.Acknowledgement"/>, this message indicates it was accepted (and responded) by the server enpoint.</para>
        /// </summary>
        public CoapMessageType Type { get; set; }

        /// <summary>
        /// Gets or Sets the Message Code. 
        /// <para>The class indicates if the message is <see cref="CoapMessageCodeClass.Request"/>, <see cref="CoapMessageCodeClass.Success"/>, <see cref="CoapMessageCodeClass.ClientError"/>, or a <see cref="CoapMessageCodeClass.ServerError"/></para>
        /// <para>See section 2.2 of [RFC7252]</para>
        /// </summary>
        public CoapMessageCode Code { get; set; }

        private byte[] _token = new byte[0];
        /// <summary>
        /// Gets or sets a opaque token used to correlate messages over multiple responses (i.e. when a reponse is not piggy-backed to the acknowledgement.
        /// This token may be any size up to 8 bytes long. When set to a zero length, it will not be serialised as part of the message.
        /// </summary>
        public byte[] Token
        {
            get
            {
                return _token;

            }
            set
            {
                if (value.Length > 8)
                    throw new ArgumentException("Token length can not be more than 8 bytes long");
                _token = value;
            }
        }

        /// <summary>
        /// Gets or Sets a Message ID to pair Requests to their immediate Responses.
        /// </summary>
        public int Id { get; set; }

        private List<CoapOption> _options = new List<CoapOption>();
        /// <summary>
        /// Gets or sets the list of options to be encoded into the message header. The order of these options are Critical and spcial care is needed when adding new items.
        /// <para>Todo: Sort items based on <see cref="CoapOption.OptionNumber"/> and preserve options with identical Optionnumbers</para>
        /// /// <para>Todo: Throw exception when non-repeatable <see cref="CoapOption"/>s are addedd</para>
        /// </summary>
        public List<CoapOption> Options
        {
            get { return _options; }
            set
            {
                _options = value;
                _options.Sort();
            }
        }

        /// <summary>
        /// Gets or Sets The paylaod of the message.
        /// </summary>
        /// <remarks>Check (or add) <see cref="Options.ContentFormat"/> in <see cref="CoapMessage.Options"/> for the format of the payload.</remarks>
        public byte[] Payload { get; set; }

        public readonly bool IsMulticast;

        public CoapMessage(bool multicast = false)
        {
            IsMulticast = multicast;
        }

        /// <summary>
        /// Serialises the message into bytes, ready to be encrypted or transported to the destination endpoint.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialise()
        {
            var result = new List<byte>();
            byte optCode = 0;

            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // |Ver| T |  TKL  |      Code     |           Message ID          |
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            var type = (byte)Type;
            result.Add((byte)(0x40 | ((type << 4) & 0x30) | _token.Length)); // Ver | T | TKL

            // +-+-+-+-+-+-+-+-+
            // |class|  detail | (See section 5.2 of [RFC7252])
            // +-+-+-+-+-+-+-+-+
            optCode = (byte)(((int)Code / 100) << 5); // Class
            optCode |= (byte)((int)Code % 100);       // Detail
            result.Add(optCode); // Code

            result.Add((byte)((Id >> 8) & 0xFF)); // Message ID (upper byte)
            result.Add((byte)(Id & 0xFF));        // Message ID (lower byte)

            // Empty messages must only contain a 4 byte header.
            if (Code == CoapMessageCode.None)
            {
                result[0] &= 0xF0; // Zero out the token length in case the application layer set one
                return result.ToArray();
            }

            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // | Token (if any, TKL bytes) ...
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            result.AddRange(_token);

            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // | Options (if any) ...
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            var currentOptionDelta = 0;

            _options.Sort();
            foreach (var option in _options)
            {
                var optionHeader = new List<byte>();
                int optionDelta = option.OptionNumber - currentOptionDelta;
                currentOptionDelta += optionDelta;

                if (optionDelta >= 269)
                {
                    optCode = 0xE0;
                    optionDelta -= 269;
                    optionHeader.Add((byte)((optionDelta & 0xFF00u) >> 8));
                    optionHeader.Add((byte)(optionDelta & 0xFFu));
                }
                else if (optionDelta >= 13)
                {
                    optCode = 0xD0;
                    optionDelta -= 13;
                    optionHeader.Add((byte)(optionDelta & 0xFFu));
                }
                else
                {
                    optCode = (byte)(optionDelta << 4);
                }

                optionDelta = option.Length;
                if (optionDelta >= 269)
                {
                    result.Add((byte)(optCode | 0x0E));
                    optionDelta -= 269;

                    result.AddRange(optionHeader);
                    result.Add((byte)((optionDelta & 0xFF00u) >> 8));
                    result.Add((byte)(optionDelta & 0xFFu));
                }
                else if (optionDelta >= 13)
                {
                    result.Add((byte)(optCode | 0x0D));
                    optionDelta -= 13;

                    result.AddRange(optionHeader);
                    result.Add((byte)(optionDelta & 0xFFu));
                }
                else
                {
                    result.Add((byte)(optCode | optionDelta));
                    result.AddRange(optionHeader);
                }

                result.AddRange(option.GetBytes());
            }

            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            // |1 1 1 1 1 1 1 1| Payload (if any) ...
            // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

            if (Payload != null && Payload.Length > 0)
            {
                result.Add(0xFF); // Payload marker
                result.AddRange(Payload);
            }

            return result.ToArray();
        }

        public void Deserialise(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length < 4)
                throw new CoapMessageFormatException("Message must be at least 4 bytes long");
            if ((data[0] & 0xC0) != 0x40)
                throw new CoapMessageFormatException("Only verison 1 of CoAP protocol is supported");

            var offset = 4;

            Type = (CoapMessageType)((data[0] & 0x30) >> 4);

            var code = ((data[1] & 0xE0) >> 5) * 100;
            code += data[1] & 0x1F;
            Code = (CoapMessageCode)code;

            Id = (ushort)((data[2] << 8) | (data[3]));

            // Don't process any further if this is a "empty" message
            if (Code == CoapMessageCode.None && data.Length > 4)
                throw new CoapMessageFormatException("Empty message must be 4 bytes long");

            if (new int[] { 1, 6, 7 }.Contains(code / 100))
                throw new CoapMessageFormatException("Message.Code can not use reserved classes");

            offset += data[0] & 0x0F;
            if ((data[0] & 0x0F) > 0)
                _token = data.Skip(4).Take(data[0] & 0x0F).ToArray();

            var optionDelta = 0;
            for (var i = offset; i < data.Length; i++)
            {
                // check for payload marker
                if (data[i] == 0xFF)
                {
                    Payload = data.Skip(i + 1).ToArray();
                    return;
                }

                var optCode = (data[i] & 0xF0) >> 4;
                var dataLen = (data[i] & 0x0F);

                if (optCode == 13)
                    optCode = data[i++ + 1] + 13;
                else if (optCode == 14)
                {
                    optCode = data[i++ + 1] << 8;
                    optCode |= data[i++ + 1] + 269;

                }
                if (dataLen == 13)
                    dataLen = data[i++ + 1];
                else if (dataLen == 14)
                {
                    dataLen = data[i++ + 1] << 8;
                    dataLen |= data[i++ + 1] + 269;
                }
                Options.Add(CoAPConnector.Options.Factory.Create(optCode + optionDelta, data.Skip(i + 1).Take(dataLen).ToArray()));
                i += dataLen;
                optionDelta += optCode;
            }


        }

        /// <summary>
        /// Shortcut method to create a <see cref="CoapMessage"/> with its optinos pre-populated to match the Uri.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static CoapMessage CreateFromUri(string input)
        {
            var message = new CoapMessage();
            message.FromUri(input);
            return message;
        }

        /// <summary>
        /// Popualtes <see cref="CoapMessage.Options"/> to match the Uri.
        /// </summary>
        /// <remarks>Any potentially conflicting <see cref="CoapOption"/>s are stripped after URI validation and before processing.</remarks>
        /// <param name="input"></param>
        public void FromUri(string input)
        {
            // Will throw exceptions that the application code can handle
            Uri uri = new Uri(input);

            if (!uri.IsAbsoluteUri)
                throw new UriFormatException("URI is not absolute and unsupported by the CoAP scheme");

            if (uri.Scheme != "coap" && uri.Scheme != "coaps")
                throw new UriFormatException("Input URI scheme is not coap:// or coaps://");

            if (uri.Fragment.Length > 0)
                throw new UriFormatException("Fragments are unsupported in the CoAP scheme");

            // Strip out any existing URI Options 
            var optionsToDiscard = new int[] { RegisteredOptionNumber.UriHost, RegisteredOptionNumber.UriPort, RegisteredOptionNumber.UriPath, RegisteredOptionNumber.UriQuery };
            _options = _options.Where(kv => !optionsToDiscard.Contains(kv.OptionNumber)).ToList();

            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    _options.Add(new Options.UriHost(uri.IdnHost));
                    break;
                case UriHostNameType.IPv4:
                case UriHostNameType.IPv6:
                    //_options.Add(new Options.UriHost { ValueString = uri.Host });
                    break;
                default:
                    throw new UriFormatException("Unknown Hostname");
            }

            if ((uri.Scheme == "coap" && !uri.IsDefaultPort && uri.Port != 5683) ||
                (uri.Scheme == "coaps" && !uri.IsDefaultPort && uri.Port != 5684))
                _options.Add(new Options.UriPort((ushort)uri.Port));

            _options.AddRange(uri.AbsolutePath.Substring(1).Split(new[] { '/' }).Select(p => new Options.UriPath(Uri.UnescapeDataString(p))));

            if (uri.Query.Length > 0)
                _options.AddRange(uri.Query.Substring(1).Split(new[] { '&' }).Select(p => new Options.UriQuery(Uri.UnescapeDataString(p))));
        }

        public override string ToString()
        {
            var result = Type == CoapMessageType.Acknowledgement ? "ACK" :
                         Type == CoapMessageType.Confirmable ? "CON" :
                         Type == CoapMessageType.NonConfirmable ? "NON" : "RST";

            result += ", MID:" + Id.ToString();

            if (Code <= CoapMessageCode.Delete)
            {
                result += ", " + Code.ToString();
            }
            else
            {
                result += string.Format(", {0}.{1:D2} {2}", ((int)Code / 100), ((int)Code % 100), Code);
            }

            if (Options.Any(o => o.OptionNumber == RegisteredOptionNumber.UriPath))
                result += ", /" + Options.Where(o => o.OptionNumber == RegisteredOptionNumber.UriPath).Select(o => o.ValueString).Aggregate((a, b) => a + "/" + b);

            return result;
        }
    }
}
