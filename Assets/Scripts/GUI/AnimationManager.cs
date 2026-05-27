using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using InitScriptName;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Solana.Unity.Extensions;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using System.Linq;
using Solana.Unity.SDK;


public class AnimationManager : MonoBehaviour
{
    public bool PlayOnEnable = true;
    Dictionary<string, string> parameters;

    void OnEnable()
    {
        //这里对平台做个判断，Windows平台单独处理一下分辨率
#if UNITY_STANDALONE_WIN
        Screen.SetResolution(360, 540, false);
#endif

        if (PlayOnEnable)
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[0]);
        }
        if (name == "MenuPlay")
        {
            for (int i = 1; i <= 3; i++)
            {
                transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
            }
            int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", PlayerPrefs.GetInt("OpenLevel")), 0);
            if (stars > 0)
            {
                for (int i = 1; i <= stars; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
                }

            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
                }

            }

        }

        if (name == "Settings" || name == "MenuPause")
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Sound/SoundOff").gameObject.SetActive(false);

            if (PlayerPrefs.GetInt("Music") == 0)
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(true);
            else
                transform.Find("Image/Music/MusicOff").gameObject.SetActive(false);

        }

    }
    void OnDisable()
    {
        //if( PlayOnEnable )
        //{
        //    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
        //        GetComponent<SequencePlayer>().sequenceArray[0].Play
        //}
    }




    public void OnFinished()
    {
        if (name == "MenuComplete")
        {
            StartCoroutine(MenuComplete());
            StartCoroutine(MenuCompleteScoring());
        }
        if (name == "MenuPlay")
        {
            InitScript.Instance.currentTarget = LevelData.GetTarget(PlayerPrefs.GetInt("OpenLevel"));

        }

    }



    IEnumerator MenuComplete()
    {
        for (int i = 1; i <= MainScript.Instance.stars; i++)
        {
            //  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
            transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().hit);
        }
    }
    IEnumerator MenuCompleteScoring()
    {
        Text scores = transform.Find("Image").Find("Scores").GetComponent<Text>();
        for (int i = 0; i <= MainScript.Score; i += 500)
        {
            scores.text = "" + i;
            // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
            yield return new WaitForSeconds(0.00001f);
        }
        scores.text = "" + MainScript.Score;
    }


    public void PlaySoundButton()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);

    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseMenu()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (gameObject.name == "MenuPreGameOver")
        {
            ShowGameOver();
        }
        if (gameObject.name == "MenuComplete")
        {
            SceneManager.LoadScene("Map");
        }
        if (gameObject.name == "MenuGameOver")
        {
            SceneManager.LoadScene("Map");
        }

        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "game")
        {
            if (GamePlay.Instance.GameStatus == GameState.Pause)
            {
                GamePlay.Instance.GameStatus = GameState.WaitAfterClose;

            }
        }
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().swish[1]);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 当各种界面上的和Play相关的按钮按下时调用的方法
    /// </summary>
    public void Play()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        //根据绑定的不同GameObejct有不同的功能
        if (gameObject.name == "MenuPreGameOver")
        {
            if (InitScript.Gems >= 6)
            {
                InitScript.Instance.SpendGems(6);
                LevelData.LimitAmount += 6;
                GamePlay.Instance.GameStatus = GameState.WaitAfterClose;
                gameObject.SetActive(false);

            }
            else
            {
                BuyGames();
            }
        }
        else if (gameObject.name == "MenuGameOver")
        {
            SceneManager.LoadScene("Map");
        }
        else if (gameObject.name == "MenuPlay")
        {

            if (InitScript.Lifes > 0)
            {
                InitScript.Instance.SpendLife(1);

                SceneManager.LoadScene("Game");
            }
            else
            {
                BuyLifeShop();
            }

        }
        //如果按下Play按钮，则加载Map场景
        else if (gameObject.name == "PlayMain")
        {
            SceneManager.LoadScene("Map");
        }
    }

    public void PlayTutorial()
    {
        //        SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.click );
        GamePlay.Instance.GameStatus = GameState.Playing;
        //    MainScript.Instance.dropDownTime = Time.time + 0.5f;
        //        CloseMenu();
    }

    public void Next()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        CloseMenu();
    }
    public void BuyGames()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        GameObject.Find("Canvas").transform.Find("GemsShop").gameObject.SetActive(true);
    }
    private const string myAddress = "8f52Mczxf4H9pcoNvRBJwAFsKhTSGhHpeA2qgF2wpSJ1";
    private const string SKRMintWallet = "SKRbvo6Gf7GondiT3BbTfuRDPqLWei4j2Qy2NPGZhW3";
    public async void BuyGemsWithSkr(ulong value, Action rewardAction)
    {
        if (Web3.Instance.WalletBase == null)
        {
            NativeToast.Show("请先连接钱包");

            try
            {
                // 现场发起连接！
                await Web3.Instance.LoginWalletAdapter();
                NativeToast.Show("钱包已连接");

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

            }
        }

        // 2. 代码走到这，说明钱包肯定连上了，继续原来的支付逻辑
        try
        {

            // 发起代币转账
            var result = await Web3.Instance.WalletBase.Transfer(new PublicKey(myAddress), new PublicKey(SKRMintWallet), value);
            Debug.Log($"支付请求完成，状态: {result.WasSuccessful}");
            if (result.WasSuccessful)
            {
                var signature = result.Result;

                NativeToast.Show("支付成功!感谢支持！");
                // 支付成功，立即生效并跳转
                rewardAction?.Invoke();
                CloseMenu(); // 顺便关界面
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
        

    
    public void Buy(GameObject pack)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        Action onSuccess = () => InitScript.Instance.PurchaseSucceded();
        // 1. 动态提取 UI 上的宝石数
        int gemCount = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x", "").Trim());
        InitScript.waitedPurchaseGems = gemCount;

        ulong amount = 0;

        // 根据礼包设置 SKR 金额 (注意：这里假设 SKR 是 6 位精度)
        if (pack.name == "Pack1") amount = 1 * 1000000;
        else if (pack.name == "Pack2") amount = 10 * 1000000;
        else if (pack.name == "Pack3") amount = 20 * 1000000;
        else if (pack.name == "Pack4") amount = 30 * 1000000;

        if (PlayerPrefs.GetInt("IsVipUser", 0) == 1)
        {
            
            NativeToast.Show("已支持，直接发奖！");
            onSuccess?.Invoke(); // VIP 直接跳过广告逻辑，调回调
            CloseMenu(); // 顺便关界面
            return;
        }
        if (amount > 0)
        {
            BuyGemsWithSkr(amount, onSuccess);
        }
    }



    /// <summary>
    /// 广告播放完成后的回调机制
    /// </summary>



    public void BuyLifeShop()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Lifes < InitScript.CapOfLife)
            GameObject.Find("Canvas").transform.Find("LiveShop").gameObject.SetActive(true);

    }
    public void BuyLife(GameObject button)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text.Replace("x", "").Trim()))
        {
            InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text.Replace("x", "").Trim()));
            InitScript.Instance.AddLife(1);
            CloseMenu();
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("GemsShop").gameObject.SetActive(true);
        }

    }



    void ShowGameOver()
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().gameOver);

        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

    #region 设置相关
    public void ShowSettings(GameObject menuSettings)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (!menuSettings.activeSelf)
        {
            menuSettings.SetActive(true);
            if (GamePlay.Instance != null)
            {
                GamePlay.Instance.GameStatus = GameState.Pause;
            }
            //menuSettings.GetComponent<SequencePlayer>().Play();
        }
        else
        {
            menuSettings.SetActive(false);
            if (GamePlay.Instance != null)
            {
                GamePlay.Instance.GameStatus = GameState.Playing;
            }
        }
    }

    public void SoundOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().volume = 0;
            InitScript.sound = false;

            Off.SetActive(true);
        }
        else
        {
            SoundBase.GetInstance().GetComponent<AudioSource>().volume = 1;
            InitScript.sound = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Sound", (int)SoundBase.GetInstance().GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }
    public void MusicOff(GameObject Off)
    {
        if (!Off.activeSelf)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            InitScript.music = false;

            Off.SetActive(true);
        }
        else
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.music = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Music", (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void Info()
    {
        if (SceneManager.GetActiveScene().name == "Map" || SceneManager.GetActiveScene().name == "menu")
            GameObject.Find("Canvas").transform.Find("Tutorial").gameObject.SetActive(true);
        else
            GameObject.Find("Canvas").transform.Find("PreTutorial").gameObject.SetActive(true);
    }

    public void Quit()
    {
        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "game")
            SceneManager.LoadScene("Map");
        else
            SceneManager.LoadScene("开局");
    }



    #endregion

    #region 道具物品

    public void FiveBallsBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.FiveBallsBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.FiveBallsBoost);
        }
        else
        {
            OpenBoostShop(BoostType.FiveBallsBoost);
        }
    }
    public void ColorBallBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.ColorBallBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.ColorBallBoost);
        }
        else
        {
            OpenBoostShop(BoostType.ColorBallBoost);
        }

    }
    public void FireBallBoost()
    {
        if (GamePlay.Instance.GameStatus != GameState.Playing) return;
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Instance.FireBallBoost > 0)
        {
            if (GamePlay.Instance.GameStatus == GameState.Playing)
                InitScript.Instance.SpendBoost(BoostType.FireBallBoost);
        }
        else
        {
            OpenBoostShop(BoostType.FireBallBoost);
        }

    }

    public void OpenBoostShop(BoostType boosType)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        GameObject.Find("Canvas").transform.Find("BoostShop").gameObject.GetComponent<BoostShop>().SetBoost(boosType);
    }

    public void BuyBoost(BoostType boostType, int price)
    {
        SoundBase.GetInstance().GetComponent<AudioSource>().PlayOneShot(SoundBase.GetInstance().click);
        if (InitScript.Gems >= price)
        {
            InitScript.Instance.BuyBoost(boostType, 1, price);
            InitScript.Instance.SpendBoost(boostType);
            CloseMenu();
        }

        else
        {
            BuyGames();
        }
    }


    #endregion




}


   