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
    public class ReviewServiceTests
    {
        private readonly Mock<IRepository<Review>> _reviewRepositoryMock;
        private readonly IMapper _mapper;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IRepository<Review>>();
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
        public async Task CreateReviewAsync_WhenValid_CreatesReview()
        {
            var request = new CreateReviewRequest
            {
                UserId = 1,
                BookId = 1,
                Rating = 5,
                Comment = "Great book!"
            };

            _reviewRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Review>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            var result = await service.CreateReviewAsync(request);

            result.Should().NotBeNull();
            result.Rating.Should().Be(request.Rating);
            result.Comment.Should().Be(request.Comment);
            _reviewRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Review>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_WhenReviewNotFound_ThrowsException()
        {
            var reviewId = 999;
            var request = new CreateReviewRequest
            {
                UserId = 1,
                BookId = 1,
                Rating = 4,
                Comment = "Updated comment"
            };

            SetupQueryable(_reviewRepositoryMock, new List<Review>());

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.UpdateReviewAsync(reviewId, request));
        }

        [Fact]
        public async Task UpdateReviewAsync_WhenReviewExists_UpdatesReview()
        {
            var reviewId = 1;
            var existingReview = new Review
            {
                ReviewID = reviewId,
                UserID = 1,
                BookID = 1,
                Rating = 3,
                Comment = "Old comment",
                Date = DateTime.UtcNow.AddDays(-1)
            };

            var request = new CreateReviewRequest
            {
                UserId = 1,
                BookId = 1,
                Rating = 5,
                Comment = "Updated comment"
            };

            SetupQueryable(_reviewRepositoryMock, new List<Review> { existingReview });

            _reviewRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Review>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            var result = await service.UpdateReviewAsync(reviewId, request);

            result.Should().NotBeNull();
            result.Rating.Should().Be(request.Rating);
            result.Comment.Should().Be(request.Comment);
            existingReview.Date.Should().BeBefore(DateTime.UtcNow);
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Review>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_WhenReviewNotFound_ThrowsException()
        {
            var reviewId = 999;
            SetupQueryable(_reviewRepositoryMock, new List<Review>());

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            await Assert.ThrowsAsync<Exception>(
                () => service.DeleteReviewAsync(reviewId));
        }

        [Fact]
        public async Task DeleteReviewAsync_WhenReviewExists_DeletesReview()
        {
            var reviewId = 1;
            var review = new Review
            {
                ReviewID = reviewId,
                UserID = 1,
                BookID = 1,
                Rating = 5,
                Comment = "Test comment"
            };

            SetupQueryable(_reviewRepositoryMock, new List<Review> { review });

            _reviewRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Review>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            await service.DeleteReviewAsync(reviewId);

            _reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Review>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetReviewsByBookIdAsync_ReturnsPagedReviews()
        {
            var bookId = 1;
            var reviews = new List<Review>
            {
                new Review { ReviewID = 1, BookID = bookId, UserID = 1, Rating = 5, Comment = "Great!", Date = DateTime.UtcNow },
                new Review { ReviewID = 2, BookID = bookId, UserID = 2, Rating = 4, Comment = "Good", Date = DateTime.UtcNow.AddDays(-1) }
            };

            SetupQueryable(_reviewRepositoryMock, reviews);

            var request = new GetReviewsRequest
            {
                BookId = bookId,
                Page = 1,
                PageSize = 10
            };

            var service = new ReviewService(_reviewRepositoryMock.Object, _mapper);

            var result = await service.GetReviewsByBookIdAsync(request);

            result.Should().NotBeNull();
            result.Items.Should().NotBeEmpty();
        }
    }
}
