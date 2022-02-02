namespace Pacco.Services.Availability.Infrastructure.Mongo.Documents
{
    internal sealed class ReservationDocument
    {
        //mongo przetrzymuje date jako int
        public int TimeStamp { get; set; }
        public int Priority { get; set; }
    }
}
