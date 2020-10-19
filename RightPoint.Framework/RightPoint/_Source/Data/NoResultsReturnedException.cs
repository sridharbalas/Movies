using System;
using System.Runtime.Serialization;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for NoResultsReturnedException.
    /// </summary>
    [Serializable]
    public class NoResultsReturnedException : RightPointException
    {
        public NoResultsReturnedException()
            : base()
        {
        }

        public NoResultsReturnedException( string message )
            : base( message )
        {
        }

        public NoResultsReturnedException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public NoResultsReturnedException( PriorityType priority, string message )
            : base( priority, message )
        {
        }

        public NoResultsReturnedException( PriorityType priority, string message, Exception innerException )
            : base( priority, message, innerException )
        {
        }

        protected NoResultsReturnedException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

    }
}