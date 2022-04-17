using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// 口パクの実装です
/// </summary>
public class FaceMorph : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer mouthMorph;
    
    private List<PhonemePart> _phonemeParts = new List<PhonemePart>();

    //音声の再生と同期させることが現状難しいので、手入力で口パクの遅延を調整する
    [SerializeField] private double phonemeDelay = 0d;
    
    
    [Serializable]
    public class PhonemeMouthIndex
    {
        public int silPau;
        public int n;
        public int a, i, u, e, o;
    }
    [SerializeField]
    private PhonemeMouthIndex pmIndex = new PhonemeMouthIndex();

    private class PhonemeMouthValue
    {
        public int SilPau = 0;
        public int N = 0;
        public int A = 0, I = 0, U = 0, E = 0, O = 0;
    }
    private PhonemeMouthValue pmValue = new PhonemeMouthValue();

    class PhonemePart
    {
        public readonly string unit;
        public double time;

        public PhonemePart(string Unit, double Time)
        {
            unit = Unit;
            time = Time;
        }
    }
    void Start()
    {
        MorphUpdate();
    }

    private void LateUpdate()
    {
        MorphUpdate();
    }

    private void MorphUpdate()
    {
        mouthMorph.SetBlendShapeWeight(pmIndex.silPau,pmValue.SilPau);
        mouthMorph.SetBlendShapeWeight(pmIndex.n, pmValue.N);
        mouthMorph.SetBlendShapeWeight(pmIndex.a, pmValue.A);
        mouthMorph.SetBlendShapeWeight(pmIndex.i, pmValue.I);
        mouthMorph.SetBlendShapeWeight(pmIndex.u, pmValue.U);
        mouthMorph.SetBlendShapeWeight(pmIndex.e, pmValue.E);
        mouthMorph.SetBlendShapeWeight(pmIndex.o, pmValue.O);
    }

    public void MorphValueReset()
    {
        pmValue.SilPau = 0;
        pmValue.N = 0;
        pmValue.A = 0;
        pmValue.I = 0;
        pmValue.U = 0;
        pmValue.E = 0;
        pmValue.O = 0;
    }

    //文字列をクラスのリストに整理
    public void PhonemeOrganize(string phonemeStr)
    {
        var strUnit = phonemeStr.Split(',');
        string cacheStr = null;
        double cacheDouble = Double.NegativeInfinity;
        _phonemeParts = new List<PhonemePart>();
        
        foreach (var item in strUnit.Select((value,index) => (value,index)))
        {
            //クラスの要素を拾う
            if (item.index % 2 == 0)
            {
                cacheStr = item.value;
            }
            else
            {
                Double.TryParse(item.value, out cacheDouble);
            }

            //要素のセットがそろったらリストに追加
            if (!String.IsNullOrEmpty(cacheStr) && !cacheDouble.Equals(Double.NegativeInfinity))
            {
                _phonemeParts.Add(new PhonemePart(cacheStr,cacheDouble));
                cacheStr = null;
                cacheDouble = Double.NegativeInfinity;
            }
        }
        //本当はもっと違うところで読んだ方がいい気がする
        BlendShapeCommit();
    }

    //BlendShapeの数値を書き換える
    public void BlendShapeCommit()
    {
        //口パクの内容を考える部分
        //各要素の時間を累計時間から要素ごとの時間に変換
        for (int i = _phonemeParts.Count - 2; i >= 0; i--)
        {
            _phonemeParts[i + 1].time -= _phonemeParts[i].time;
        }
        //実際にモーフの数値を弄っている部分
        Task.Run(async () =>
        {
            await Task.Delay((int) (phonemeDelay * 1000d));
            double cacheTime = 0;
            foreach (var item in _phonemeParts)
            {
                var delayTime = item.time + cacheTime;
                Debug.Log("音素ループ： " + item.unit + " " + delayTime);
                switch (item.unit)
                {
                    case "sil": 
                    case "pau":
                        MorphValueReset();
                        pmValue.SilPau = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    case "N":
                        MorphValueReset();
                        pmValue.N = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    //後でこのあたりに両唇音の処理を追加する予定です
                    case "a":
                    case "A":
                        MorphValueReset();
                        pmValue.A = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    case "i":
                    case "I":
                        MorphValueReset();
                        pmValue.I = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    case "u":
                    case "U":
                        MorphValueReset();
                        pmValue.U = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    case "e":
                    case "E":
                        MorphValueReset();
                        pmValue.E = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    case "o":
                    case "O":
                        MorphValueReset();
                        pmValue.O = 100;
                        await Task.Delay((int) (delayTime * 1000d));
                        cacheTime = 0;
                        break;
                    default :
                        MorphValueReset();
                        cacheTime += item.time;
                        break;
                }
            }
            MorphValueReset();
        });
    }
}
