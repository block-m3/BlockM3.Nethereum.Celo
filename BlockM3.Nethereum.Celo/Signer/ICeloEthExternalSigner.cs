using Nethereum.Signer;
using System.Numerics;
using System.Threading.Tasks;

namespace BlockM3.Nethereum.Celo.Signer
{
    public interface ICeloEthExternalSigner
    {
        bool CalculatesV { get; }
        ExternalSignerTransactionFormat ExternalSignerTransactionFormat { get; }

        Task<string> GetAddressAsync();
        Task<EthECDSASignature> SignAsync(byte[] rawBytes);
        Task<EthECDSASignature> SignAsync(byte[] rawBytes, BigInteger chainId);
        Task SignAsync(CeloTransactionChainId transaction);
    }
}
