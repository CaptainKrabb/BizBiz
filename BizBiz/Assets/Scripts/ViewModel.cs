using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewModel : MonoBehaviour 
{
    public Text buttonText;
    public GameObject cube;
    public Slider slider;

    public void Slider_Changed(float newValue)
    {
        Vector3 pos = cube.transform.position;
        pos.y = newValue;
        cube.transform.position = pos;
    }

    public void Button_Click()
    {
        Debug.Log("Hello, World!");
    }

    public void Button_String(string msg)
    {
        buttonText.text = msg;
    }

    public void Toggle_Changed(bool newValue)
    {
        cube.SetActive(newValue);
        slider.interactable = newValue;
    }

    public void Text_Changed(string newText)
    {
        float temp = float.Parse(newText);
        cube.transform.localScale = new Vector3(temp, temp, temp);
    }

}
