﻿using Flurl;
using Flurl.Http;
using NBitcoin;
using Newtonsoft.Json;
using Stratis.Bitcoin.Controllers.Models;
using Stratis.Bitcoin.Features.BlockStore.Models;
using Stratis.Bitcoin.Features.Wallet.Models;
using System.Text;

namespace AddressOwnershipTool.Common;

public class NodeApiClient : IDisposable, INodeApiClient
{
    private readonly string baseUrl;

    private HttpClient httpClient;

    public NodeApiClient(string baseUrl)
    {
        this.baseUrl = baseUrl;
        this.httpClient = new HttpClient();
    }

    public bool ValidateAddress(string address)
    {
        ValidatedAddress validateResult = $"{this.baseUrl}"
            .AppendPathSegment("Node/validateaddress")
            .SetQueryParam("address", address)
            .GetJsonAsync<ValidatedAddress>().GetAwaiter().GetResult();

        return validateResult.IsValid;
    }

    public decimal GetAddressBalance(string address)
    {
        AddressBalancesResult result = $"{this.baseUrl}"
            .AppendPathSegment("Blockstore/getaddressesbalances")
            .SetQueryParam("addresses", address)
            .SetQueryParam("minConfirmations", 0)
            .GetJsonAsync<AddressBalancesResult>().GetAwaiter().GetResult();

        AddressBalanceResult addressBalance = result.Balances.FirstOrDefault();

        if (addressBalance == null)
            return 0;

        // TODO: Check the units
        return addressBalance.Balance.ToDecimal(MoneyUnit.Satoshi);
    }

    public async Task<BlockTransactionDetailsModel> RetrieveBlockAtHeightAsync(int blockHeight)
    {
        var blockHash = await $"{this.baseUrl}"
            .AppendPathSegment("consensus/getblockhash")
            .SetQueryParams(new { height = blockHeight })
            .GetJsonAsync<string>();

        if (blockHash == null)
            return null;

        BlockTransactionDetailsModel blockModel = await $"{this.baseUrl}"
            .AppendPathSegment("blockstore/block")
            .SetQueryParams(new SearchByHashRequest() { Hash = blockHash, ShowTransactionDetails = true, OutputJson = true })
            .GetJsonAsync<BlockTransactionDetailsModel>();

        return blockModel;
    }

    public List<AddressBalanceChange> GetVerboseAddressBalance(string address)
    {
        VerboseAddressBalancesResult result = $"{this.baseUrl}"
            .AppendPathSegment("Blockstore/getverboseaddressesbalances")
            .SetQueryParam("addresses", address)
            .GetJsonAsync<VerboseAddressBalancesResult>().GetAwaiter().GetResult();

        AddressIndexerData addressBalance = result.BalancesData.FirstOrDefault();

        if (addressBalance == null)
            return new List<AddressBalanceChange>();

        return addressBalance.BalanceChanges;
    }

    public TransactionModel GetTransaction(string txId)
    {
        TransactionModel result = $"{this.baseUrl}"
            .AppendPathSegment($"transactions/")
            .AppendPathSegment($"{txId}")
            .GetJsonAsync<TransactionModel>().GetAwaiter().GetResult();

        return result;
    }

    public WalletBuildTransactionModel BuildTransaction(string walletName, string walletPassword, string accountName, List<RecipientModel> recipients)
    {
        var result = $"{this.baseUrl}"
            .AppendPathSegment("Wallet/build-transaction")
            .PostJsonAsync(new BuildTransactionRequest
            {
                WalletName = walletName,
                AccountName = accountName,
                FeeType = "medium",
                Password = walletPassword,
                Recipients = recipients,
                AllowUnconfirmed = true,
                ShuffleOutputs = true
            })
            .ReceiveBytes().GetAwaiter().GetResult();

        WalletBuildTransactionModel buildTransactionModel = JsonConvert.DeserializeObject<WalletBuildTransactionModel>(Encoding.ASCII.GetString(result));

        return buildTransactionModel;
    }

    public void SendTransaction(string hex)
    {
        WalletSendTransactionModel sendActionResult = $"{this.baseUrl}"
            .AppendPathSegment("wallet/send-transaction")
            .PostJsonAsync(new SendTransactionRequest
            {
                Hex = hex
            })
            .ReceiveJson<WalletSendTransactionModel>().GetAwaiter().GetResult();
    }

    public List<string> GetAccounts(string wallet)
    {
        List<string> result = $"{this.baseUrl}"
            .AppendPathSegment("Wallet/accounts")
            .SetQueryParam("WalletName", $"{wallet}")
            .GetJsonAsync<List<string>>().GetAwaiter().GetResult();

        return result;
    }

    public List<string> GetAddresses(string walletName, string account)
    {
        AddressesModel result = $"{this.baseUrl}"
            .AppendPathSegment("Wallet/addresses")
            .SetQueryParam("WalletName", walletName)
            .SetQueryParam("AccountName", account)
            .GetJsonAsync<AddressesModel>().GetAwaiter().GetResult();

        return result.Addresses.Select(a => a.Address).ToList();
    }

    public string SignMessage(string walletName, string walletPassword, string address)
    {
        var model = new SignMessageRequest() { ExternalAddress = address, Message = address, Password = walletPassword, WalletName = walletName };

        string result = $"{this.baseUrl}"
            .AppendPathSegment("Wallet/signmessage")
            .PostJsonAsync(model).ReceiveJson<string>().GetAwaiter().GetResult();

        return result;
    }

    public void Dispose()
    {
        this.httpClient?.Dispose();
    }
}
