using System;

namespace Pacco.Services.Availability.Application.Exceptions
{
    public abstract class AppExceptionBase : Exception
    {
        public abstract string Code { get; }

        protected AppExceptionBase(string message) : base(message)
        {
        }
    }
}
