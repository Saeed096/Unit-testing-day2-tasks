using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using CarFactoryAPI.Entities;
using CarFactoryAPI.Repositories_DAL;
using CarFactoryAPI_test.stups;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CarFactoryAPI_test
{
    public class OwnerServiceTests : IDisposable
    {
        private readonly ITestOutputHelper outputHelper;
        // Create Mock of the dependencies
        Mock<ICarsRepository> carRepoMock;
        Mock<IOwnersRepository> ownersRepoMock;
        Mock<ICashService> cashServiceMock;

        // use the fake object as dependency
        OwnersService ownersService;

        public OwnerServiceTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
            // Test Start up
            outputHelper.WriteLine("Test start up");

            // Create Mock of the dependencies
            carRepoMock = new();
            ownersRepoMock = new();
            cashServiceMock = new();

            // use the fake object as dependency
            ownersService = new OwnersService(carRepoMock.Object, ownersRepoMock.Object, cashServiceMock.Object);

        }

        public void Dispose()
        {
            outputHelper.WriteLine("Test clean up");
        }
        [Fact]
        [Trait("Author", "Ahmed")]

        public void BuyCar_CarNotExist_NotExist() 
        {
            outputHelper.WriteLine("Test 1");
            // Arrange
            FactoryContext factoryContext = new FactoryContext();

            // CarRepository carRepository = new CarRepository(factoryContext);

            // Fake Dependency
            CarRepoStup carRepoStup = new CarRepoStup();

            // Real Dependency
            OwnerRepository ownerRepository = new OwnerRepository(factoryContext);
            CashService cashService = new CashService();

            OwnersService ownersService = new OwnersService(carRepoStup,ownerRepository,cashService);

            BuyCarInput buyCarInput = new BuyCarInput()
            { OwnerId = 10, CarId = 100, Amount = 5000};

            // Act
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("n't exist", result);
        }

        [Fact]
        [Trait("Author","Ahmed")]
        public void BuyCar_CarWithOwner_Sold()
        {
            outputHelper.WriteLine("Test 2");

            // Arrange

            //// Create Mock of the dependencies
            //Mock<ICarsRepository> carRepoMock = new();
            //Mock<IOwnersRepository> ownersRepoMock = new();
            //Mock<ICashService> cashServiceMock = new();

            // Build the mock Data
            Car car = new Car() { Id = 10, Owner = new Owner() };

            // Setup the called method
            carRepoMock.Setup(cM=>cM.GetCarById(10)).Returns(car);

            // use the fake object as dependency
            //OwnersService ownersService = new OwnersService(carRepoMock.Object,ownersRepoMock.Object,cashServiceMock.Object);

            BuyCarInput buyCarInput = new BuyCarInput()
            {
                CarId = 10,
                OwnerId = 100,
                Amount = 5000
            };

            // Act
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("sold", result);

        }


        [Fact]
        [Trait("Author", "Ali")]
        [Trait("Priority", "5")]


        public void BuyCar_OwnorNotExist_NotExist()
        {
            outputHelper.WriteLine("Test 3");

            // Arrange
            // Build the mock Data
            Car car = new Car() { Id = 5 };
            Owner owner = null;

            // Setup the called Methods
            carRepoMock.Setup(cm => cm.GetCarById(It.IsAny<int>())).Returns(car);
            ownersRepoMock.Setup(om => om.GetOwnerById(It.IsAny<int>())).Returns(owner);

           
            BuyCarInput buyCarInput = new() { CarId = 5, OwnerId = 100, Amount = 5000 };


            // Act 
            string result = ownersService.BuyCar(buyCarInput);

            // Assert
            Assert.Contains("n't exist", result);
        }


        // day 2 tasks 

        [Fact]
        public void BuyCar_ownerHasCar_alreadyHasCar()
        {
            // arrange
            Car car = new Car();

            BuyCarInput buyCarInput = new() { CarId = 1, OwnerId = 2 , Amount =3 };
            carRepoMock.Setup(cM => cM.GetCarById(It.IsAny<int>())).Returns(car);
            ownersRepoMock.Setup(oM => oM.GetOwnerById(It.IsAny<int>())).Returns(new Owner() { Car = new Car()});


            // act
            string result = ownersService.BuyCar(buyCarInput);
            // assert
            Assert.Contains("Already have car", result);
        }


        [Fact]
        public void BuyCar_AmountLessThanPrice_InsufficientAmount()
        {
            // arrange
            BuyCarInput buyCarInput = new BuyCarInput() {Amount = 0 , CarId = 2 , OwnerId = 3 };
            carRepoMock.Setup(cM => cM.GetCarById(It.IsAny<int>())).Returns(new Car() { Price = 1});
            ownersRepoMock.Setup(oM => oM.GetOwnerById(It.IsAny<int>())).Returns(new Owner());
            // act
            string result = ownersService.BuyCar(buyCarInput);
            // assert
            Assert.Contains("Insufficient funds", result);
        }


        [Fact]
        public void BuyCar_InvalidAssignToOrder_SomethingWentWrong()
        {
            // arrange
            Car car = new Car() { Price = 1 };
            Owner owner = new Owner();
            BuyCarInput buyCarInput = new BuyCarInput() { Amount = 100, OwnerId = 1, CarId = 2 };
            carRepoMock.Setup(cM => cM.GetCarById(It.IsAny<int>())).Returns(car);
            ownersRepoMock.Setup(oM => oM.GetOwnerById(It.IsAny<int>())).Returns(owner);
            cashServiceMock.Setup(cM => cM.Pay(It.IsAny<double>())).Returns("Paid");
            carRepoMock.Setup(cM => cM.AssignToOwner(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            // act
            string result = ownersService.BuyCar(buyCarInput);
            // assert
            Assert.Contains("Something went wrong", result);
        }

        [Fact]
        public void BuyCar_Noerrors_successfulPayment()
        {
            // arrange
            Car car = new Car() { Price = 1};
            Owner owner = new Owner();
            BuyCarInput buyCarInput = new BuyCarInput() {Amount = 100 , OwnerId = 1 , CarId = 2 };
            carRepoMock.Setup(cM => cM.GetCarById(It.IsAny<int>())).Returns(car);
            ownersRepoMock.Setup(oM => oM.GetOwnerById(It.IsAny<int>())).Returns(owner);
            cashServiceMock.Setup(cM => cM.Pay(It.IsAny<double>())).Returns("Paid");
            carRepoMock.Setup(cM => cM.AssignToOwner(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // act
            string result = ownersService.BuyCar(buyCarInput);
            // assert
            Assert.Contains("Successfull", result);
        }






        
    }
}
