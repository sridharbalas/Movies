using System;
using System.Runtime.Serialization;

namespace RightPoint
{
    /// <summary>
    /// Summary description for RightPointException.
    /// </summary>
    [Serializable()]
    public class RightPointException : Exception
    {
        public enum PriorityType
        {
            Critical,
            Warning,
            Information
        }

        private PriorityType _priority;

        /// <summary>
        /// The Priority of the exception
        /// </summary>
        public PriorityType Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        public RightPointException()
        {
            _priority = PriorityType.Critical;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RightPointException( string message )
            : base( message )
        {
            _priority = PriorityType.Critical;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RightPointException( string message, Exception innerException )
            : base( message, innerException )
        {
            _priority = PriorityType.Critical;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="message">The message.</param>
        public RightPointException( PriorityType priority, string message )
            : base( message )
        {
            _priority = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RightPointException( PriorityType priority, string message, Exception innerException )
            : base( message, innerException )
        {
            _priority = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RightPointException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected RightPointException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            _priority = PriorityType.Critical;
        }
    }
}