using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.KeyStore;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.Net;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Transaction = Nethereum.Signer.Transaction;

namespace BlockM3.Nethereum.Celo.Accounts
{
    public class CeloAccountSignerTransactionManager : TransactionManagerBase
    {
        private readonly CeloAccountOfflineTransactionSigner _transactionSigner;
        public BigInteger ChainId { get; private set; }
        public string FeeCurrency {get; private set;}
        public string GatewayFeeRecipient {get; private set;}
        public BigInteger? GatewayFee { get; private set; }

        public CeloAccountSignerTransactionManager(IClient rpcClient, Account account, BigInteger? chainId, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
        {
            if (!chainId.HasValue)
            {
                NetVersion nv=new NetVersion(rpcClient);
                string version=nv.SendRequestAsync().GetAwaiter().GetResult();
                ChainId = BigInteger.Parse(version);
            }
            else
                ChainId = chainId.Value;
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Client = rpcClient;
            _transactionSigner = new CeloAccountOfflineTransactionSigner();
        }


        public CeloAccountSignerTransactionManager(IClient rpcClient, string privateKey, BigInteger? chainId,string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
        {
            if (!chainId.HasValue)
            {
                NetVersion nv=new NetVersion(rpcClient);
                string version=nv.SendRequestAsync().GetAwaiter().GetResult();
                ChainId = BigInteger.Parse(version);
            }
            else
                ChainId = chainId.Value;
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
            Client = rpcClient;
            Account = new Account(privateKey);
            Account.NonceService = new InMemoryNonceService(Account.Address, rpcClient);
            _transactionSigner = new CeloAccountOfflineTransactionSigner();
        }

        public CeloAccountSignerTransactionManager(string privateKey, BigInteger? chainId = null,string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : this(null, privateKey, chainId, feeCurrency, gatewayFeeRecipient, gatewayFee)
        {

        }

        public override BigInteger DefaultGas { get; set; } = Transaction.DEFAULT_GAS_LIMIT;


        public override Task<string> SendTransactionAsync(TransactionInput transactionInput)
        {
            if (transactionInput == null) throw new ArgumentNullException(nameof(transactionInput));
            return SignAndSendTransactionAsync(transactionInput);
        }

        public override Task<string> SignTransactionAsync(TransactionInput transaction)
        {
            return SignTransactionRetrievingNextNonceAsync(transaction);
        }

        public string SignTransaction(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            SetDefaultGasPriceAndCostIfNotSet(transaction);
            return _transactionSigner.SignTransaction((Account) Account, transaction, ChainId, FeeCurrency, GatewayFeeRecipient, GatewayFee);
        }

        protected async Task<string> SignTransactionRetrievingNextNonceAsync(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");
            var nonce = await GetNonceAsync(transaction).ConfigureAwait(false);
            transaction.Nonce = nonce;
            var gasPrice = await GetGasPriceAsync(transaction).ConfigureAwait(false);
            transaction.GasPrice = gasPrice;
            return SignTransaction(transaction);
        }

        public async Task<HexBigInteger> GetNonceAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            var nonce = transaction.Nonce;
            if (nonce == null)
            {
                if (Account.NonceService == null)
                    Account.NonceService = new InMemoryNonceService(Account.Address, Client);
                Account.NonceService.Client = Client;
                nonce = await Account.NonceService.GetNextNonceAsync().ConfigureAwait(false);
            }
            return nonce;
        }

        private async Task<string> SignAndSendTransactionAsync(TransactionInput transaction)
        {
            if (Client == null) throw new NullReferenceException("Client not configured");
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");

            var ethSendTransaction = new EthSendRawTransaction(Client);
            var signedTransaction = await SignTransactionRetrievingNextNonceAsync(transaction).ConfigureAwait(false);
            return await ethSendTransaction.SendRequestAsync(signedTransaction.EnsureHexPrefix()).ConfigureAwait(false);
        }
    }
}
