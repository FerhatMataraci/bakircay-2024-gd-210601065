using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using Match;

namespace Match.View
{
    public class ZoomSkillButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private TMP_Text cooldownText;

        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color cooldownColor = Color.gray;

        private float cooldownDuration = 8f;
        private float activeDuration = 3f;
        private bool isReady = true;

        private void Start()
        {
            ValidateReferences();
            SetupButton();
        }

        private void ValidateReferences()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
                if (button == null)
                {
                    Debug.LogError($"{gameObject.name}: Button component is missing!");
                    return;
                }
            }
            if (buttonText == null)
            {
                buttonText = GetComponentInChildren<TMP_Text>();
                if (buttonText == null)
                {
                    Debug.LogError($"{gameObject.name}: TMP_Text component is missing!");
                }
            }
            if (cooldownText == null)
            {
                cooldownText = transform.Find("CooldownText")?.GetComponent<TMP_Text>();
                if (cooldownText == null)
                {
                    Debug.LogError($"{gameObject.name}: Cooldown Text component is missing!");
                }
            }
        }

        private void SetupButton()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
                ResetButton();
            }
        }

        private void UpdateCooldownText(float remainingTime)
        {
            if (cooldownText == null) return;
            
            cooldownText.text = remainingTime > 0 ? remainingTime.ToString("F1") : "";
            
            if (remainingTime <= 0)
            {
                cooldownText.DOColor(activeColor, 0.5f).SetEase(Ease.InOutSine);
            }
        }

        private void OnButtonClick()
        {
            if (!isReady || button == null) return;
            button.interactable = false;
            GameEvents.InvokeZoomSkillUsed();
            StartCoroutine(HandleZoomSkill());
        }

        private IEnumerator HandleZoomSkill()
        {
            if (button == null || buttonText == null) yield break;

            isReady = false;
            float remainingTime = activeDuration;
            
            while (remainingTime > 0)
            {
                buttonText.text = "ACTIVE";
                UpdateCooldownText(remainingTime);
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            remainingTime = cooldownDuration;
            cooldownText.color = cooldownColor;
            
            while (remainingTime > 0)
            {
                buttonText.text = "COOLDOWN";
                UpdateCooldownText(remainingTime);
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            ResetButton();
        }

        private void ResetButton()
        {
            isReady = true;
            if (button != null)
            {
                button.interactable = true;
            }
            if (buttonText != null)
            {
                buttonText.text = "Zoom";
            }
            UpdateCooldownText(0);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (button == null)
                button = GetComponent<Button>();
            if (buttonText == null && button != null)
                buttonText = GetComponentInChildren<TMP_Text>();
            if (cooldownText == null && button != null)
                cooldownText = transform.Find("CooldownText")?.GetComponent<TMP_Text>();
        }
#endif
    }
}