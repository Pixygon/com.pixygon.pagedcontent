using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Pixygon.PagedContent {
    public class PagedContentManager : MonoBehaviour {
        [SerializeField] private GameObject _pagedContentPrefab;
        [SerializeField] private Transform _contentParent;

        [SerializeField] private Vector2 _cellSize;
        [SerializeField] private Vector2 _spacing;
        [SerializeField] private int _itemsPerPage = 30;

        [SerializeField] private GameObject _prevButton;
        [SerializeField] private GameObject _nextButton;

        [SerializeField] private TextMeshProUGUI _pageNumber;

        [SerializeField] private InputAction _prevPageAction;
        [SerializeField] private InputAction _nextPageAction;

        private int CurrentPage { get; set; }
        private int _totalPages;
        private int _number;

        private List<GameObject> _pageContent;
        private object[] _itemDatas;
        private Action<object, int> _action;
        private bool _nIsItemCount;
        private float _buttonTimer;
        
        private void Update() {
            if (_buttonTimer >= 0f)
                _buttonTimer -= Time.deltaTime;
        }
        private void OnEnable() {
            _prevPageAction.Enable();
            _nextPageAction.Enable();
            _prevPageAction.performed += PrevPageClick;
            _nextPageAction.performed += NextPageClick;
        }

        private void OnDisable() {
            _prevPageAction.Disable();
            _nextPageAction.Disable();
            _prevPageAction.performed -= PrevPageClick;
            _nextPageAction.performed -= NextPageClick;
        }

        public void NextPage() {
            SetPage(CurrentPage + 1);
        }
        public void PreviousPage() {
            SetPage(CurrentPage - 1);
        }
        private bool _rotatingPages;
        public void PopulateCollections(object[] d, int n, Action<object, int> a, int page = 0, bool nIsItemCount = false, bool rotatingPages = false) {
            ClearPage();
            _action = a;
            _number = n;
            _itemDatas = d;
            _nIsItemCount = nIsItemCount;
            _rotatingPages = rotatingPages;
            if(d == null) return;
            if(d.Length == 0) return;
            SetPage(page);
            SetLayout();
        }
        private async void SetPage(int page) {
            ClearPage();
            CurrentPage = page;
            _totalPages = _itemDatas.Length / _itemsPerPage;
            _pageNumber.text = _totalPages is 1 or 0 ? string.Empty : $"{CurrentPage+1}/{_totalPages+1}";
            var x = _itemsPerPage * (CurrentPage + 1);
            if(_itemDatas.Length - 1 < x)
                x = _itemDatas.Length;
            for(var i = _itemsPerPage * CurrentPage; i < x; i++) {
                var g = Instantiate(_pagedContentPrefab, _contentParent);
                if(_nIsItemCount)
                    _number = i;
                g.GetComponent<PagedContentObject>().Initialize(_itemDatas[i], _number, _action);
                _pageContent.Add(g);
                await Task.Yield();
            }
            EventSystem.current.SetSelectedGameObject(_pageContent[0]);
            if(_prevButton != null && !_rotatingPages) _prevButton.SetActive(CurrentPage != 0);
            if(_nextButton != null && !_rotatingPages) _nextButton.SetActive(CurrentPage != _totalPages);
        }
        public void ClearPage() {
            if(_pageContent != null) {
                foreach(var g in _pageContent)
                    Destroy(g);
            }

            _pageNumber.text = string.Empty;
            _pageContent = new List<GameObject>();
            if(_prevButton != null)
                _prevButton.SetActive(false);
            if(_nextButton != null)
                _nextButton.SetActive(false);
        }
        private void SetLayout() {
            var g = _contentParent.GetComponent<GridLayoutGroup>();
            g.cellSize = _cellSize;
            g.spacing = _spacing;
        }
        private void PrevPageClick(InputAction.CallbackContext i) {
            if (i.phase != InputActionPhase.Performed) return;
            if(_prevButton != null)
                if (!_prevButton.activeInHierarchy)
                    return;
            if (_buttonTimer >= 0) return;
            _buttonTimer = .5f;
            PreviousPage();
        }
        private void NextPageClick(InputAction.CallbackContext i) {
            if (i.phase != InputActionPhase.Performed) return;
            if(_nextButton != null)
                if (!_nextButton.activeInHierarchy)
                    return;
            if (_buttonTimer >= 0) return;
            _buttonTimer = .5f;
            NextPage();
        }
    }
}