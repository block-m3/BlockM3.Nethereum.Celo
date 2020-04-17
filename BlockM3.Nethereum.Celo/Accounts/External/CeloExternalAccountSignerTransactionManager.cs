using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;
using Transaction = Nethereum.Signer.Transaction;
using Nethereum.Web3.Accounts;
using Nethereum.RPC.Net;
using BlockM3.Nethereum.Celo.Signer;

namespace BlockM3.Nethereum.Celo.Accounts.External
{
    public class CeloExternalAccountSignerTransactionManager : TransactionManagerBase
    {
       private readonly CeloTransactionSigner _transactionSigner;
        public BigInteger ChainId { get; private set; }
        public string FeeCurrency {get; private set;}
        public string GatewayFeeRecipient {get; private set;}
        public BigInteger? GatewayFee { get; private set; }

        public CeloExternalAccountSignerTransactionManager(IClient rpcClient, CeloExternalAccount account, BigInteger? chainId = null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
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
            _transactionSigner = new CeloTransactionSigner();
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

        public async Task<string> SignTransactionExternallyAsync(TransactionInput transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (!transaction.From.IsTheSameAddress(Account.Address))
                throw new Exception("Invalid account used signing");
            SetDefaultGasPriceAndCostIfNotSet(transaction);

            var nonce = transaction.Nonce;
            if (nonce == null) throw new ArgumentNullException(nameof(transaction), "Transaction nonce has not been set");

            var gasPrice = transaction.GasPrice;
            var gasLimit = transaction.Gas;

            var value = transaction.Value ?? new HexBigInteger(0);

            string signedTransaction;
            BigInteger gwfee = GatewayFee.HasValue ? GatewayFee.Value :new BigInteger(0);

            var externalSigner = ((CeloExternalAccount) Account).ExternalSigner;
            signedTransaction = await _transactionSigner.SignTransactionAsync(externalSigner, ChainId,
                    transaction.To,
                    value.Value, FeeCurrency,  GatewayFeeRecipient, gwfee, nonce,
                    gasPrice.Value, gasLimit.Value, transaction.Data);

            return signedTransaction;
        }


        public string SignTransaction(TransactionInput transaction)
        {
            return SignTransactionRetrievingNextNonceAsync(transaction).Result;
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
            return await SignTransactionExternallyAsync(transaction).ConfigureAwait(false);
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
