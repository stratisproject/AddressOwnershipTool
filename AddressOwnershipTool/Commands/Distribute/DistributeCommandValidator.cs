﻿using FluentValidation;

namespace AddressOwnershipTool.Commands.Distribute;

public class DistributeCommandValidator : AbstractValidator<DistributeCommand>
{
    public DistributeCommandValidator()
    {            
        RuleFor(x => x.WalletName).NotEmpty().WithName("--walletname");
        RuleFor(x => x.WalletPassword).NotEmpty().WithName("--walletpassword");
    }
}
