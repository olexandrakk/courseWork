using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using courseWork.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System.Linq;

namespace courseWork.Tests.Services
{
    public class PublisherServiceTests
    {
        private readonly Mock<IRepository<Publisher>> _publisherRepositoryMock;
        private readonly IMapper _mapper;

        public PublisherServiceTests()
        {
            _publisherRepositoryMock = new Mock<IRepository<Publisher>>();
            _mapper = TestHelper.CreateMapper();
        }

        private void SetupQueryable<T>(Mock<IRepository<T>> mock, List<T> data) where T : class
        {
            var mockQueryable = data.BuildMock();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());
            mock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public TestAsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public T Current => _enumerator.Current;

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public ValueTask DisposeAsync() 
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }
        }

        [Fact]
        public async Task CreatePublisherAsync_WhenNameExists_ThrowsInvalidOperationException()
        {
            var request = new CreatePublisherRequest
            {
                Name = "Test Publisher",
                Address = "Test Address"
            };

            var existingPublisher = new Publisher
            {
                PublisherID = 1,
                Name = "Test Publisher",
                Address = "Existing Address"
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher> { existingPublisher });

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreatePublisherAsync(request));
        }

        [Fact]
        public async Task CreatePublisherAsync_WhenValid_CreatesPublisher()
        {
            var request = new CreatePublisherRequest
            {
                Name = "New Publisher",
                Address = "New Address"
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher>());

            _publisherRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Publisher>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            var result = await service.CreatePublisherAsync(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Address.Should().Be(request.Address);
            _publisherRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Publisher>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetAllPublishersAsync_ReturnsAllPublishers()
        {
            var publishers = new List<Publisher>
            {
                new Publisher { PublisherID = 1, Name = "Publisher 1", Address = "Address 1" },
                new Publisher { PublisherID = 2, Name = "Publisher 2", Address = "Address 2" }
            };

            SetupQueryable(_publisherRepositoryMock, publishers);

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            var result = await service.GetAllPublishersAsync();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdatePublisherAsync_WhenPublisherNotFound_ThrowsException()
        {
            var publisherId = 999;
            var request = new CreatePublisherRequest
            {
                Name = "Updated Publisher",
                Address = "Updated Address"
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher>());

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.UpdatePublisherAsync(publisherId, request));
        }

        [Fact]
        public async Task UpdatePublisherAsync_WhenNameExists_ThrowsInvalidOperationException()
        {
            var publisherId = 1;
            var existingPublisher = new Publisher
            {
                PublisherID = publisherId,
                Name = "Original Publisher",
                Address = "Original Address"
            };

            var anotherPublisher = new Publisher
            {
                PublisherID = 2,
                Name = "Another Publisher",
                Address = "Another Address"
            };

            var request = new CreatePublisherRequest
            {
                Name = "Another Publisher",
                Address = "Updated Address"
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher> { existingPublisher, anotherPublisher });

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdatePublisherAsync(publisherId, request));
        }

        [Fact]
        public async Task UpdatePublisherAsync_WhenValid_UpdatesPublisher()
        {
            var publisherId = 1;
            var publisher = new Publisher
            {
                PublisherID = publisherId,
                Name = "Original Publisher",
                Address = "Original Address"
            };

            var request = new CreatePublisherRequest
            {
                Name = "Updated Publisher",
                Address = "Updated Address"
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher> { publisher });

            _publisherRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Publisher>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            var result = await service.UpdatePublisherAsync(publisherId, request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Address.Should().Be(request.Address);
            _publisherRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Publisher>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeletePublisherAsync_WhenPublisherNotFound_ThrowsException()
        {
            var publisherId = 999;
            SetupQueryable(_publisherRepositoryMock, new List<Publisher>());

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeletePublisherAsync(publisherId));
        }

        [Fact]
        public async Task DeletePublisherAsync_WhenPublisherHasBooks_ThrowsInvalidOperationException()
        {
            var publisherId = 1;
            var publisher = new Publisher
            {
                PublisherID = publisherId,
                Name = "Test Publisher",
                Address = "Test Address",
                Books = new List<Book> { new Book { BookID = 1 } }
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher> { publisher });

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.DeletePublisherAsync(publisherId));
        }

        [Fact]
        public async Task DeletePublisherAsync_WhenValid_DeletesPublisher()
        {
            var publisherId = 1;
            var publisher = new Publisher
            {
                PublisherID = publisherId,
                Name = "Test Publisher",
                Address = "Test Address",
                Books = new List<Book>()
            };

            SetupQueryable(_publisherRepositoryMock, new List<Publisher> { publisher });

            _publisherRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Publisher>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new PublisherService(_publisherRepositoryMock.Object, _mapper);

            await service.DeletePublisherAsync(publisherId);

            _publisherRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Publisher>(), It.IsAny<bool>()), Times.Once);
        }
    }
}

