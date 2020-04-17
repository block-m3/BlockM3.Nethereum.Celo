using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer;

namespace BlockM3.Nethereum.Celo.Signer
{
    public class CeloTransactionFactory
    {
        public static CeloSignedTransactionBase CreateTransaction(string rlpHex)
        {
            return CreateTransaction(rlpHex.HexToByteArray());
        }

        public static CeloSignedTransactionBase CreateTransaction(byte[] rlp)
        {
            var rlpSigner = CeloSignedTransactionBase.CreateDefaultRLPSigner(rlp);
            return new CeloTransactionChainId(rlpSigner);
        }

        public static CeloSignedTransactionBase CreateTransaction(string to, BigInteger gas, BigInteger gasPrice,string feeCurrency, string gatewayFeeRecipient, BigInteger gatewayFee, BigInteger amount, string data, BigInteger nonce, string r, string s, string v)
        {
            var rBytes = r.HexToByteArray();
            var sBytes = s.HexToByteArray();
            var vBytes = v.HexToByteArray();
            var vBigInteger = vBytes.ToBigIntegerFromRLPDecoded();
            var chainId = EthECKey.GetChainFromVChain(vBigInteger);
            return new CeloTransactionChainId(nonce.ToBytesForRLPEncoding(), gasPrice.ToBytesForRLPEncoding(), gas.ToBytesForRLPEncoding(), feeCurrency.HexToByteArray(), gatewayFeeRecipient.HexToByteArray(),gatewayFee.ToBytesForRLPEncoding(),
                    to.HexToByteArray(), amount.ToBytesForRLPEncoding(), data.HexToByteArray(), chainId.ToBytesForRLPEncoding(), rBytes, sBytes, vBytes);
       
        }
    }
}
