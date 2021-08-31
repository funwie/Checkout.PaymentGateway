## Payment Gateway API
A payment gateway API and a acquiring bank fake api simulator to fully test the payment flow

### Tools
- Requires C# and .Net 5, and SQLite
- Optional tools: Visual Studio or preferred IDE and Docker installed
- Tested using Nunit and XUnit frameworks

### Task 1a - Payment Request Endpoint
- Send a payment request with the payment details and include MerchantId, optional IdempotencyKey and optional RequestId headers.
- The MerchantId is a way to link payments to Merchants since Auth is not implemented for now. 
- Also, there is a fake merchant service that uses the merchantId to get a merchant's details (name and bank account) for the payment acquiring.
- Implementation assumes that multiple payment types require different payment acquirers. See Acquirer factory.
- The payment details, card details, and merchant bank account are sent to the acquirer.
- The Domain project is a basic implementation of DDD. It's not close to perfect lol.
- The IdempotencyKey was to prevent duplication payments, but feature is not implementated.
- The RequestId is to correlate the payment request as it is going through different services.
- The open api documentation will have more information about the usage of the endpoint
- Empty merchantId returns Unauthorized

### Task 1b - Retrieve a Payment Endpoint
- Send the paymentId and the merchantId to retrieve a payment.
- The merchantId is to ensure merchant retrieve only their payments. Simulates authentication.
- The open api documentation will have more information about the usage of the endpoint.
- Empty merchantId returns Unauthorized

### Task 2 - Simulate the Acquiring bank in integration tests
- Created a Checkout.ApiFaker library that is used in the integration tests to simulate the Acquiring bank api.
- Created an api sandbox that can be used for manual testing to simulate the Acquiring bank api.
- Created an SDK for the acquiring bank to use in the Application project. 

### Running Application

#### From Visual Studio or Rider
- Open the solution
- Set startup project to PaymentGateway
- Set Debug option to IIS Express
- Debug the application.
- Use the Swagger UI to try out the api

#### Terminal 
- Open the solution folder in a terminal
- Run `dotnet run --project src/PaymentGateway/PaymentGateway.csproj`
- Open the url to Swagger UI and try out the api

#### From Docker
- Open the solution folder in a terminal
- Run `docker build -f docker/Dockerfile -t payment_gateway .` 
- After image builds, run `docker run -dp 8000:80 --name payment_gateway-api payment_gateway`
- Access the payment gateway api from localhost:8000 and explore the API with the Swagger UI
- Health endpoint is at /healthcheck

#### Use the Acquiring bank sandbox to manual test
- Sandbox url `https://acquirer-bank-simulator.getsandbox.com`
- Change the cardAcquiringBank.BaseUrl in both appsettings.json and appsettings.Development.json to above.
- Amount of 100 will Decline the acquiring request.
- Amount of 500 will fail the acquiring request.
- Any other amount will Authorize the acquiring request.

#### Run Tests
- The Acquiring back fake api is used in the integration test.
- Ensure the cardAcquiringBank.BaseUrl config is set to expected fake acquiring bank host `http://localhost:8088`  
- Open the solution in VS and run all tests
- OR open the solution folder in terminal and run `dotnet test`

### Improvements
- Serious refactoring to clean the code, especially the mappings and error handling and logging
- Add unit tests for more test cases
- Change to persistence stack to a better technology. SQLite was used to make the exercise portable

### Testing flow improvement with mock Acquiring Bank server
- Build and host a mock api server for the Acquiring bank
- This can be generated from the Acquiring bank OpenAPI spec (if exist)

### Cloud Technologies to Consider
- High Availability and Security are the most critical features of this system so the most performant technologies will be considered
- Technologies include Databases, Virtual Machines, Containers, Load Balancing, Auto Horizontal scaling, Message queues, Retry and Circuit breaker policies, and many more

### Sanbox of sample
![Sample of Get Sanbox](https://github.com/funwie/Checkout.PaymentGateway/blob/master/imgs/sanbox.png)


