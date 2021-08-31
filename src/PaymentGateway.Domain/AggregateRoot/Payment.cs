using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.Events;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.SeedWork;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Domain.AggregateRoot
{
    public class Payment : AggregateRoot<Guid>
    {
        public decimal Amount { get; private set; }
        public SupportedCurrency Currency { get; private set; }
        public PaymentType Type { get; private set; }
        public PaymentSource Source { get; private set; }
        public PaymentDestination Destination { get; private set; }
        public Merchant Merchant { get; private set; }
        public Shopper Shopper { get; private set; }
        public string Description { get; private set; }
        public string Reference { get; private set; }
        public bool? Approved { get; private set; }
        public string Status { get; private set; }
        public DateTime RequestedOn { get; private set; }
        public AcquirerResult AcquirerResult { get; private set; }
        public IReadOnlyCollection<Transaction> Transactions => _transactions;
        private readonly List<Transaction> _transactions;


    public bool IsCompleted => Status != PaymentStatus.Processing;

        public Payment(Guid id, 
                       decimal amount, 
                       SupportedCurrency currency, 
                       PaymentType type, 
                       string description, 
                       string reference)
        {
            base.Id = id;
            Amount = amount;
            Currency = currency;
            Type = type;
            Description = description;
            Reference = reference;
            RequestedOn = DateTime.UtcNow;
            Status = PaymentStatus.Processing;

            _transactions = new List<Transaction>();
        }

        public Payment(Guid id,
                        decimal amount,
                        bool? approved,
                        string description,
                        string reference,
                        SupportedCurrency currency,
                        PaymentType type,
                        string status,
                        DateTime requestedOn,
                        PaymentSource source,
                        PaymentDestination destination,
                        Merchant merchant,
                        Shopper shopper,
                        AcquirerResult acquirerResult,
                        List<Transaction> transactions = null)
        {
            base.Id = id;
            Amount = amount;
            Approved = approved;
            Description = description;
            Reference = reference;
            Currency = currency;
            Type = type;
            Status = status;
            RequestedOn = requestedOn;
            Source = source;
            Destination = destination;
            Merchant = merchant;
            Shopper = shopper;
            AcquirerResult = acquirerResult;
            _transactions = transactions ?? new List<Transaction>();
        }

        public void Complete(AcquirerResult acquirerResult)
        {
            if (IsCompleted)
                throw new InvalidOperationException("Payment already completed");

            Approved = acquirerResult.Approved;
            Status = acquirerResult.Status;
            AcquirerResult = acquirerResult;

            _transactions.Add(new Transaction(Guid.NewGuid(), Amount, Approved, Status,  acquirerResult.Reference, acquirerResult.PerformedOn));
            AddDomainEvent(new PaymentAuthorisedDomainEvent());
        }

        public void AddSource(PaymentSource source)
        {
            if (Source != null)
                throw new InvalidOperationException("Source already exist on payment");

            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public void AddMerchant(Merchant merchant)
        {
            if (Merchant != null)
                throw new InvalidOperationException("Merchant already exist on payment");

            Merchant = merchant ?? throw new ArgumentNullException(nameof(merchant));
        }

        public void AddShopper(Shopper shopper)
        {
            if (Shopper != null)
                throw new InvalidOperationException("Shopper already exist on payment");

            Shopper = shopper ?? throw new ArgumentNullException(nameof(shopper));
        }

        public void AddDestination(PaymentDestination destination)
        {
            if (Destination != null)
                throw new InvalidOperationException("Destination already exist on payment");

            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }
    }
}
