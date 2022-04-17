using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 入力された表情データ、発話内容データをもとに、
/// 自作のコンソールアプリを実行します
/// するとCevioAIがしゃべります
/// </summary>
public class CevioTestScr : MonoBehaviour
{
    //読み上げテキスト
    public string Message = "こんにちは";
    //感情を設定(小春六花用)
    public int happy, normal, angry, sorrow, calm;
    //実行可能間隔（秒）
    public float WaitTime = 1.00F;
    [SerializeField]
    private Slider happySlider, normalSlider, angrySlider, sorrowSlider, calmSlider;

    //CevioAI用.NETアプリを相対パスで取得
    private string exeName = "CevioNET.exe";
    private string localPath;

    private Process process;
    private float exedtime = 1.00F;

    private void Start()
    {
        if(localPath == null)
        {
            localPath = Path.Combine(Application.streamingAssetsPath, exeName);
        }
    }

    public void RikkaTalk(string messageInput)
    {
        if(Time.time - exedtime > WaitTime) {
            //プロセス作成
            process = new Process ();
            process.StartInfo.FileName = localPath;
            process.StartInfo.Arguments = messageInput +
                    " " + happy + " " + normal + " " + angry + " " + sorrow + " " + calm;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            //実行
            process.Start ();
            //実行時間を記録
            exedtime = Time.time;
        }
        else
        {
            UnityEngine.Debug.Log("連続入力はできません");
        }
    }

    public void SetHappy()
    {
        happy = (int)happySlider.value;
    }
    public void SetNormal()
    {
        normal = (int)normalSlider.value;
    }
    public void SetAngry()
    {
        angry = (int)angrySlider.value;
    }
    public void SetSorrow()
    {
        sorrow = (int)sorrowSlider.value;
    }
    public void SetCalm()
    {
        calm = (int)calmSlider.value;
    }
    //冷静に考えたらコレupdateで常にvalue取ればよかったんじゃ...
    //まあいいか
}
