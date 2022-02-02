using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Core.Entietes;
using Pacco.Services.Availability.Core.ValueObjects;
using System;
using System.Linq;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Documents
{
    internal static class Extensions
    {
        /// <summary> 
        /// mapowanie na encje z dokumentu na nasz model dziedzinowy
        /// nie korzystamy z static factory tylko uzywamy normalnie konstruktora
        /// domain eventy są ulotne, w momencie w ktorym je odczytujemy nie mamy do nich dostepu
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Resource AsEntity(this ResourceDocument document)
            => new Resource(document.Id,
                    document.Tags,
                    document.Reservations.Select(r => new Reservation(r.TimeStamp.AsDateTime(), r.Priority)),
                    document.Version);

        /// <summary>
        /// mapowanie z encji na dokument bazodanowy
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static ResourceDocument AsDocument(this Resource resource)
        => new ResourceDocument
        {
            Id = resource.Id,
            Version = resource.Version,
            Tags = resource.Tags,
            Reservations = resource.Reservations.Select(r => new ReservationDocument
            {
                TimeStamp = r.DateTime.AsDaysSinceEpoch(),
                Priority = r.Priority
            })
        };

        /// <summary>
        /// Mapowanie ResourceDocument na ResourceDto
        /// </summary>
        public static ResourceDto AsDto(this ResourceDocument document)
        => new ResourceDto()
        {
            Id = document.Id,
            Tags = document.Tags ?? Enumerable.Empty<string>(),
            Reservations = document.Reservations?.Select(r => new ReservationDto()
            {
                DateTime = r.TimeStamp.AsDateTime(),
                Priority = r.Priority
            }) ?? Enumerable.Empty<ReservationDto>()
        };
        

        /// <summary>
        /// Przechodzenie z datetime na timestamp
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        internal static int AsDaysSinceEpoch(this DateTime dateTime)
            => (dateTime - new DateTime()).Days;

        /// <summary>
        /// Przechodzenie z timestamp na datetime
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        internal static DateTime AsDateTime(this int daysSinceEpoch)
            => new DateTime().AddDays(daysSinceEpoch);
    }
}
