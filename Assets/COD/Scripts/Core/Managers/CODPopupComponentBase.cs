using System.Collections.Generic;

namespace COD.Core
{
    public class CODPopupComponentBase : CODMonoBehaviour
    {
        protected CODPopupData popupData;

        public virtual void Init(CODPopupData popupData)
        {
            this.popupData = popupData;
            OnOpenPopup();
        }

        protected virtual void OnOpenPopup()
        {
            popupData.OnPopupOpen?.Invoke();
        }

        public virtual void ClosePopup()
        {
            OnClosePopup();
        }

        protected virtual void OnClosePopup()
        {
            popupData.OnPopupClose?.Invoke(this);
        }
    }
}

