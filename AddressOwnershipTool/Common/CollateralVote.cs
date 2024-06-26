﻿using NBitcoin;

namespace AddressOwnershipTool.Common;

public sealed class CollateralVote
{
    public string Address { get; set; }
    public Money Balance { get; set; }
    public string Selection { get; set; }
    public int BlockHeight { get; set; }
}