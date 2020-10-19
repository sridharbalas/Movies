using System;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for InvalidReturnCodeException.
    /// </summary>
    [Serializable]
    public class InvalidReturnCodeException : RightPointException
    {
        private Int32? _returnCode;

        public Int32? ReturnCode
        {
            get { return ( _returnCode ); }
        }

        public InvalidReturnCodeException( Int32? returnCode ) : base()
        {
            _returnCode = returnCode;
        }

        public InvalidReturnCodeException( Int32? returnCode, string message )
            : base( message )
        {
            _returnCode = returnCode;
        }

        public InvalidReturnCodeException( Int32? returnCode, string message, Exception innerException )
            : base( message, innerException )
        {
            _returnCode = returnCode;
        }

        public InvalidReturnCodeException( Int32? returnCode, PriorityType priority, string message )
            : base( priority, message )
        {
            _returnCode = returnCode;
        }

        public InvalidReturnCodeException( Int32? returnCode, PriorityType priority, string message,
                                           Exception innerException )
            : base( priority, message, innerException )
        {
            _returnCode = returnCode;
        }
    }
}