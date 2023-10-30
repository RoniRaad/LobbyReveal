using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LobbyReveal.Core.Exceptions
{
    public class RiotClientNotFoundException : Exception
    {
        public RiotClientNotFoundException()
        {
        }

        public RiotClientNotFoundException(string directory) : base($"Riot client was not found in directory {directory}.")
        {
        }

        public RiotClientNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RiotClientNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }
    }
}
