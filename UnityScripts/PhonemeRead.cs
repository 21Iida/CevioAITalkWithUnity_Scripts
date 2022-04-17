using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// コンソールアプリの実行結果をNamedPipeで受け取って整理します
/// </summary>
public class PhonemeRead : MonoBehaviour
{
    private CancellationTokenSource _tokenSource = new CancellationTokenSource();
    [SerializeField] private FaceMorph _faceMorph;
    
    private void Update()
    {
        var task = PhonemePipe();
    }

    async Task PhonemePipe()
    {
        await Task.Run(async () =>
        {
            using (var stream = new NamedPipeClientStream("PhonemePipe"))
            {
                stream.Connect();

                using (var reader = new StreamReader(stream))
                {
                    while (true)
                    {
                        var str = await reader.ReadLineAsync();
                        if (str == null)
                        {
                            break;
                        }
                        Debug.Log("Data:" + str);
                        _faceMorph.PhonemeOrganize(str);
                        Thread.Sleep(1000);
                        if (_tokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            }
            Debug.Log("End of Stream.");
        }, _tokenSource.Token);
    }

    private void OnDestroy()
    {
        _tokenSource.Cancel();
        Debug.Log("task cancelled.");
    }
}
