﻿using Nethereum.Signer;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Model;
using Nethereum.Hex.HexConvertors.Extensions;

namespace BlockM3.Nethereum.Celo.Signer
{
    public abstract class CeloSignedTransactionBase
    {
        public static RLPSigner CreateDefaultRLPSigner(byte[] rawData)
        {
           return new RLPSigner(rawData, NUMBER_ENCODING_ELEMENTS);  
        }

        //Number of encoding elements (output for transaction)
        public const int NUMBER_ENCODING_ELEMENTS = 9;
        public static readonly BigInteger DEFAULT_GAS_PRICE = BigInteger.Parse("20000000000");
        public static readonly BigInteger DEFAULT_GAS_LIMIT = BigInteger.Parse("21000");
        public static readonly BigInteger DEFAULT_GATEWAY_FEE = BigInteger.Parse("0");

        protected RLPSigner SimpleRlpSigner { get; set; }

        public byte[] RawHash => SimpleRlpSigner.RawHash;

        /// <summary>
        ///     The counter used to make sure each transaction can only be processed once, you may need to regenerate the
        ///     transaction if is too low or too high, simples way is to get the number of transacations
        /// </summary>
        public byte[] Nonce => SimpleRlpSigner.Data[0] ?? DefaultValues.ZERO_BYTE_ARRAY;

        public byte[] Value => SimpleRlpSigner.Data[7] ?? DefaultValues.ZERO_BYTE_ARRAY;

        public byte[] ReceiveAddress => SimpleRlpSigner.Data[6];

        public byte[] GasPrice => SimpleRlpSigner.Data[1] ?? DefaultValues.ZERO_BYTE_ARRAY;

        public byte[] GasLimit => SimpleRlpSigner.Data[2];

        public byte[] FeeCurrency => SimpleRlpSigner.Data[3];

        public byte[] GatewayFeeRecipient => SimpleRlpSigner.Data[4];

        public byte[] GatewayFee => SimpleRlpSigner.Data[5];

        public byte[] Data => SimpleRlpSigner.Data[8];

        public EthECDSASignature Signature => SimpleRlpSigner.Signature;

        public abstract EthECKey Key { get;  }
            

        public byte[] GetRLPEncoded()
        {
            return SimpleRlpSigner.GetRLPEncoded();
        }

        public byte[] GetRLPEncodedRaw()
        {
            return SimpleRlpSigner.GetRLPEncodedRaw();
        }

        public virtual void Sign(EthECKey key)
        {
            SimpleRlpSigner.Sign(key);
        }

        public void SetSignature(EthECDSASignature signature)
        {
            SimpleRlpSigner.SetSignature(signature);
        }

        protected static string ToHex(byte[] x)
        {
            if (x == null) return "0x";
            return x.ToHex();
        }
#if !DOTNET35

        public abstract Task SignExternallyAsync(ICeloEthExternalSigner externalSigner);
#endif
    }
}
