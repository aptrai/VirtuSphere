using UnityEngine;
using UnityEngine.UI;

public class CatDesc : MonoBehaviour
{
    public GameObject CatDescObj;
    public RawImage CatDescImg;

    public Texture2D[] pictures;

    public void DisplayPicture(int index)
    {
        CatDescObj.SetActive(true);

        if (index >= 0 && index < pictures.Length)
        {
            CatDescImg.texture = pictures[index];
        }
        else
        {
            Debug.LogError("Invalid picture index provided: " + index);
        }
    }
    public void ClosePicture()
    {
        CatDescObj.SetActive(false);
    }
}
