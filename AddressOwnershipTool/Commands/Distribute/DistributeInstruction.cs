﻿using CommandLine;

namespace AddressOwnershipTool.Commands.Distribute;

[Verb("distribute", HelpText = "Distributes tokens")]
public class DistributeInstruction : BaseInstruction
{
    [Option('n', "walletname", Required = true, HelpText = "Please provide distribution wallet name.")]
    public string WalletName { get; set; }

    [Option('p', "walletpassword", Required = true, HelpText = "Please provide distribution wallet password.")]
    public string WalletPassword { get; set; }

    [Option('a', "walletaccount", Required = false, HelpText = "Please provide distribution wallet account (Default 'account 0').", Default = "account 0")]
    public string WalletAccount { get; set; }

    [Option('c', "cirrus", Required = false, HelpText = "[Optional] Specify if you want to use CIRRUS, default is STRAX.")]
    public bool UseCirrus { get; set; }

    public DistributeCommand ToCommand()
    {
        return new DistributeCommand
        {
            WalletName = WalletName,
            WalletPassword = WalletPassword,
            WalletAccount = WalletAccount,
            UseCirrus = UseCirrus,
            Testnet = Testnet
        };
    }
}
