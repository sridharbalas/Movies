using System;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for InvalidConnectionKeyException.
    /// </summary>
    [Serializable]
    public class InvalidConnectionKeyException : RightPointException
    {
        public InvalidConnectionKeyException() : base()
        {
        }

        public InvalidConnectionKeyException( string message ) : base( message )
        {
        }

        public InvalidConnectionKeyException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public InvalidConnectionKeyException( PriorityType priority, string message ) : base( priority, message )
        {
        }

        public InvalidConnectionKeyException( PriorityType priority, string message, Exception innerException )
            : base( priority, message, innerException )
        {
        }
    }
}