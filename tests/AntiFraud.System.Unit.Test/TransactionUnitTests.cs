using System;
using AntiFraud.System.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AntiFraud.System.UnitTests.Domain.Entities
{
    [Trait("Entity", "Transaction")]
    public sealed class TransactionTests
    {
        // Helper para criar uma transação base para os testes
        private Transaction CreateDefaultTransaction()
        {
            return new Transaction(
                amount: 4000,
                cardHolder: "Maria Teste",
                cardNumber: "1234-5678-9012-3456",
                ipAddress: "192.168.1.1",
                location: "BR",
                transactionDate: DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CreateTransaction_WithValidData_ShouldSetInitialStatusToPending()
        {
            // Arrange & Act
            var transaction = CreateDefaultTransaction();

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Pending);
            transaction.Id.Should().NotBeEmpty();
            transaction.CardHolder.Should().Be("Maria Teste");
        }

        [Fact]
        public void Approve_WhenStatusIsPending_ShouldChangeStatusToApproved()
        {
            // Arrange
            var transaction = CreateDefaultTransaction();

            // Act
            transaction.Approve();

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Approved);
        }

        [Fact]
        public void Reject_WhenStatusIsPending_ShouldChangeStatusToRejected()
        {
            // Arrange
            var transaction = CreateDefaultTransaction();

            // Act
            transaction.Reject();

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Rejected);
        }

        [Fact]
        public void SendToReview_WhenStatusIsPending_ShouldChangeStatusToReview()
        {
            // Arrange
            var transaction = CreateDefaultTransaction();

            // Act
            transaction.SendToReview();

            // Assert
            transaction.Status.Should().Be(TransactionStatus.Review);
        }

        [Fact]
        public void Approve_WhenStatusIsAlreadyApproved_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var transaction = CreateDefaultTransaction();
            transaction.Approve(); 

            // Act
            Action act = () => transaction.Approve();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Não é possível aprovar uma transação com o status 'Approved'.");
        }

        [Fact]
        public void SendToReview_WhenStatusIsNotPending_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var transaction = CreateDefaultTransaction();
            transaction.Approve(); 

            // Act
            Action act = () => transaction.SendToReview();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Apenas transações pendentes podem ser enviadas para revisão.");
        }
    }
}