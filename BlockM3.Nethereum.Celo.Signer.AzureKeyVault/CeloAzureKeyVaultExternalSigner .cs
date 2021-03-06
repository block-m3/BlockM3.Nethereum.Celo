﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Nethereum.Signer.Crypto;
using Nethereum.Signer;

namespace BlockM3.Nethereum.Celo.Signer.AzureKeyVault
{
    public class CeloAzureKeyVaultExternalSigner : CeloEthExternalSignerBase
    {
        public override ExternalSignerTransactionFormat ExternalSignerTransactionFormat { get; protected set; } = ExternalSignerTransactionFormat.Hash;
        public override bool CalculatesV { get; protected set; } = false;
        public KeyVaultClient KeyVaultClient { get; private set; }
        public string VaultUrl { get; }

        public CeloAzureKeyVaultExternalSigner(KeyVaultClient keyVaultClient, string vaultUrl)
        {
            KeyVaultClient = keyVaultClient;
            VaultUrl = vaultUrl;
        }

        protected override async Task<byte[]> GetPublicKeyAsync()
        {
            var keyBundle = await KeyVaultClient.GetKeyAsync(VaultUrl);
            var xLen = keyBundle.Key.X.Length;
            var yLen = keyBundle.Key.Y.Length;
            var publicKey = new byte[1 + xLen + yLen];
            publicKey[0] = 0x04;
            var offset = 1;
            Buffer.BlockCopy(keyBundle.Key.X, 0, publicKey, offset, xLen);
            offset = offset + xLen;
            Buffer.BlockCopy(keyBundle.Key.Y, 0, publicKey, offset, yLen);
            return publicKey;
        }

        protected override async Task<ECDSASignature> SignExternallyAsync(byte[] hash)
        {
            var keyOperationResult = await KeyVaultClient.SignAsync(VaultUrl, "ECDSA256", hash);
            var signature = keyOperationResult.Result;
            return ECDSASignatureFactory.FromComponents(signature).MakeCanonical();
        }


        public override async Task SignAsync(CeloTransactionChainId transaction)
        {
            await SignHashTransactionAsync(transaction).ConfigureAwait(false);
        }
    }
}
