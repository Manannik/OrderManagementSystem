﻿using Application.Models;
using FluentValidation;

namespace WebApplication.Controllers.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
    {
        RuleFor(f => f.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(f => f.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);
        
        RuleFor(f => f.Description)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(f => f.Quantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
        
        RuleFor(f => f.CategoryModelDtos)
            .NotEmpty();
    }
    }
}