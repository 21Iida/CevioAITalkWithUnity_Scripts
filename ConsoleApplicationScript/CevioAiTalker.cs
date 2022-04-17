using CeVIO.Talk.RemoteService2;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace CevioNET
{
    /// <summary>
    /// CeVIO Aiをコンソールから動かすためのアプリです
    /// 動かし方は "喋らせたい内容" 嬉しさ 普通 怒り 哀しみ 落ち着き の順に入力します
    /// 実行するとPCにインストール済みのCeVIO AIが喋ってくれます
    /// また、コンソールにはテキストの音素データが表示されます
    /// その音素データをNamedPipeでUnityと通信します
    /// 
    /// CeVIO AIのDLLについては以下URL参照
    /// <https://cevio.jp/guide/cevio_ai/interface/dotnet/>
    /// </summary>
    class CevioAiTalker
    {
        static void Main(string[] args)
        {
            // 【CeVIO AI】起動
            ServiceControl2.StartHost(false);

            // Talkerインスタンス生成
            Talker2 talker = new Talker2();

            // キャスト設定
            talker.Cast = "小春六花";

            // 音量設定
            talker.Volume = 50;

            // 話す速さ
            talker.Speed = 50;

            // 抑揚設定
            talker.ToneScale = 50;

            //共有するための文字列
            string phonemeDataList = "";

            // 感情値を設定する
            // 順に "嬉しい", "普通", "怒り", "哀しみ", "落ち着き"
            for (int i = 0; i < talker.Components.Length; i++)
            {
                TalkerComponent2 talkerComponent2 = talker.Components[i];
                uint emotion = uint.Parse(args[i + 1]);
                talkerComponent2.Value = emotion;
            }

            //音素データ
            foreach (var item in talker.GetPhonemes(args[0]))
            {
                System.Console.Write(item.Phoneme);
                System.Console.WriteLine("," + item.EndTime);
                //後で修正ポイント。StartTimeよりもEndTimeを取得した方がリップシンクにはいい感じ
                phonemeDataList += item.Phoneme + "," + item.EndTime + ",";
            }

            //NamedPipeの参考サイト<https://yotiky.hatenablog.com/entry/dotnet_namedpipe>
            Task.Run(async () =>
            {
                try
                {
                    using (var stream = new NamedPipeServerStream("PhonemePipe"))
                    {
                        await stream.WaitForConnectionAsync();

                        using (var writer = new StreamWriter(stream))
                        {
                            writer.AutoFlush = true;
                            //while (true)
                            {
                                await writer.WriteLineAsync(phonemeDataList);
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Connection is closed.");
                }
            });

            // 再生
            SpeakingState2 state = talker.Speak(args[0]);

            state.Wait();
        }
    }
}
