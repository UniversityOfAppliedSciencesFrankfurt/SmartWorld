namespace Modbus.Message
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Text_Messages;

    public class SlaveExceptionResponse : AbstractModbusMessage, IModbusMessage
    {
        private static readonly Dictionary<byte, string> _exceptionMessages = CreateExceptionMessages();

        public SlaveExceptionResponse()
        {
        }

        public SlaveExceptionResponse(byte slaveAddress, byte functionCode, byte exceptionCode)
            : base(slaveAddress, functionCode)
        {
            SlaveExceptionCode = exceptionCode;
        }

        public override int MinimumFrameSize
        {
            get { return 3; }
        }

        public byte SlaveExceptionCode
        {
            get { return MessageImpl.ExceptionCode.Value; }
            set { MessageImpl.ExceptionCode = value; }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            string msg = _exceptionMessages.ContainsKey(SlaveExceptionCode)
                ? _exceptionMessages[SlaveExceptionCode]
                : Exceptions_Resources.Unknown;

            return string.Format(
                CultureInfo.InvariantCulture,
                Exceptions_Resources.SlaveExceptionResponseFormat,
                Environment.NewLine,
                FunctionCode,
                SlaveExceptionCode,
                msg);
        }

        internal static Dictionary<byte, string> CreateExceptionMessages()
        {
            Dictionary<byte, string> messages = new Dictionary<byte, string>(9);

            messages.Add(1, Exceptions_Resources.IllegalFunction);
            messages.Add(2, Exceptions_Resources.IllegalDataAddress);
            messages.Add(3, Exceptions_Resources.IllegalDataValue);
            messages.Add(4, Exceptions_Resources.SlaveDeviceFailure);
            messages.Add(5, Exceptions_Resources.Acknowledge);
            messages.Add(6, Exceptions_Resources.SlaveDeviceBusy);
            messages.Add(8, Exceptions_Resources.MemoryParityError);
            messages.Add(10, Exceptions_Resources.GatewayPathUnavailable);
            messages.Add(11, Exceptions_Resources.GatewayTargetDeviceFailedToRespond);

            return messages;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (FunctionCode <= Modbus.ExceptionOffset)
            {
                throw new FormatException(Exceptions_Resources.SlaveExceptionResponseInvalidFunctionCode);
            }

            SlaveExceptionCode = frame[2];
        }
    }
}
