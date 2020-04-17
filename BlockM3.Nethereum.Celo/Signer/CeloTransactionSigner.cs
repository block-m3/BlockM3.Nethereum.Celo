using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;

namespace BlockM3.Nethereum.Celo.Signer
{
    public class CeloTransactionSigner
    {
        public byte[] GetPublicKey(string rlp)
        {
            var transaction = CeloTransactionFactory.CreateTransaction(rlp);
            return transaction.Key.GetPubKey();
        }

        public string GetSenderAddress(string rlp)
        {
            var transaction =CeloTransactionFactory.CreateTransaction(rlp);
            return transaction.Key.GetPublicAddress();
        }

        public bool VerifyTransaction(string rlp)
        {
            var transaction = CeloTransactionFactory.CreateTransaction(rlp);
            return transaction.Key.VerifyAllowingOnlyLowS(transaction.RawHash, transaction.Signature);
        }

        public byte[] GetPublicKey(string rlp, Chain chain)
        {
            return GetPublicKey(rlp, (int)chain);
        }

        public byte[] GetPublicKey(string rlp, BigInteger chainId)
        {
            var transaction = new CeloTransactionChainId(rlp.HexToByteArray(), chainId);
            return transaction.Key.GetPubKey();
        }

        public string GetSenderAddress(string rlp, Chain chain)
        {
            return GetSenderAddress(rlp, (int)chain);
        }

        public string GetSenderAddress(string rlp, BigInteger chainId)
        {
            var transaction = new CeloTransactionChainId(rlp.HexToByteArray(), chainId);
            return transaction.Key.GetPublicAddress();
        }

        public bool VerifyTransaction(string rlp, Chain chain)
        {
            return VerifyTransaction(rlp, (int)chain);
        }

        public bool VerifyTransaction(string rlp, BigInteger chainId)
        {
            var transaction = new CeloTransactionChainId(rlp.HexToByteArray(), chainId);
            return transaction.Key.VerifyAllowingOnlyLowS(transaction.RawHash, transaction.Signature);
        }

    



        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, nonce);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, nonce);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, nonce, data);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, nonce, data);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, nonce, gasPrice, gasLimit);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, nonce, gasPrice, gasLimit);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, nonce);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, nonce, data);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, data, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, nonce, gasPrice, gasLimit);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, data, chainId);
            return SignTransaction(privateKey, transaction);
        }



        private string SignTransaction(byte[] privateKey, CeloTransactionChainId transaction)
        {
            transaction.Sign(new EthECKey(privateKey, true));
            return transaction.GetRLPEncoded().ToHex();
        }


        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, data);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit);
        }

        public string SignTransaction(string privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey, new BigInteger((int)chain), to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(string privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey.HexToByteArray(), chainId, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce,feeCurrency, gatewayFeeRecipient, gatewayFee, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, data);
        }



        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit);
        }


        public string SignTransaction(byte[] privateKey, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransaction(privateKey, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit, data);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, feeCurrency, gatewayFeeRecipient, gatewayFee, data, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, data, chainId);
            return SignTransaction(privateKey, transaction);
        }

        public string SignTransaction(byte[] privateKey, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, chainId);
            return SignTransaction(privateKey, transaction);
        }


#if !DOTNET35


        private async Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, CeloTransactionChainId transaction)
        {
            await transaction.SignExternallyAsync(externalSigner).ConfigureAwait(false);
            return transaction.GetRLPEncoded().ToHex();
        }

       public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount,
            BigInteger nonce)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, nonce);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, nonce, data);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, data, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, nonce, gasPrice, gasLimit);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, nonce, gasPrice, gasLimit, data);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, data, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }


        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, feeCurrency, gatewayFeeRecipient, gatewayFee, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee,  nonce, data);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, feeCurrency, gatewayFeeRecipient, gatewayFee, data, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, Chain chain, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            return SignTransactionAsync(externalSigner, (int)chain, to, amount, feeCurrency, gatewayFeeRecipient, gatewayFee, nonce, gasPrice, gasLimit, data);
        }

        public Task<string> SignTransactionAsync(ICeloEthExternalSigner externalSigner, BigInteger chainId, string to, BigInteger amount, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee,
            BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data)
        {
            var transaction = new CeloTransactionChainId(to, amount, nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, data, chainId);
            return SignTransactionAsync(externalSigner, transaction);
        }
#endif

    }
}
