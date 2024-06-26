﻿using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Scan;

public class ScanCommand : ICommand<Result>
{
    public bool UseCirrus { get; set; }

    public bool Testnet { get; set; }

    public int StartBlock { get; set; }

    public int EndBlock { get; set; }

    public string OutputFolder { get; set; }
}
