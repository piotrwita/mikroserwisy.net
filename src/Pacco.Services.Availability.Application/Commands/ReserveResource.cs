﻿using Convey.CQRS.Commands;
using System;

namespace Pacco.Services.Availability.Application.Commands
{
    public class ReserveResource : ICommand
    {
        //queries nie moga moga miec setow 
        public Guid ResourceId { get; }
        public DateTime DateTime { get; }
        public int Priority { get; }
        public Guid CustomerId { get; }

        public ReserveResource(Guid resourceId, DateTime dateTime, int priority, Guid customerId)
        {
            ResourceId = resourceId;
            DateTime = dateTime;
            Priority = priority;
            CustomerId = customerId;
        }
    }
}
