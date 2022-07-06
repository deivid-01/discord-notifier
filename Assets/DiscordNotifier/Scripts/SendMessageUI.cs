using System;
using System.Collections;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SendMessageUI : MonoBehaviour
    {
        public event Action<string> OnTrySendMsg;

        [SerializeField] private Button btnSendMsg;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private GameObject panelWrongKey;


        private Coroutine showAndHide;
        private void Awake()
        {
            panelWrongKey.SetActive(false);
            btnSendMsg.onClick.AddListener(TrySendMsg);
        }

        public void SetActiveElement(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void TrySendMsg()
        {
            OnTrySendMsg?.Invoke(inputField.text);
            btnSendMsg.interactable = false;
        }

        public void ShowError()
        {
            if (showAndHide != null)
            {
                StopCoroutine(showAndHide);
            }

            showAndHide= StartCoroutine(ShowHidePanelCoroutine(panelWrongKey,OnDisplayedError));

            void OnDisplayedError()
            {
                btnSendMsg.interactable = true;
            } 
        }

        public IEnumerator ShowHidePanelCoroutine(GameObject obj,Action OnComplete)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(1);
            obj.SetActive(false);
            OnComplete?.Invoke();
        }
    }
