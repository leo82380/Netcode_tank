using DG.Tweening;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelUI : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private LobbyUI _lobbyUIPrefab;
    [SerializeField] private float _spacing = 30f;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _refreshBtn;

    private List<LobbyUI> _lobbyUIList;
    private RectTransform _rectTrm;
    private CanvasGroup _canvasGroup;

    private bool _isRefresh = false;

    private void Awake()
    {
        _lobbyUIList = new List<LobbyUI>();
        _rectTrm = GetComponent<RectTransform>();
        _closeBtn.onClick.AddListener(CloseWindow);
        _refreshBtn.onClick.AddListener(RefreshList);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        //float screenHeight = Screen.height;
        _rectTrm.anchoredPosition = new Vector2(0, 2000);
    }

    public void OpenWindow()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_rectTrm.DOAnchorPosY(0, 0.8f));
        seq.Join(_canvasGroup.DOFade(1f, 0.8f));
        seq.AppendCallback(() =>
        {
            _canvasGroup.interactable = true;
            RefreshList();
        });
    }

    public void CloseWindow()
    {
        float screenHeight = Screen.height;
        _canvasGroup.interactable = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(_rectTrm.DOAnchorPosY(screenHeight, 0.8f));
        seq.Join(_canvasGroup.DOFade(0f, 0.8f));
    }

    public async void RefreshList()
    {
        if (_isRefresh) return;
        _isRefresh = true;
        LoaderUI.Instance.Show(true);
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();

            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.AvailableSlots, 
                    "0",
                    QueryFilter.OpOptions.GT),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    value:"0",
                    op:QueryFilter.OpOptions.EQ)  //락이 0인 애들  
            };

            QueryResponse lobbies = 
                        await Lobbies.Instance.QueryLobbiesAsync(options);

            //기존 로비를 다 지우고
            ClearLobbies();
            // 새로운 로비로 생성해준다.
            foreach(Lobby lobby in lobbies.Results)
            {
                CreateLobbyUI(lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
        _isRefresh = false;
        LoaderUI.Instance.Show(false);
    }

    private void ClearLobbies()
    {
        foreach(LobbyUI ui in _lobbyUIList)
        {
            Destroy(ui.gameObject);
        }
        _lobbyUIList.Clear();
    }

    private void CreateLobbyUI(Lobby lobby)
    {
        LobbyUI ui = Instantiate(_lobbyUIPrefab, _scrollRect.content);

        //여기에 로비 정보 셋팅부분이 들어갈 예정
        ui.SetRoomTemplate(lobby);

        _lobbyUIList.Add(ui);

        float offset = _spacing;

        for(int i = 0; i < _lobbyUIList.Count; ++i)
        {
            _lobbyUIList[i].Rect.anchoredPosition = new Vector2(0, -offset);
            offset += _lobbyUIList[i].Rect.sizeDelta.y + _spacing;
        }

        Vector2 contentSize = _scrollRect.content.sizeDelta;
        contentSize.y = offset;
        _scrollRect.content.sizeDelta = contentSize;
    }
}