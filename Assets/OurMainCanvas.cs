using UnityEngine;

public class OurMainCanvas : MonoBehaviour
{


    public void ToggleCanvas()
    {
        if (gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
