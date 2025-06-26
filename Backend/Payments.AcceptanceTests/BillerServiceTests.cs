using FluentAssertions;
using Payments.Contracts;
using Payments.Domain.Exceptions;
using Payments.Services.Abstractions;
using NUnit.Framework;

namespace Payments.AcceptanceTests
{
    [TestFixture]
    public class BillerServiceTests : IAsyncDisposable
    {
        private TestFixture? _testFixture;
        private IBillerService? _billerService;

        [SetUp]
        public void SetUp()
        {
            _testFixture = new TestFixture();
            _billerService = _testFixture.BillerService;
        }

        [TearDown]
        public void TearDown()
        {
            _testFixture?.DisposeAsync();
        }

        [Test]
        public async Task GetAllAsync_WhenNoBillersExist_ReturnsEmptyCollection()
        {
            // Act
            var result = await _billerService!.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task CreateAsync_WithValidData_CreatesBillerSuccessfully()
        {
            // Arrange
            var billerForCreation = new BillerForCreationDto
            {
                Name = "Test Biller",
                ApiKey = "sk_test_1234567890abcdef"
            };

            // Act
            var result = await _billerService!.CreateAsync(billerForCreation);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be(billerForCreation.Name);
            result.ApiKey.Should().Be(billerForCreation.ApiKey);
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task GetByIdAsync_WhenBillerExists_ReturnsBiller()
        {
            // Arrange
            var billerForCreation = new BillerForCreationDto
            {
                Name = "Test Biller",
                ApiKey = "sk_test_1234567890abcdef"
            };
            var createdBiller = await _billerService!.CreateAsync(billerForCreation);

            // Act
            var result = await _billerService.GetByIdAsync(createdBiller.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdBiller.Id);
            result.Name.Should().Be(createdBiller.Name);
            result.ApiKey.Should().Be(createdBiller.ApiKey);
        }

        [Test]
        public async Task GetByIdAsync_WhenBillerDoesNotExist_ThrowsBillerNotFoundException()
        {
            // Arrange
            var nonExistentId = long.MaxValue;

            // Act & Assert
            var exception = Assert.ThrowsAsync<BillerNotFoundException>(() => 
                _billerService!.GetByIdAsync(nonExistentId));
            exception.Should().NotBeNull();
        }

        [Test]
        public async Task UpdateAsync_WhenBillerExists_UpdatesBillerSuccessfully()
        {
            // Arrange
            var billerForCreation = new BillerForCreationDto
            {
                Name = "Original Name",
                ApiKey = "sk_test_original"
            };
            var createdBiller = await _billerService!.CreateAsync(billerForCreation);

            var billerForUpdate = new BillerForUpdateDto
            {
                Name = "Updated Name",
                ApiKey = "sk_test_updated"
            };

            // Act
            await _billerService.UpdateAsync(createdBiller.Id, billerForUpdate);

            // Assert
            var updatedBiller = await _billerService.GetByIdAsync(createdBiller.Id);
            updatedBiller.Name.Should().Be(billerForUpdate.Name);
            updatedBiller.ApiKey.Should().Be(billerForUpdate.ApiKey);
            updatedBiller.UpdatedAt.Should().BeAfter(updatedBiller.CreatedAt);
        }

        [Test]
        public async Task UpdateAsync_WhenBillerDoesNotExist_ThrowsBillerNotFoundException()
        {
            // Arrange
            var nonExistentId = long.MaxValue;
            var billerForUpdate = new BillerForUpdateDto
            {
                Name = "Updated Name",
                ApiKey = "sk_test_updated"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<BillerNotFoundException>(() => 
                _billerService!.UpdateAsync(nonExistentId, billerForUpdate));
            exception.Should().NotBeNull();
        }

        [Test]
        public async Task DeleteAsync_WhenBillerExists_DeletesBillerSuccessfully()
        {
            // Arrange
            var billerForCreation = new BillerForCreationDto
            {
                Name = "Test Biller",
                ApiKey = "sk_test_1234567890abcdef"
            };
            var createdBiller = await _billerService!.CreateAsync(billerForCreation);

            // Act
            await _billerService.DeleteAsync(createdBiller.Id);

            // Assert
            var exception = Assert.ThrowsAsync<BillerNotFoundException>(() => 
                _billerService.GetByIdAsync(createdBiller.Id));
            exception.Should().NotBeNull();
        }

        [Test]
        public async Task DeleteAsync_WhenBillerDoesNotExist_ThrowsBillerNotFoundException()
        {
            // Arrange
            var nonExistentId = long.MaxValue;

            // Act & Assert
            var exception = Assert.ThrowsAsync<BillerNotFoundException>(() => 
                _billerService!.DeleteAsync(nonExistentId));
            exception.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllAsync_WhenMultipleBillersExist_ReturnsAllBillers()
        {
            // Arrange
            var biller1 = new BillerForCreationDto
            {
                Name = "Biller 1",
                ApiKey = "sk_test_1"
            };
            var biller2 = new BillerForCreationDto
            {
                Name = "Biller 2",
                ApiKey = "sk_test_2"
            };

            await _billerService!.CreateAsync(biller1);
            await _billerService.CreateAsync(biller2);

            // Act
            var result = await _billerService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(b => b.Name == "Biller 1");
            result.Should().Contain(b => b.Name == "Biller 2");
        }

        public async ValueTask DisposeAsync()
        {
            // Clean up any test data if needed
            await ValueTask.CompletedTask;
        }
    }
} 