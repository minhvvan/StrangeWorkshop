using UnityEngine;
using UnityEngine.UI;

public class ResultUIContorller : BasePopupUI
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (IsOpen)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}