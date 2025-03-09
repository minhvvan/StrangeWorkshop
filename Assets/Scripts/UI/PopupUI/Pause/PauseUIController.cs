using DG.Tweening;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : BasePopupUI
{
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _settingButton;
    [SerializeField] Button _restartButton;
    [SerializeField] Button _gotoChapterButton;
    [SerializeField] Button _mainMenuButton;


    [Header("Title Animation")]
    [SerializeField] RectTransform _titleRect;
    [SerializeField] float _titleMoveDuration = 0.3f;
    [SerializeField] Vector2 _titleStartPosition = new Vector2(-750, 0);
    [SerializeField] Vector2 _titleOriginPosition = new Vector2(-42, 0);
    [SerializeField] Ease _titleEase = Ease.InQuad;

    [Header("Menu Animation")]
    [SerializeField] RectTransform _menuRect;
    [SerializeField] float _menuMoveDuration = 0.3f;
    [SerializeField] Vector2 _menuStartPosition = new Vector2(-400, -190);
    [SerializeField] Vector2 _menuOriginPosition = new Vector2(-40, -190);
    [SerializeField] Ease _menuEase = Ease.InQuad;

    [Header("SubMenu Animation")]
    [SerializeField] RectTransform _subMenuRect;
    [SerializeField] float _subMenuMoveDuration = 0.3f;
    [SerializeField] Vector2 _subMenuStartPosition = new Vector2(-400, 0);
    [SerializeField] Vector2 _subMenuOriginPosition = new Vector2(0, 0);
    [SerializeField] Ease _subMenuEase = Ease.InQuad;


    void Awake()
    {
        _resumeButton.onClick.AddListener(OnClickResume);
        _settingButton.onClick.AddListener(OnClickSetting);
        _restartButton.onClick.AddListener(OnClickRestart);
        _gotoChapterButton.onClick.AddListener(OnClickGotoChapter);
        _mainMenuButton.onClick.AddListener(OnClickMainMenu);

        SetOnShowEvent(() => {
            animateShowTitle();
            animateShowMenu();
            animateShowSubMenu();
        });

        SetOnCloseEvent(() => {
            animateHideTitle();
            animateHideMenu();
            animateHideSubMenu();
        });

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
    
    public void animateShowTitle(){
        if(_titleRect == null) return;

        _titleRect.anchoredPosition = _titleStartPosition;
        _titleRect.DOAnchorPos(_titleOriginPosition, _titleMoveDuration).SetEase(_titleEase).SetUpdate(true);
    }

    public void animateHideTitle(){
        if(_titleRect == null) return;

        _titleRect.DOAnchorPos(_titleStartPosition, _titleMoveDuration).SetEase(_titleEase).SetUpdate(true);
    }

    public void animateShowMenu(){
        if(_menuRect == null) return;

        _menuRect.anchoredPosition = _menuStartPosition;
        _menuRect.DOAnchorPos(_menuOriginPosition, _menuMoveDuration).SetEase(_menuEase).SetUpdate(true);
    }

    public void animateHideMenu(){
        if(_menuRect == null) return;

        _menuRect.DOAnchorPos(_menuStartPosition, _menuMoveDuration).SetEase(_menuEase).SetUpdate(true);
    }

    public void animateShowSubMenu(){
        if(_subMenuRect == null) return;

        _subMenuRect.anchoredPosition = _subMenuStartPosition;
        _subMenuRect.DOAnchorPos(_subMenuOriginPosition, _subMenuMoveDuration).SetEase(_subMenuEase).SetUpdate(true);
    }

    public void animateHideSubMenu(){
        if(_subMenuRect == null) return;

        _subMenuRect.DOAnchorPos(_subMenuStartPosition, _subMenuMoveDuration).SetEase(_subMenuEase).SetUpdate(true);
    }
}