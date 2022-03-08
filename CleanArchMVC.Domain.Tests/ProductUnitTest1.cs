using CleanArchMVC.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace CleanArchMVC.Domain.Tests
{
    public class ProductUnitTest1
    {
        [Fact(DisplayName = "Create Product With Valid Parameters")]
        public void CreateProduct_WithValidParameters_ResultObjectValidState()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, "Product Image");
            action.Should().
                NotThrow<CleanArchMVC.Domain.Validation.DomainExceptionValidation>();

        }

        [Fact(DisplayName = "Create Product With Negative Id")]
        public void CreateProduct_NegativeIdValue_DomainExceptionInvalidId()
        {
            Action action = () => new Product(-1, "Product Name", "Product Description", 9.99m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid Id value");

        }

        [Fact(DisplayName = "Create Product With Missing Name")]
        public void CreateProduct_MissingNameValue_DomainExceptionRequiredName()
        {
            Action action = () => new Product(1, "", "Product Description", 9.99m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid name. Name is required");
        }

        [Fact(DisplayName = "Create Product With Short Name")]
        public void CreateProduct_ShortNameValue_DomainExceptionShortName()
        {
            Action action = () => new Product(1, "Pr", "Product Description", 9.99m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid name, too short, minimum 3 characters");
        }

        [Fact(DisplayName = "Create Product With Missing Description")]
        public void CreateProduct_MissingDescriptionValue_DomainExceptionMissingDescription()
        {
            Action action = () => new Product(1, "Product Name", "", 9.99m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid description. Description is required");
        }

        [Fact(DisplayName = "Create Product With Short Description")]
        public void CreateProduct_ShortDescriptionValue_DomainExceptionShortDescription()
        {
            Action action = () => new Product(1, "Product Name", "Prod", 9.99m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid description, too short, minimum 5 characters");
        }

        [Fact(DisplayName = "Create Product With Invalid Price Value")]
        public void CreateProduct_InvalidPriceValue_DomainExceptionPriceValue()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", -1m, 99, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid price value");
        }

        [Theory(DisplayName = "Create Product With Invalid Stock Value")]
        [InlineData(-5)]
        public void CreateProduct_InvalidStockValue_DomainExceptionStockValue(int value)
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, value, "Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid stock value");
        }

        [Fact(DisplayName = "Create Product With Long Image Name")]
        public void CreateProduct_LongImageNameValue_DomainExceptionLongImageName()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99,
                "Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image Product Image");
            action.Should().
                Throw<CleanArchMVC.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid image name, too long, maximum 250 characters");
        }

        [Fact(DisplayName = "Create Product With Null Image Name")]
        public void CreateProduct_NullImageNameValue_DomainExceptionNullImageName()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, null);
            action.Should().
                NotThrow<CleanArchMVC.Domain.Validation.DomainExceptionValidation>();
        }

        [Fact(DisplayName = "Create Product With Null Image Name")]
        public void CreateProduct_NullImageNameValue_NoNullReferenceException()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, null);
            action.Should().
                NotThrow<NullReferenceException>();
        }

        [Fact(DisplayName = "Create Product With Empty Image Name")]
        public void CreateProduct_EmptyImageNameValue_DomainExceptionEmptyImageName()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, "");
            action.Should().
                NotThrow<CleanArchMVC.Domain.Validation.DomainExceptionValidation>();
        }
    }
}
