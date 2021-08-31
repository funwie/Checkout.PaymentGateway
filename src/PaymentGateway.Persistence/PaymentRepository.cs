using Microsoft.Extensions.Options;
using PaymentGateway.Domain;
using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Persistence.DataModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Persistence
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly SQLiteAsyncConnection _databaseAsync;

        public PaymentRepository(IOptions<DatabaseSettings> options)
        {
            var databaseSettings = options.Value;
            _databaseAsync = new SQLiteAsyncConnection(databaseSettings.ConnectionString);

            _databaseAsync.CreateTableAsync<PaymentDataModel>();
            _databaseAsync.CreateTableAsync<MerchantDataModel>();
            _databaseAsync.CreateTableAsync<ShopperDataModel>();
            _databaseAsync.CreateTableAsync<PaymentSourceDataModel>();
            _databaseAsync.CreateTableAsync<PaymentDestinationDataModel>();
            _databaseAsync.CreateTableAsync<CardDataModel>();
            _databaseAsync.CreateTableAsync<AcquirerResultDataModel>();
            _databaseAsync.CreateTableAsync<TransactionDataModel>();
        }

        public async Task Add(Payment payment)
        {
            var merchant = await GetMerchantById(payment.Merchant.Id);
            try
            {
                await _databaseAsync.RunInTransactionAsync(tran =>
                {
                    if (merchant is null)
                    {
                        tran.Insert(Map(payment.Merchant));
                    }
                    
                    tran.Insert(Map(payment.Destination));
                    tran.Insert(Map(payment.Shopper));
                    tran.Insert(Map(payment.Source.Card));
                    tran.Insert(Map(payment.Source));
                    tran.Insert(Map(payment));
                    tran.Insert(Map(payment.AcquirerResult, payment.Id));
                    tran.InsertAll(Map(payment.Transactions, payment.Id));
                });
            }
            catch (Exception exception)
            {
                throw new RepositoryException("Failed to add payment aggregate", exception);
            }
        }

        public async Task Update(Payment payment)
        {
            await _databaseAsync.UpdateAsync(payment);
        }

        public async Task<Payment> GetById(Guid id, CancellationToken cancellationToken)
        {
            var paymentIdToSearch = id.ToString();
            var payment = await _databaseAsync.Table<PaymentDataModel>()
                    .Where(payment => payment.Id == paymentIdToSearch).FirstOrDefaultAsync();

            if (payment is null) throw new RepositoryNotFoundException();

            var merchantModel = await _databaseAsync.GetAsync<MerchantDataModel>(payment.MerchantId);
            var shopperModel = await _databaseAsync.GetAsync<ShopperDataModel>(payment.ShopperId);
            var paymentSourceModel = await _databaseAsync.GetAsync<PaymentSourceDataModel>(payment.SourceId);
            var cardModel = await _databaseAsync.GetAsync<CardDataModel>(paymentSourceModel.CardId);
            var paymentDestinationModel = await _databaseAsync.GetAsync<PaymentDestinationDataModel>(payment.DestinationId);

            var acquirerResultDataModel = await _databaseAsync.Table<AcquirerResultDataModel>()
                .Where(result => result.PaymentId == payment.Id).FirstOrDefaultAsync();
            
            var transactions = await _databaseAsync.Table<TransactionDataModel>()
                .Where(transaction => transaction.PaymentId == payment.Id).ToListAsync();

            Enum.TryParse(payment.Currency, out SupportedCurrency supportedCurrency);
            Enum.TryParse(payment.Type, out PaymentType paymentType);
            var paymentAggregate = new Payment(Guid.Parse(payment.Id),
                payment.Amount,
                payment.Approved,
                payment.Description,
                payment.Reference,
                supportedCurrency,
                paymentType,
                payment.Status,
                payment.RequestedOn,
                Map(paymentSourceModel, cardModel),
                Map(paymentDestinationModel),
                Map(merchantModel),
                Map(shopperModel),
                Map(acquirerResultDataModel),
                Map(transactions));

            return paymentAggregate;
        }

        private async Task<MerchantDataModel> GetMerchantById(Guid merchantId)
        {
            var merchantIdToSearch = merchantId.ToString();
            return await _databaseAsync.Table<MerchantDataModel>()
                .Where(merchant => merchant.Id == merchantIdToSearch).FirstOrDefaultAsync();
        }

        private static Merchant Map(MerchantDataModel merchantModel)
        {
            return merchantModel is null ? null : new Merchant(Guid.Parse(merchantModel.Id), merchantModel.Name);
        }

        private static Shopper Map(ShopperDataModel shopper)
        {
            if (shopper is null) return null;

            var contact = new Contact(shopper.Phone, shopper.Email);
            var shippingAddress = new Address(shopper.HouseNumber, shopper.Line1, shopper.Line2, shopper.City, shopper.Postcode, shopper.Country);
            return new Shopper(Guid.Parse(shopper.Id), shopper.Name, shopper.Reference, contact, shippingAddress);
        }

        private static PaymentSource Map(PaymentSourceDataModel source, CardDataModel card)
        {
            if (source is null) return null;

            var billingAddress = new Address(card.HouseNumber, 
                                             card.Line1, 
                                             card.Line2, 
                                             card.City, 
                                             card.Postcode,
                                             card.Country);

            var sourceCard = new Card(Guid.Parse(card.Id), 
                                      card.FullName, 
                                      card.CompanyName, 
                                      card.CardNumber, 
                                      card.Cvv,
                                      card.ExpiryMonth, 
                                      card.ExpiryYear, 
                                      billingAddress);

            Enum.TryParse(source.Type, out PaymentSourceType sourceType);
            return new PaymentSource(Guid.Parse(source.Id), sourceType, sourceCard, Guid.Parse(source.ShopperId));
        }

        private static PaymentDestination Map(PaymentDestinationDataModel destination)
        {
            if (destination is null) return null;

            var bankAccount = new BankAccount(destination.SortCode, destination.AccountNumber, destination.BankName);
            Enum.TryParse(destination.Type, out PaymentDestinationType destinationType);
            return new PaymentDestination(Guid.Parse(destination.Id),
                                          destinationType, 
                                          bankAccount,
                                          Guid.Parse(destination.MerchantId));
        }

        private static AcquirerResult Map(AcquirerResultDataModel model)
        {
            if (model is null) return null;

            return new AcquirerResult(model.Name, model.Approved, model.Reference, model.Status, model.PerformedOn, model.Amount);
        }

        private static List<Transaction> Map(List<TransactionDataModel> transactions)
        {
            return transactions.Select(transaction => new Transaction(Guid.Parse(transaction.Id),
                transaction.Amount,
                transaction.Approved,
                transaction.Type,
                transaction.Reference,
                transaction.PerformedAt)).ToList();
        }

        private static PaymentDataModel Map(Payment payment)
        {
            if (payment is null) return null;

            return new PaymentDataModel
            {
                Id = payment.Id.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency.ToString(),
                Type = payment.Type.ToString(),
                SourceId = payment.Source.Id.ToString(),
                DestinationId = payment.Destination.Id.ToString(),
                MerchantId = payment.Merchant.Id.ToString(),
                ShopperId = payment.Shopper.Id.ToString(),
                Description = payment.Description,
                Reference = payment.Reference,
                Approved = payment.Approved,
                Status = payment.Status,
                RequestedOn = payment.RequestedOn
            };
        }

        private static MerchantDataModel Map(Merchant merchant)
        {
            if (merchant is null) return null;

            return new MerchantDataModel
            {
                Id = merchant.Id.ToString(),
                Name = merchant.Name
            };
        }

        private static ShopperDataModel Map(Shopper shopper)
        {
            if (shopper is null) return null;

            return new ShopperDataModel
            {
                Id = shopper.Id.ToString(),
                Name = shopper.Name,
                Reference = shopper.Reference,
                Phone = shopper.Contact?.Phone,
                Email = shopper.Contact?.Email,
                HouseNumber = shopper.ShippingAddress.HouseNumber,
                Line1 = shopper.ShippingAddress?.Line1,
                Line2 = shopper.ShippingAddress?.Line2,
                City = shopper.ShippingAddress?.City,
                Postcode = shopper.ShippingAddress?.Postcode,
                Country = shopper.ShippingAddress?.Country
            };
        }

        private static PaymentSourceDataModel Map(PaymentSource source)
        {
            if (source is null) return null;

            return new PaymentSourceDataModel
            {
                Id = source.Id.ToString(),
                Type = source.Type.ToString(),
                CardId = source.Card.Id.ToString(),
                ShopperId = source.ShopperId.ToString()
            };
        }

        private static PaymentDestinationDataModel Map(PaymentDestination destination)
        {
            if (destination is null) return null;

            return new PaymentDestinationDataModel
            {
                Id = destination.Id.ToString(),
                Type = destination.Type.ToString(),
                AccountNumber = destination.BankAccount?.AccountNumber,
                SortCode = destination.BankAccount?.SortCode,
                BankName = destination.BankAccount?.BankName,
                MerchantId = destination.MerchantId.ToString()
            };
        }

        private static CardDataModel Map(Card card)
        {
            if (card is null) return null;

            return new CardDataModel
            {
                Id = card.Id.ToString(),
                FullName = card.FullName,
                CompanyName = card.CompanyName,
                CardNumber = card.CardNumber,
                Cvv = card.Cvv,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                MaskedCardNumber = card.MaskedCardNumber,
                MaskedCvv = card.MaskedCvv,
                HouseNumber = card.BillingAddress?.HouseNumber,
                Line1 = card.BillingAddress?.Line1,
                Line2 = card.BillingAddress?.Line2,
                City = card.BillingAddress?.City,
                Postcode = card.BillingAddress?.Postcode,
                Country = card.BillingAddress?.Country
            };
        }

        private static AcquirerResultDataModel Map(AcquirerResult acquirerResult, Guid paymentId)
        {
            if (acquirerResult is null) return null;

            return new AcquirerResultDataModel
            {
                Id = Guid.NewGuid().ToString(),
                PaymentId = paymentId.ToString(),
                Name = acquirerResult.Name,
                Approved = acquirerResult.Approved,
                Amount = acquirerResult.Amount,
                Reference = acquirerResult.Reference,
                Status = acquirerResult.Status,
                PerformedOn = acquirerResult.PerformedOn
            };
        }

        private static IEnumerable<TransactionDataModel> Map(IReadOnlyCollection<Transaction> transactions, Guid paymentId)
        {
            return transactions.Select(transaction => new TransactionDataModel
            {
                Id = transaction.Id.ToString(),
                PaymentId = paymentId.ToString(),
                Amount = transaction.Amount,
                Approved = transaction.Approved,
                Type = transaction.Type,
                Reference = transaction.Reference,
                PerformedAt = transaction.PerformedOn
            });
        }
    }
}
