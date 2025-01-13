using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Match.View
{
    /// <summary>
    /// Oyun UI sistemini kontrol eden ana s�n�f
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Score UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Slider progressSlider;

        [Header("Button References")]
        [SerializeField] private Button resetButton;
        [SerializeField] private Button zoomSkillButton;
        [SerializeField] private Button hintSkillButton;

        [Header("Button Texts")]
        [SerializeField] private TMP_Text zoomButtonText;
        [SerializeField] private TMP_Text hintButtonText;

        [Header("Skill Settings")]
        [SerializeField] private float zoomCooldown = 8f;
        [SerializeField] private float hintCooldown = 30f;
        [SerializeField] private float scorePopupDuration = 0.1f;
        [SerializeField] private float fillAnimationDuration = 0.3f;

        [Header("Score Animation Settings")]
        [SerializeField] private Color scoreIncreaseColor = Color.green;
        [SerializeField] private float scoreAnimationDuration = 0.5f;
        [SerializeField] private float scorePunchScale = 1.2f;

        // Private variables
        private readonly string _scoreTextFormat = "Score: {0}";
        private ItemSpawner _itemSpawner;
        private int _score = 0;

        // Skill states
        private bool isZoomSkillReady = true;
        private bool isHintSkillReady = true;

        private void Start()
        {
            ValidateReferences();
            InitializeComponents();
            SetupButtonListeners();
            
            // Event'e abone ol
            GameEvents.OnItemsSpawned += OnItemsSpawned;
        }

        private void ValidateReferences()
        {
            // Score UI kontrolleri
            if (scoreText == null)
                Debug.LogError($"{gameObject.name}: Score Text reference is missing!");
            if (progressSlider == null)
                Debug.LogError($"{gameObject.name}: Progress Slider reference is missing!");

            // Buton kontrolleri
            if (resetButton == null)
                Debug.LogError($"{gameObject.name}: Reset Button reference is missing!");
            if (zoomSkillButton == null)
                Debug.LogError($"{gameObject.name}: Zoom Skill Button reference is missing!");
            if (hintSkillButton == null)
                Debug.LogError($"{gameObject.name}: Hint Skill Button reference is missing!");

            // Buton text'lerini otomatik bul
            if (zoomButtonText == null && zoomSkillButton != null)
            {
                zoomButtonText = zoomSkillButton.GetComponentInChildren<TMP_Text>();
                if (zoomButtonText == null)
                    Debug.LogError($"{gameObject.name}: Zoom Button Text component is missing!");
            }

            if (hintButtonText == null && hintSkillButton != null)
            {
                hintButtonText = hintSkillButton.GetComponentInChildren<TMP_Text>();
                if (hintButtonText == null)
                    Debug.LogError($"{gameObject.name}: Hint Button Text component is missing!");
            }
        }

        private void InitializeComponents()
        {
            _itemSpawner = FindObjectOfType<ItemSpawner>();
            if (_itemSpawner == null)
            {
                Debug.LogError("ItemSpawner bulunamad�!");
                return;
            }

            Initialize(_itemSpawner);
        }

        private void SetupButtonListeners()
        {
            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
                resetButton.onClick.AddListener(ResetGame);
            }

            if (zoomSkillButton != null)
            {
                zoomSkillButton.onClick.RemoveAllListeners();
                zoomSkillButton.onClick.AddListener(OnZoomSkillButtonClick);
            }

            if (hintSkillButton != null)
            {
                hintSkillButton.onClick.RemoveAllListeners();
                hintSkillButton.onClick.AddListener(OnHintSkillButtonClick);
            }
        }

        public void Initialize(ItemSpawner itemSpawner)
        {
            if (itemSpawner == null)
            {
                Debug.LogError("ItemSpawner null olamaz!");
                return;
            }

            _itemSpawner = itemSpawner;
            _score = 0;
            SetupUI();

            // Event'lere abone ol
            GameEvents.OnItemMatched += OnItemMatched;
            GameEvents.OnItemsSpawned += SetupUI;
        }

        private void OnDestroy()
        {
            // Event aboneliklerini temizle
            GameEvents.OnItemMatched -= OnItemMatched;
            GameEvents.OnItemsSpawned -= OnItemsSpawned;
            GameEvents.OnItemsSpawned -= SetupUI;

            // Button listener'lar� temizle
            if (resetButton != null)
                resetButton.onClick.RemoveListener(ResetGame);
            if (zoomSkillButton != null)
                zoomSkillButton.onClick.RemoveListener(OnZoomSkillButtonClick);
            if (hintSkillButton != null)
                hintSkillButton.onClick.RemoveListener(OnHintSkillButtonClick);
        }

        private void SetupUI()
        {
            // Score text'ini ayarla
            if (scoreText != null)
                scoreText.text = string.Format(_scoreTextFormat, _score);

            // Slider'ı sıfırla
            if (progressSlider != null)
            {
                progressSlider.value = 0;
                progressSlider.maxValue = 1;
            }

            // Zoom butonunu ayarla
            if (zoomSkillButton != null)
            {
                zoomSkillButton.interactable = true;
                if (zoomButtonText != null)
                    zoomButtonText.text = "Zoom";
            }

            // Hint butonunu ayarla
            if (hintSkillButton != null)
            {
                hintSkillButton.interactable = true;
                if (hintButtonText != null)
                    hintButtonText.text = "Hint";
            }
        }

        private void OnItemMatched(ItemData data)
        {
            int oldScore = _score;
            _score += data.itemScore;
            UpdateUI();
            AnimateScoreIncrease(oldScore, _score);
        }

        private void AnimateScoreIncrease(int fromScore, int toScore)
        {
            if (scoreText == null) return;

            // Orijinal değerleri sakla
            Color originalColor = scoreText.color;
            Vector3 originalScale = scoreText.transform.localScale;

            // Renk animasyonu
            scoreText.DOColor(scoreIncreaseColor, scoreAnimationDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    scoreText.DOColor(originalColor, scoreAnimationDuration * 0.5f)
                        .SetEase(Ease.InQuad);
                });

            // Scale animasyonu
            scoreText.transform.DOPunchScale(Vector3.one * scorePunchScale, scoreAnimationDuration, 1, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    scoreText.transform.localScale = originalScale;
                });

            // Sayı artış animasyonu
            DOTween.To(() => fromScore, x => {
                if (scoreText != null)
                {
                    scoreText.text = string.Format(_scoreTextFormat, x);
                }
            }, toScore, scoreAnimationDuration)
                .SetEase(Ease.OutQuad);
        }

        private IEnumerator ScorePopupAnimation()
        {
            yield break;
        }

        private void UpdateUI()
        {
            // Score text'ini güncelle
            if (scoreText != null)
                scoreText.text = string.Format(_scoreTextFormat, _score);

            // Progress slider'ı güncelle
            if (_itemSpawner != null && progressSlider != null)
            {
                float fillAmount = 1 - (_itemSpawner.CurrentItemCount / (float)_itemSpawner.SpawnedItemCount);
                StartCoroutine(SmoothSliderAnimation(fillAmount));
            }
        }

        private IEnumerator SmoothSliderAnimation(float targetValue)
        {
            if (progressSlider == null) yield break;

            float startValue = progressSlider.value;
            float elapsed = 0;

            while (elapsed < fillAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fillAnimationDuration;
                progressSlider.value = Mathf.Lerp(startValue, targetValue, t);
                yield return null;
            }

            progressSlider.value = targetValue;
        }

        #region Skill Button Handlers

        public void OnZoomSkillButtonClick()
        {
            if (!isZoomSkillReady || zoomSkillButton == null) return;

            zoomSkillButton.interactable = false;
            GameEvents.InvokeZoomSkillUsed();

            StartCoroutine(ZoomSkillCooldown());
        }

        private IEnumerator ZoomSkillCooldown()
        {
            if (zoomSkillButton == null || zoomButtonText == null) yield break;

            isZoomSkillReady = false;

            float remainingTime = 3f;
            while (remainingTime > 0)
            {
                zoomButtonText.text = "ACTIVE";
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            remainingTime = zoomCooldown;
            while (remainingTime > 0)
            {
                zoomButtonText.text = "COOLDOWN";
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            isZoomSkillReady = true;
            zoomSkillButton.interactable = true;
            zoomButtonText.text = "Zoom";
        }

        public void OnHintSkillButtonClick()
        {
            if (!isHintSkillReady || hintSkillButton == null) return;

            hintSkillButton.interactable = false;
            GameEvents.InvokeHintSkillUsed();

            StartCoroutine(HintSkillCooldown());
        }

        private IEnumerator HintSkillCooldown()
        {
            if (hintSkillButton == null || hintButtonText == null) yield break;

            isHintSkillReady = false;

            float remainingTime = 4f;
            while (remainingTime > 0)
            {
                hintButtonText.text = "ACTIVE";
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            remainingTime = hintCooldown;
            while (remainingTime > 0)
            {
                hintButtonText.text = "COOLDOWN";
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            isHintSkillReady = true;
            hintSkillButton.interactable = true;
            hintButtonText.text = "Hint";
        }

        #endregion

        #region Game Reset

        public void ResetGame()
        {
            StopAllCoroutines();

            // Score'u sıfırla
            _score = 0;
            if (scoreText != null)
                scoreText.text = string.Format(_scoreTextFormat, _score);

            // Slider'ı sıfırla
            if (progressSlider != null)
                progressSlider.value = 0;

            // Yetenekleri resetle
            ResetSkills();

            // ItemSpawner'ı resetle
            if (_itemSpawner != null)
            {
                ResetItems();
            }
            else
            {
                Debug.LogError("ItemSpawner is null!");
            }
        }

        private void ResetSkills()
        {
            // Zoom yeteneğini resetle
            isZoomSkillReady = true;
            if (zoomSkillButton != null)
            {
                zoomSkillButton.interactable = true;
                if (zoomButtonText != null)
                    zoomButtonText.text = "Zoom";
            }

            // Hint yeteneğini resetle
            isHintSkillReady = true;
            if (hintSkillButton != null)
            {
                hintSkillButton.interactable = true;
                if (hintButtonText != null)
                    hintButtonText.text = "Hint";
            }
        }

        private void ResetItems()
        {
            if (_itemSpawner == null || _itemSpawner.spawnedObjects == null) return;

            // Mevcut itemları bir listeye kopyala
            List<Transform> itemsToDestroy = new List<Transform>(_itemSpawner.spawnedObjects);
            
            // Spawner listesini temizle
            _itemSpawner.spawnedObjects.Clear();

            // Her bir itemi hemen yok et
            foreach (var item in itemsToDestroy)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            // Skor ve slider'ı sıfırla
            _score = 0;
             if (scoreText != null)
                scoreText.text = string.Format(_scoreTextFormat, _score);
            if (progressSlider != null)
            {
                progressSlider.value = 0;
            }
            UpdateUI();

            // Yeni itemları spawn et
            StartCoroutine(SpawnWithDelay());
        }

        private IEnumerator SpawnWithDelay()
        {
            yield return new WaitForSeconds(0.2f); // Yok etme işleminin tamamlanması için kısa bir süre bekle
            if (_itemSpawner != null)
            {
                _itemSpawner.SpawnObjects();
            }
        }

        #endregion

        private void OnItemsSpawned()
        {
            // Yeni objeler spawn edildiğinde skor ve slider'ı sıfırla
            _score = 0;
             if (scoreText != null)
                scoreText.text = string.Format(_scoreTextFormat, _score);
            UpdateUI();
            if (progressSlider != null)
            {
                progressSlider.value = 0;
            }
        }
    }
}
