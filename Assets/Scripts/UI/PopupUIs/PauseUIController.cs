using UnityEngine;

public class PauseUIController : BasePopupUI
{
    // [Header("UI")]
    // [SerializeField] RectTransform transform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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