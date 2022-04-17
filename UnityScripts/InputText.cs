using UnityEngine;
using TMPro;

/// <summary>
/// UIからテキストをもらって、読み上げの実行をします
/// </summary>
public class InputText : MonoBehaviour
{
    [SerializeField]
    TMP_InputField tMP_InputField;
    [SerializeField]
    CevioTestScr cevioTestScr;

    public void GetInputText()
    {
        string inputtex = tMP_InputField.text;
        Debug.Log(inputtex);
        cevioTestScr.RikkaTalk(inputtex);
        tMP_InputField.text = "";
    }
}
