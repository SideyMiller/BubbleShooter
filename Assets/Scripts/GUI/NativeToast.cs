using UnityEngine;

public class NativeToast
{
    // 这里的 duration 只有两个选项：0 是短时间(约2秒)，1 是长时间(约3.5秒)
    public static void Show(string message, bool longDuration = false)
    {
        // 只在安卓真机上运行，防止在编辑器里报错
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("模拟 Toast: " + message);
            return;
        }

        // 必须在主线程调用 Android UI
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (currentActivity != null)
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText",
                    currentActivity,
                    message,
                    longDuration ? 1 : 0 // 1 = LENGTH_LONG, 0 = LENGTH_SHORT
                );
                toastObject.Call("show");
            }));
        }
    }
}