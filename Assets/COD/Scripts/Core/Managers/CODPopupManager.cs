using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace COD.Core
{
    public class CODPopupManager
    {
        public List<CODPopupData> PopupsData = new();
        public Canvas popupsCanvas;

        private Dictionary<PopupTypes, CODPopupComponentBase> cachedPopups = new();

        public CODPopupManager()
        {
            CreateCanvas();
        }
        private void CreateCanvas()
        {
            CODManager.Instance.FactoryManager.CreateAsync("PopupCanvas", Vector3.zero, (Canvas canvas) =>
            {
                popupsCanvas = canvas;
                Object.DontDestroyOnLoad(popupsCanvas);
            });
        }
        public void AddPopupToQueue(CODPopupData popupData)
        {
            PopupsData.Add(popupData);
            TryShowNextPopup();
        }
        public void TryShowNextPopup()
        {
            if (PopupsData.Count <= 0)
            {
                return;
            }

            SortPopups();
            OpenPopup(PopupsData[0]);
        }
        public void SortPopups()
        {
            PopupsData = PopupsData.OrderBy(x => x.Priority).ToList();
        }
        public void OpenPopup(CODPopupData codPopupData)
        {
            codPopupData.OnPopupClose += OnClosePopup;
            PopupsData.Remove(codPopupData);

            if (cachedPopups.ContainsKey(codPopupData.PopupType))
            {
                var pop = cachedPopups[codPopupData.PopupType];
                pop.gameObject.SetActive(true);
                pop.Init(codPopupData);
            }
            else
            {
                CODManager.Instance.FactoryManager.CreateAsync(codPopupData.PopupType.ToString(),
                    Vector3.zero, (CODPopupComponentBase popupComponent) =>
                    {
                        cachedPopups.Add(codPopupData.PopupType, popupComponent);
                        popupComponent.transform.SetParent(popupsCanvas.transform, false);
                        popupComponent.Init(codPopupData);
                    });
            }
        }
        private void OnClosePopup(CODPopupComponentBase codPopupComponentBase)
        {
            codPopupComponentBase.gameObject.SetActive(false);
            TryShowNextPopup();
        }
    }
    public class CODPopupData
    {
        public int Priority;
        public PopupTypes PopupType;

        public Action OnPopupOpen;
        public Action<CODPopupComponentBase> OnPopupClose;

        public object GenericData;

        public static CODPopupData WelcomeMessage = new()
        {
            Priority = 0,
            PopupType = PopupTypes.WelcomeMessage,
            GenericData = "Hello loonitech :-)"
        };

        public static CODPopupData UpgradePopupData = new()
        {
            Priority = 0,
            PopupType = PopupTypes.UpgradePopupMenu
        };
    }

    public enum PopupTypes
    {
        WelcomeMessage,
        Store,
        UpgradePopupMenu
    }

}
