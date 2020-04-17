using Nethereum.Web3.Accounts;
using System.Numerics;
using Nethereum.KeyStore;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.NonceServices;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;

namespace BlockM3.Nethereum.Celo.Accounts
{
    public class CeloAccount : Account
    {
        
        public string FeeCurrency {get; set;}
        public string GatewayFeeRecipient {get;  set;}
        public BigInteger? GatewayFee { get; set; }


        public static CeloAccount LoadFromKeyStore(string json, string password, BigInteger? chainId = null)
        {
            var keyStoreService = new KeyStoreService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, json);
            return new CeloAccount(key, chainId);
        }


        public CeloAccount(EthECKey key, BigInteger? chainId=null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(key, chainId)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            //mpiva called again, base constructor will call it without the above parameters
            InitialiseDefaultTransactionManager();
        }

        public CeloAccount(string privateKey, BigInteger? chainId=null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(privateKey, chainId)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            InitialiseDefaultTransactionManager();
        }

        public CeloAccount(byte[] privateKey, BigInteger? chainId=null, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(privateKey, chainId)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            InitialiseDefaultTransactionManager();
        }

        public CeloAccount(EthECKey key, Chain chain, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(key, chain)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            InitialiseDefaultTransactionManager();
        }

        public CeloAccount(string privateKey, Chain chain, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(privateKey, chain)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            InitialiseDefaultTransactionManager();
        }

        public CeloAccount(byte[] privateKey, Chain chain, string feeCurrency=null, string gatewayFeeRecipient=null, BigInteger? gatewayFee=null) : base(privateKey, chain)
        {
            FeeCurrency = feeCurrency;
            GatewayFeeRecipient = gatewayFeeRecipient;
            GatewayFee = gatewayFee;
            InitialiseDefaultTransactionManager();
        }

        protected override void InitialiseDefaultTransactionManager()
        {
            TransactionManager = new CeloAccountSignerTransactionManager(null, this, ChainId, FeeCurrency, GatewayFeeRecipient, GatewayFee);
        }
    }
}
