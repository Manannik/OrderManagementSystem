﻿using Application.Models;
using FluentValidation;

namespace WebApplication.Controllers.Validators
{
    public class UpdateProductQuantityRequestValidator:AbstractValidator<OrderedQuantity>
    {
        public UpdateProductQuantityRequestValidator()
    {
        RuleFor(f => f.OrderQuantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
    }
}