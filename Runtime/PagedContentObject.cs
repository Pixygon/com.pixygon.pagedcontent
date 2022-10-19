using Pixygon.Tooltip;
using System;
using Pixygon.ButtonEffects;
using UnityEngine;

namespace Pixygon.PagedContent {
    public class PagedContentObject : ButtonEffect {
        [SerializeField] protected GameObject _loadObject;
        protected Action<object, int> _action;
        private object _data;
        protected int _number;
        
        public virtual async void Initialize(object d, int num, Action<object, int> a) {
            if(d == null) {
                Debug.Log("Something went wrong while spawning this!!");
                return;
            }
            _number = num;
            _action = a;
            _data = d;
            if(_icon != null)
                _startSize = _icon.rectTransform.sizeDelta;
        }

        public virtual void Select() {
            _action?.Invoke(_data, _number);
        }

        protected void SetTooltip(string header = "", string subheader = "", string content = "") {
            if (gameObject == null) return;
            var tt = gameObject.AddComponent<TooltipTrigger>();
            tt.SetTooltip(header, subheader, content);
        }
        /*
        protected async void LoadIcon(AssetReference r) {
            _loadObject.SetActive(true);
            _icon.color = Color.clear;
            _icon.sprite = await AddressableLoader.LoadAsset<Sprite>(r);
            _icon.color = Color.white;
            _loadObject.SetActive(false);
        }
        */
    }
}