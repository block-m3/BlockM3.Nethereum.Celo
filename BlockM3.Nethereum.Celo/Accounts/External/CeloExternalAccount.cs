using System;
using System.Numerics;
using System.Threading.Tasks;
using BlockM3.Nethereum.Celo.Signer;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.Personal;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Web3.Accounts.Managed;
using Transaction = Nethereum.Signer.Transaction;

namespace BlockM3.Nethereum.Celo.Accounts.External
{
    public class CeloExternalAccount : IAccount
    {
        public ICeloEthExternalSigner ExternalSigner { get; }
        public BigInteger? ChainId { get; }
        public string FeeCurrency {get; private set;}
        public string GatewayFeeRecipient {get; private set;}
        public BigInteger? GatewayFee { get; private set; }

        public CeloExternalAccount(ICeloEthExternalSigner externalSigner, BigInteger? chainId = null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
        {
            ExternalSigner = externalSigner;
            ChainId = chainId;
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
        }

        public CeloExternalAccount(string address, ICeloEthExternalSigner externalSigner, BigInteger? chainId = null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
        {
            ChainId = chainId;
            Address = address;
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            ExternalSigner = externalSigner;
        }

        public async Task InitialiseAsync()
        {
            Address = await ExternalSigner.GetAddressAsync();
        }

        public void InitialiseDefaultTransactionManager(IClient client)
        {
            TransactionManager = new CeloExternalAccountSignerTransactionManager(client, this, ChainId,FeeCurrency, GatewayFeeRecipient, GatewayFee);
        }

        public string Address { get; protected set; }
        public ITransactionManager TransactionManager { get; protected set; }
        public INonceService NonceService { get; set; }
    }
}
