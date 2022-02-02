using Convey.WebApi.Exceptions;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Core.Exceptions;
using System;
using System.Net;

namespace Pacco.Services.Availability.Infrastructure.Exceptions
{
    internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception)
            => exception switch
            {
                DomainExceptionBase ex => new ExceptionResponse(new { code = ex.Code, reason = ex.Message }, 
                    HttpStatusCode.BadRequest),
                AppExceptionBase ex => new ExceptionResponse(new { code = ex.Code, reason = ex.Message }, 
                    HttpStatusCode.BadRequest),
                _ => new ExceptionResponse(new { code = "error", reason = "There was an error." }, 
                    HttpStatusCode.InternalServerError)
            };
    }
}
