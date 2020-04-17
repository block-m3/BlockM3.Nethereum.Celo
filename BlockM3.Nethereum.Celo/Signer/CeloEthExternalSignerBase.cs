using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer.Crypto;
using Nethereum.Signer;



namespace BlockM3.Nethereum.Celo.Signer
{
#if !DOTNET35
    public abstract class CeloEthExternalSignerBase : EthExternalSignerBase, ICeloEthExternalSigner
    {
        public abstract Task SignAsync(CeloTransactionChainId transaction);

        public override Task SignAsync(TransactionChainId transaction)
        {
            throw new System.NotSupportedException();
        }
        public override Task SignAsync(Transaction transaction)
        {
           throw new System.NotSupportedException();
        }

        protected async Task SignHashTransactionAsync(CeloTransactionChainId transaction)
        {
            if (ExternalSignerTransactionFormat == ExternalSignerTransactionFormat.Hash)
            {
                var signature = await SignAsync(transaction.RawHash, transaction.GetChainIdAsBigInteger());
                transaction.SetSignature(signature);
            }
        }
        protected async Task SignRLPTransactionAsync(CeloTransactionChainId transaction)
        {
            if (ExternalSignerTransactionFormat == ExternalSignerTransactionFormat.RLP)
            {
                var signature = await SignAsync(transaction.RawHash, transaction.GetChainIdAsBigInteger());
                transaction.SetSignature(signature);
            }
        }
    }
#endif
}
