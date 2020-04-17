using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Model;
using Nethereum.RLP;
using Nethereum.Signer;
using System.Threading.Tasks;

namespace BlockM3.Nethereum.Celo.Signer
{
    public class CeloTransaction : CeloSignedTransactionBase
    {
        public CeloTransaction(byte[] rawData)
        {
            SimpleRlpSigner = new RLPSigner(rawData, NUMBER_ENCODING_ELEMENTS);
            ValidateValidV(SimpleRlpSigner);
        }

        public CeloTransaction(RLPSigner rlpSigner)
        {
            ValidateValidV(rlpSigner);
            SimpleRlpSigner = rlpSigner;
        }

        private static void ValidateValidV(RLPSigner rlpSigner)
        {
            if (rlpSigner.IsVSignatureForChain())
                throw new Exception("TransactionChainId should be used instead of Transaction");
        }

        public CeloTransaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] receiveAddress, byte[] value,
            byte[] data)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, receiveAddress, value, data));
        }

        public CeloTransaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] r, byte[] s, byte v)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, receiveAddress, value, data),
                r, s, v);
        }
        public CeloTransaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] chainId)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId), NUMBER_ENCODING_ELEMENTS);
        }

        public CeloTransaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit,  byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] chainId, byte[] r, byte[] s, byte[] v)
        {
            SimpleRlpSigner = new RLPSigner(
                GetElementsInOrder(nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data, chainId),
                r, s, v, NUMBER_ENCODING_ELEMENTS);
        }


        public CeloTransaction(string to, BigInteger amount, BigInteger nonce)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT)
        {
        }

        public CeloTransaction(string to, BigInteger amount, BigInteger nonce, string data)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, data)
        {
        }

        public CeloTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit)
            : this(to, amount, nonce, gasPrice, gasLimit, "")
        {
        }

        public CeloTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data) : this(to,amount,nonce,gasPrice,null,null,DEFAULT_GATEWAY_FEE,data)
        {
        }
        
        public CeloTransaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, string data) : this(nonce.ToBytesForRLPEncoding(),
            gasPrice.ToBytesForRLPEncoding(),
            gasLimit.ToBytesForRLPEncoding(), feeCurrency.HexToByteArray(), gatewayFeeRecipient.HexToByteArray(), gatewayFee.ToBytesForRLPEncoding(), to.HexToByteArray(), amount.ToBytesForRLPEncoding(),
            data.HexToByteArray()
        )
        {
        }


        public string ToJsonHex()
        {
            var s = "['{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}']";
            return string.Format(s, Nonce.ToHex(),
                GasPrice.ToHex(), GasLimit.ToHex(), FeeCurrency.ToHex(), GatewayFeeRecipient.ToHex(), GatewayFee.ToHex(), ReceiveAddress.ToHex(), Value.ToHex(), ToHex(Data),
                Signature.V.ToHex(),
                Signature.R.ToHex(),
                Signature.S.ToHex());
        }

        private byte[][] GetElementsInOrder(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] feeCurrency, byte[] gatewayFeeRecipient, byte[] gatewayFee, byte[] receiveAddress,
            byte[] value,
            byte[] data)
        {
            if (receiveAddress == null)
                receiveAddress = DefaultValues.EMPTY_BYTE_ARRAY;
            if (feeCurrency == null || feeCurrency.Length==0)
                feeCurrency = DefaultValues.EMPTY_BYTE_ARRAY;
            if (gatewayFeeRecipient == null || gatewayFeeRecipient.Length==0)
                gatewayFeeRecipient = DefaultValues.EMPTY_BYTE_ARRAY;
            //order  nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data
            return new[] {nonce, gasPrice, gasLimit, feeCurrency, gatewayFeeRecipient, gatewayFee, receiveAddress, value, data};
        }

        public override EthECKey Key => EthECKey.RecoverFromSignature(SimpleRlpSigner.Signature, SimpleRlpSigner.RawHash);

#if !DOTNET35
        public override async Task SignExternallyAsync(ICeloEthExternalSigner externalSigner)
        {
           await externalSigner.SignAsync(this).ConfigureAwait(false);
        }
#endif
    }
}
