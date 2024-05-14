using CarAPI.Entities;
using CarFactoryAPI.Entities;
using CarFactoryAPI.Repositories_DAL;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CarFactoryAPI_test
{
    // day 2 tasks
    public class OwnerRepositoryTests
    {
        private Mock<FactoryContext> factoryContextMock;
        private ITestOutputHelper testOutputHelper;

        public OwnerRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            factoryContextMock = new Mock<FactoryContext>();
            this.testOutputHelper = testOutputHelper;
            testOutputHelper.WriteLine("Test startup");
        }

        public void Dispose()
        {
            testOutputHelper.WriteLine("Test cleanup");
        }


        [Fact(Skip ="Working on it")]
        [Trait("Author" , "saeed")] 
        public void AddOwner_sendOwner_finditInCollection()
        {
            // arrange
            List<Owner> owners = new List<Owner>();
            OwnerRepository ownerRepository = new OwnerRepository(factoryContextMock.Object);
            factoryContextMock.Setup(fCM => fCM.Owners).ReturnsDbSet(owners);

            Owner owner = new Owner();
            // act
            // why fail????????????????????????????????
             ownerRepository.AddOwner(owner);

          //  bool result = ownerRepository.AddOwner(owner);


            // assert
            // fail
             Assert.Collection(factoryContextMock.Object.Owners, item => Assert.Same(owner, item)); 
            
            // Assert.True(result);
        }


        [Fact]  // here
        [Trait("Author", "saeed")]
        public void GetAll_callFunction_getAll()
        {
            // arrange 
            OwnerRepository ownerRepository = new OwnerRepository(factoryContextMock.Object);
            List<Owner> owners = new List<Owner>() {
            new Owner{Id = 1, Name ="saeed"} , new Owner{Id = 2 , Name = "ali"}
            };

            factoryContextMock.Setup(fCM => fCM.Owners).ReturnsDbSet(owners);

            List<Owner> resultOwners = new List<Owner>();
            // act 
            resultOwners = ownerRepository.GetAllOwners();
            // assert
            Assert.Equal(resultOwners, owners);
        }

    }
}
