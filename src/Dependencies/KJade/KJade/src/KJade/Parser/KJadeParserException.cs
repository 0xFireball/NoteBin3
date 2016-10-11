using System;

namespace KJade.Parser
{
    public class KJadeParserException : Exception
    {
        [Obsolete]
        public KJadeParserException() : base()
        {
        }

        [Obsolete]
        public KJadeParserException(string message) : base(message)
        {
        }

        /// <summary>
        /// Throw a parser exception at a certain position in the code.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="position"></param>
        public KJadeParserException(string message, CodePosition position) : base($"({position.Line},{position.Column}) - {message}")
        {
        }
    }
}