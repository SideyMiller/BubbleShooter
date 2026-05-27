
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Solana.Unity.SDK; // 核心引用
using Solana.Unity.Rpc.Models;
using System;

// ReSharper disable once CheckNamespace

public class WalletHolder : MonoBehaviour 
{ 
    public Button toggleWallet_btn;
    public Button bgainGame;
    public GameObject wallet;

    void Start()
    {
        // 1. 如果已经是 VIP，直接闪人进游戏
        if (PlayerPrefs.GetInt("IsVipUser", 0) == 1)
        {
            
            Loading.StartLoading();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            NativeToast.Show("身份确认，跳过钱包连接！");
            SceneManager.LoadScene("Menu");
            
            return;
        }
        wallet.SetActive(false);



#if UNITY_EDITOR
    Debug.Log("[Test] 编辑器模式：跳过链接钱包，打开界面");
    toggleWallet_btn.onClick.AddListener(async () => {
            
                    wallet.SetActive(true);
                
        });
#endif
        toggleWallet_btn.onClick.AddListener(async () => {
            if (!wallet.activeSelf)
            {
                toggleWallet_btn.interactable = false;
                Loading.StartLoading();
                try
                {
                    // 这里使用了 await，逻辑正确
                    await Web3.Instance.LoginWalletAdapter();
                    NativeToast.Show("钱包连接成功！");
                    wallet.SetActive(true);
                }
                catch (System.Exception e)
                {
                    string errorMsg = e.Message.ToLower();

                    if (errorMsg.Contains("reject") || errorMsg.Contains("cancel") || errorMsg.Contains("denied"))
                    {
                        // 玩家自己关掉了钱包窗口，或者是点了拒绝
                        NativeToast.Show("已取消连接");
                    }
                    else
                    {
                        // 哪怕是网络断了、没装钱包、超时，统统报失败
                        NativeToast.Show("连接失败，请重试");
                    }
                    Loading.StopLoading(); // 假设你有这个方法
                    toggleWallet_btn.interactable = true;
                }
                
            }
        });

        bgainGame.onClick.AddListener(() => {
            Loading.StartLoading();
            SceneManager.LoadScene("Menu");

        });
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
