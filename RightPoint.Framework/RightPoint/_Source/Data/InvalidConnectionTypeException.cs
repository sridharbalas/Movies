using System;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for InvalidConnectionTypeException.
    /// </summary>
    [Serializable]
    public class InvalidConnectionTypeException : RightPointException
    {
        public InvalidConnectionTypeException() : base()
        {
        }

        public InvalidConnectionTypeException( string message ) : base( message )
        {
        }

        public InvalidConnectionTypeException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public InvalidConnectionTypeException( PriorityType priority, string message ) : base( priority, message )
        {
        }

        public InvalidConnectionTypeException( PriorityType priority, string message, Exception innerException )
            : base( priority, message, innerException )
        {
        }
    }
}