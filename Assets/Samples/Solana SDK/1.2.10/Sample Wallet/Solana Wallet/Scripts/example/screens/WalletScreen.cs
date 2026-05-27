using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using codebase.utility;
using Cysharp.Threading.Tasks;
using Solana.Unity.Extensions;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using UnityEngine.SceneManagement;
using InitScriptName;
using Solana.Unity.SDK;

// ReSharper disable once CheckNamespace

namespace Solana.Unity.SDK.Example
{
    public class WalletScreen : SimpleScreen
    {
        [SerializeField]
        private TextMeshProUGUI lamports;
        [SerializeField]
        private TextMeshProUGUI addressDisplay;
        [SerializeField]
        private Button sendBtn;
        [SerializeField]
        private Button nosendBtn;
        [SerializeField]
        private Button logoutBtn;
        private const string myAddress = "8f52Mczxf4H9pcoNvRBJwAFsKhTSGhHpeA2qgF2wpSJ1";
        private const string SKRMintWallet = "SKRbvo6Gf7GondiT3BbTfuRDPqLWei4j2Qy2NPGZhW3";

        static bool isVipUser = false; // 全局 VIP 状态


        public void Start()
        {


            logoutBtn.onClick.AddListener(() =>
            {
                Loading.StartLoading();
                Web3.Instance.Logout();
                gameObject.SetActive(false);
                Loading.StopLoading();

            });

            nosendBtn.onClick.AddListener(() => {
                Loading.StartLoading();
                SceneManager.LoadScene("Menu");

            });

            sendBtn.onClick.AddListener(() => { BuyVipWithSkr(); });
            Web3.OnWalletChangeState += OnWalletChangeState;
        }
        private void OnWalletChangeState() { }
        private async UniTask GetSkrBalance()
        {
            if (lamports == null) return;

            // 获取所有代币账号
            var tokens = await Web3.Wallet.GetTokenAccounts(Solana.Unity.Rpc.Types.Commitment.Confirmed);

            if (tokens != null)
            {
                // 找到 SKR 的那个账号
                var skrToken = tokens.FirstOrDefault(t => t.Account.Data.Parsed.Info.Mint == SKRMintWallet);

                MainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (skrToken != null)
                    {
                        lamports.text = $"{skrToken.Account.Data.Parsed.Info.TokenAmount.UiAmountString} SKR";
                    }
                    else
                    {
                        lamports.text = "0 SKR";
                    }
                });
            }
        }

        public async void BuyVipWithSkr()
        {
            Loading.StartLoading();
#if UNITY_EDITOR
    Debug.Log("[Test] 编辑器模式：跳过支付，直接发放vip");
    SetVipStatus(true);
    SceneManager.LoadScene("Menu");
    return; 
#endif
            try
            {

                ulong amount = 50 * 1000000; // 100个SKR (6位精度)

                // 发起代币转账
                var result = await Web3.Instance.WalletBase.Transfer(new PublicKey(myAddress), new PublicKey(SKRMintWallet), amount);
                Debug.Log($"支付请求完成，状态: {result.WasSuccessful}");
                if (result.WasSuccessful)
                {
                    var signature = result.Result;

                    NativeToast.Show("支付成功! 感谢支持！");
                    // 支付成功，立即生效并跳转
                    SetVipStatus(true);
                    SceneManager.LoadScene("Menu");
                }
                else
                {

                    NativeToast.Show("支付失败");
                }
            }
            catch (System.Exception e)
            {
                string errorMsg = e.Message.ToLower();

                if (errorMsg.Contains("reject") || errorMsg.Contains("cancel") || errorMsg.Contains("denied"))
                {

                    NativeToast.Show("支付取消");
                }
                else if (errorMsg.Contains("insufficient") || errorMsg.Contains("balance"))
                {

                    NativeToast.Show("支付失败");
                }
                else if (errorMsg.Contains("timeout"))
                {

                    NativeToast.Show("支付失败");
                }
                else
                {

                    NativeToast.Show("支付失败");
                }
            }
        }
            
            
        
        private void OnEnable()
        {
            Loading.StopLoading();


            // 每次打开界面，刷新一下数据
            if (Web3.Account != null)
            {
                UpdateAddressDisplay();     // 转换地址
                GetSkrBalance().Forget();   // 查 SKR 余额
            }
        }
        private void OnDisable()
        {

        }

        // --- 核心逻辑 2：地址转换显示 ---
        private void UpdateAddressDisplay()
        {
            if (addressDisplay == null) return;

            string fullKey = Web3.Account.PublicKey.ToString();
            // 截取前4位和后4位，中间加点
            string shortKey = $"{fullKey.Substring(0, 4)}...{fullKey.Substring(fullKey.Length - 4)}";

            // 如果你想显示 .skr，这里只是单纯的文本拼接。
            // 真正的域名解析需要复杂的 RPC，建议先用这个简写
            addressDisplay.text = shortKey;
        }



        private void OnDestroy()
        {
            if (_stopTask is null) return;
            _stopTask.Cancel();
        }

        private CancellationTokenSource _stopTask;

        void SetVipStatus(bool status)
        {
            isVipUser = status;
            // 持久化保存到本地设备
            PlayerPrefs.SetInt("IsVipUser", status ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"[System] VIP 状态已更新并保存: {status}");
        }
    }
}
