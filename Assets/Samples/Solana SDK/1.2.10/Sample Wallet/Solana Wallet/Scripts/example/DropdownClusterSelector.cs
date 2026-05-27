using Solana.Unity.SDK;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

// ReSharper disable once CheckNamespace

public class DropdownClusterSelector : MonoBehaviour
{
    void OnEnable()
    {
        int rpcDefault = PlayerPrefs.GetInt("rpcCluster", 0);
        RpcNodeDropdownSelected(rpcDefault);
        Web3.OnWalletInstance += () => RpcNodeDropdownSelected(rpcDefault);
        GetComponent<TMP_Dropdown>().value = rpcDefault;
    }
    
    public void RpcNodeDropdownSelected(int value)
    {
        if(Web3.Instance == null) return;
        Web3.Instance.rpcCluster = (RpcCluster) value;

        Web3.Instance.customRpc = "https://rpc.magicblock.app/mainnet/";
        Web3.Instance.webSocketsRpc = "wss://rpc.magicblock.app/mainnet/";
        PlayerPrefs.SetInt("rpcCluster", value);
        PlayerPrefs.Save();
        Web3.Instance.LoginXNFT().AsUniTask().Forget();
    }
}
