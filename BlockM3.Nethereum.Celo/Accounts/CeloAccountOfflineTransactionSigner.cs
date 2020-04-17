using BlockM3.Nethereum.Celo.Signer;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlockM3.Nethereum.Celo.Accounts
{
    public class CeloAccountOfflineTransactionSigner
    {
        public static readonly BigInteger DEFAULT_GATEWAY_FEE = BigInteger.Parse("0");


        private readonly CeloTransactionSigner _transactionSigner;

        public CeloAccountOfflineTransactionSigner(CeloTransactionSigner transactionSigner)
        {
            _transactionSigner = transactionSigner;
        }

        public CeloAccountOfflineTransactionSigner()
        {
            _transactionSigner = new CeloTransactionSigner();
        }


        public string SignTransaction(Account account, TransactionInput transaction, BigInteger chainId, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (string.IsNullOrWhiteSpace(transaction.From))
            {
                transaction.From = account.Address;
            }
            else if (!transaction.From.IsTheSameAddress(account.Address))
            {
                throw new Exception("Invalid account used for signing, does not match the transaction input");
            }

            var nonce = transaction.Nonce;
            if (nonce == null) throw new ArgumentNullException(nameof(transaction), "Transaction nonce has not been set");

            var gasPrice = transaction.GasPrice;
            var gasLimit = transaction.Gas;

            var value = transaction.Value ?? new HexBigInteger(0);
            var gwfee = gatewayFee.HasValue ? gatewayFee.Value : new BigInteger(0);

            string signedTransaction;
            signedTransaction = _transactionSigner.SignTransaction(account.PrivateKey, chainId,
                    transaction.To,
                    value.Value,feeCurrency, gatewayFeeRecipient,  gwfee, nonce,
                    gasPrice.Value, gasLimit.Value, transaction.Data);
            return signedTransaction;
        }
    }
}
