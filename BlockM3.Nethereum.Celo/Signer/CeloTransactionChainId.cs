using System;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Model;
using Nethereum.RLP;
using Nethereum.Signer;
using System.Threading.Tasks;

namespace BlockM3.Nethereum.Celo.Signer
{
    public class CeloTransactionChainId : CeloSignedTransactionBase
    {
                //The R and S Hashing values
        private static readonly byte[] RHASH_DEFAULT = 0.ToBytesForRLPEncoding();
        private static readonly byte[] SHASH_DEFAULT = 0.ToBytesForRLPEncoding();

        public CeloTransactionChainId(byte[] rawData, BigInteger chainId)
        {
            //Instantiate and decode
            SimpleRlpSigner = new RLPSigner(rawData, NUMBER_ENCODING_ELEMENTS);
            ValidateValidV(SimpleRlpSigner);
            AppendDataForHashRecovery(chainId);
        }

        public CeloTransactionChainId(RLPSigner rlpSigner)
        {
            SimpleRlpSigner = rlpSigner;
            ValidateValidV(SimpleRlpSigner);
            GetChainIdFromVAndAppendDataForHashRecovery();
        }

        private static void ValidateValidV(RLPSigner rlpSigner)
        {
            if (!rlpSigner.IsVSignatureForChain())
                throw new Exception("Transaction should be used instead of TransactionChainId, invalid V");
        }
        private void GetChainIdFromVAndAppendDataForHashRecovery()
        {
            var chainId = GetChainFromVChain();
            AppendDataForHashRecovery(chainId);
        }

        private void AppendDataForHashRecovery(BigInteger chainId)
        {
            //append the chainId, r and s so it can be recovered using the raw hash
            //the encoding has only the default 6 values
            SimpleRlpSigner.AppendData(chainId.ToBytesForRLPEncoding(), RHASH_DEFAULT,
                SHASH_DEFAULT);
        }

        public CeloTransactionChainId(byte[] rawData)
        {
            //Instantiate and decode
            SimpleRlpSigner = new RLPSigner(rawData, NUMBER_ENCODING_ELEMENTS);
            ValidateValidV(SimpleRlpSigner);
            GetChainIdFromVAndAppendDataForHashRecovery();
        }

        private BigInteger GetChainFromVChain()
        {
            return EthECKey.GetChainFromVChain(Signature.V.ToBigIntegerFromRLPDecoded());
        }



        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, BigInteger chainId) 
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, chainId)
        {
        }

        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, string data, BigInteger chainId) 
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, data, chainId)
        {
        }

        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, BigInteger chainId)
            : this(to, amount, nonce, gasPrice, gasLimit, "", chainId)
        {
        }

        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data, BigInteger chainId) : this(to,amount,nonce,null,null,DEFAULT_GATEWAY_FEE,data,chainId)
        {
        }

        public CeloTransactionChainId(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] chainId)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId), NUMBER_ENCODING_ELEMENTS);
        }

        public CeloTransactionChainId(byte[] nonce, byte[] gasPrice, byte[] gasLimit,  byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] chainId, byte[] r, byte[] s, byte[] v)
        {
            SimpleRlpSigner = new RLPSigner(
                GetElementsInOrder(nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId),
                r, s, v, NUMBER_ENCODING_ELEMENTS);
        }
        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, BigInteger chainId)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, feeCurrency, gatewayFeeRecipient, gatewayFee, chainId)
        {
        }
        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, string data, BigInteger chainId)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, feeCurrency, gatewayFeeRecipient, gatewayFee, data, chainId)
        {
        }
        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, BigInteger chainId)
            : this(to, amount, nonce, gasPrice, gasLimit,  feeCurrency, gatewayFeeRecipient, gatewayFee, "", chainId)
        {
        }

        public CeloTransactionChainId(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, string data, BigInteger chainId) : this(nonce.ToBytesForRLPEncoding(),
            gasPrice.ToBytesForRLPEncoding(),
            gasLimit.ToBytesForRLPEncoding(), feeCurrency.HexToByteArray(), gatewayFeeRecipient.HexToByteArray(), gatewayFee.ToBytesForRLPEncoding(), to.HexToByteArray(), amount.ToBytesForRLPEncoding(),
            data.HexToByteArray(), chainId.ToBytesForRLPEncoding()
        )
        {
        }

        public BigInteger GetChainIdAsBigInteger()
        {
            return ChainId.ToBigIntegerFromRLPDecoded();
        }

        public byte[] ChainId => SimpleRlpSigner.Data[9];

        public byte[] RHash => SimpleRlpSigner.Data[10];

        public byte[] SHash => SimpleRlpSigner.Data[11];

        /// <summary>
        /// Recovered Key from Signature
        /// </summary>
        public override EthECKey Key => EthECKey.RecoverFromSignature(SimpleRlpSigner.Signature,
            SimpleRlpSigner.RawHash,
            ChainId.ToBigIntegerFromRLPDecoded());

       public string ToJsonHex()
        {
            var data =
                $"['{Nonce.ToHex()}','{GasPrice.ToHex()}','{GasLimit.ToHex()}','{FeeCurrency.ToHex()}','{GatewayFeeRecipient.ToHex()}','{GatewayFee.ToHex()}','{ReceiveAddress.ToHex()}','{Value.ToHex()}','{ToHex(Data)}','{ChainId.ToHex()}','{RHash.ToHex()}','{SHash.ToHex()}'";

            if (Signature != null)
                data = data + $", '{Signature.V.ToHex()}', '{Signature.R.ToHex()}', '{Signature.S.ToHex()}'";
            return data + "]";
        }

        public override void Sign(EthECKey key)
        {
            SimpleRlpSigner.Sign(key, GetChainIdAsBigInteger());
        }

        private byte[][] GetElementsInOrder(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress,
            byte[] value, byte[] data, byte[] chainId)
        {
            if (receiveAddress == null)
                receiveAddress = DefaultValues.EMPTY_BYTE_ARRAY;
            if (feeCurrency == null || feeCurrency.Length==0)
                feeCurrency = DefaultValues.EMPTY_BYTE_ARRAY;
            if (gatewayFeeRecipient == null || gatewayFeeRecipient.Length==0)
                gatewayFeeRecipient = DefaultValues.EMPTY_BYTE_ARRAY;
            //order  nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId, r = 0, s =0
            return new[]
            {
                nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId, RHASH_DEFAULT,
                SHASH_DEFAULT
            };
        }

#if !DOTNET35
        public override async Task SignExternallyAsync(ICeloEthExternalSigner externalSigner)
        {
             await externalSigner.SignAsync(this).ConfigureAwait(false);
        }
#endif
    }
}