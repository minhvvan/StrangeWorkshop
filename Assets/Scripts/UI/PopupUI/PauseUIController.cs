using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : BasePopupUI
{
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _settingButton;
    [SerializeField] Button _restartButton;
    [SerializeField] Button _gotoChapterButton;
    [SerializeField] Button _mainMenuButton;

    void Awake()
    {
        _resumeButton.onClick.AddListener(OnClickResume);
        _settingButton.onClick.AddListener(OnClickSetting);
        _restartButton.onClick.AddListener(OnClickRestart);
        _gotoChapterButton.onClick.AddListener(OnClickGotoChapter);
        _mainMenuButton.onClick.AddListener(OnClickMainMenu);
    }

    void OnClickResume()
    {
        GameManager.Instance.ResumeGame();
    }

    void OnClickSetting()
    {
        //세팅 패널을 열어서 편집 할 수 있도록 기능을 만드는게 좋아보임
        //내용이 좀 있어서 나중에 추가하면 될듯
    }

    void OnClickRestart()
    {
        ConfirmPopupUIController confirmPopupUIController = UIManager.Instance.GetUI<ConfirmPopupUIController>(UIType.ConfirmPopupUI);
        confirmPopupUIController.ShowUI("게임 재시작", "게임을 재시작하면 현재 진행중인 게임이 저장되지 않습니다. 계속하시겠습니까?", () =>
        {
            GameManager.Instance.RestartGame();
        }, null, ConfirmPopupUIController.ConfirmPopupType.YELLOW);
    }

    void OnClickGotoChapter()
    {
        ConfirmPopupUIController confirmPopupUIController = UIManager.Instance.GetUI<ConfirmPopupUIController>(UIType.ConfirmPopupUI);
        confirmPopupUIController.ShowUI("챕터로 돌아가기", "챕터로 돌아가면 현재 진행중인 게임이 저장되지 않습니다. 계속하시겠습니까?", () =>
        {
            GameManager.Instance.StartGame();    
        }, null, ConfirmPopupUIController.ConfirmPopupType.YELLOW);
    }

    void OnClickMainMenu()
    {
        ConfirmPopupUIController confirmPopupUIController = UIManager.Instance.GetUI<ConfirmPopupUIController>(UIType.ConfirmPopupUI);
        confirmPopupUIController.ShowUI("메인 메뉴로 돌아가기", "메인 메뉴로 돌아가면 현재 진행중인 게임이 저장되지 않습니다. 계속하시겠습니까?", () =>
        {
            GameManager.Instance.LoadMainMenu();
        }, null, ConfirmPopupUIController.ConfirmPopupType.RED);
    }
}