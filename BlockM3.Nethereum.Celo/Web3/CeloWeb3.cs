using System;
using System.Net.Http.Headers;
using BlockM3.Nethereum.Celo.Accounts;
using BlockM3.Nethereum.Celo.Signer;
using Common.Logging;
using Nethereum.BlockchainProcessing.Services;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;

namespace BlockM3.Nethereum.Celo.Web3
{
    public class CeloWeb3 : IWeb3
    {
        private static readonly AddressUtil addressUtil = new AddressUtil();
        private static readonly Sha3Keccack sha3Keccack = new Sha3Keccack();

        public CeloWeb3(IClient client)
        {
            Client = client;
            InitialiseInnerServices();
            IntialiseDefaultGasAndGasPrice();
        }

        public CeloWeb3(CeloAccount account, IClient client) : this(client)
        {
            TransactionManager = account.TransactionManager;
            TransactionManager.Client = Client;
        }

        public CeloWeb3(string url = @"http://localhost:8545/", ILog log = null, AuthenticationHeaderValue authenticationHeader = null)
        {
            IntialiseDefaultRpcClient(url, log, authenticationHeader);
            InitialiseInnerServices();
            IntialiseDefaultGasAndGasPrice();
        }

        public CeloWeb3(CeloAccount account, string url = @"http://localhost:8545/", ILog log = null, AuthenticationHeaderValue authenticationHeader = null) : this(url, log, authenticationHeader)
        {
            TransactionManager = account.TransactionManager;
            TransactionManager.Client = Client;
        }

        public ITransactionManager TransactionManager
        {
            get => Eth.TransactionManager;
            set => Eth.TransactionManager = value;
        }

        public static UnitConversion Convert { get; } = new UnitConversion();

        public static CeloTransactionSigner OfflineTransactionSigner { get; } = new CeloTransactionSigner();

        public IClient Client { get; private set; }

        public IEthApiContractService Eth { get; private set; }
        public IShhApiService Shh { get; private set; }
        public INetApiService Net { get; private set; }
        public IPersonalApiService Personal { get; private set; }
        public IBlockchainProcessingService Processing { get; private set; }

        private void IntialiseDefaultGasAndGasPrice()
        {
            TransactionManager.DefaultGas = Transaction.DEFAULT_GAS_LIMIT;
            TransactionManager.DefaultGasPrice = Transaction.DEFAULT_GAS_PRICE;
        }

        public static string GetAddressFromPrivateKey(string privateKey)
        {
            return EthECKey.GetPublicAddress(privateKey);
        }

        public static bool IsChecksumAddress(string address)
        {
            return addressUtil.IsChecksumAddress(address);
        }

        public static string Sha3(string value)
        {
            return sha3Keccack.CalculateHash(value);
        }

        public static string ToChecksumAddress(string address)
        {
            return addressUtil.ConvertToChecksumAddress(address);
        }

        public static string ToValid20ByteAddress(string address)
        {
            return addressUtil.ConvertToValid20ByteAddress(address);
        }

        protected virtual void InitialiseInnerServices()
        {
            Eth = new EthApiContractService(Client);
            Processing = new BlockchainProcessingService(Eth);
            Shh = new ShhApiService(Client);
            Net = new NetApiService(Client);
            Personal = new PersonalApiService(Client);
        }

        private void IntialiseDefaultRpcClient(string url, ILog log, AuthenticationHeaderValue authenticationHeader)
        {
            Client = new RpcClient(new Uri(url), authenticationHeader, null, null, log);
        }
    }
}
