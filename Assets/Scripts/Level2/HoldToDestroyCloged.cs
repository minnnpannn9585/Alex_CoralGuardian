using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HoldToDestroyCloged : MonoBehaviour
{
    [Header("Hold Settings")]
    [SerializeField] private KeyCode holdKey = KeyCode.E;
    [SerializeField] private float holdSeconds = 5f;

    [Header("UI")]
    [SerializeField] private Slider progressSlider;

    private float holdTimer = 0f;
    private Cloged currentCloged;

    private void Awake()
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentCloged == null)
        {
            ResetProgressUI();
            return;
        }

        if (Input.GetKey(holdKey))
        {
            holdTimer += Time.deltaTime;

            float t = Mathf.Clamp01(holdTimer / holdSeconds);
            SetProgressUI(t);

            if (holdTimer >= holdSeconds)
            {
                Destroy(currentCloged.gameObject);
                currentCloged = null;
                holdTimer = 0f;
                ResetProgressUI();
            }
        }
        else
        {
            // 松开按键：停止并清空进度（如果你想“松开保留进度”，把这里改掉即可）
            holdTimer = 0f;
            SetProgressUI(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Cloged"))
            return;

        var cloged = other.GetComponent<Cloged>();
        if (cloged == null)
            return;

        currentCloged = cloged;
        holdTimer = 0f;
        SetProgressUI(0f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentCloged == null)
            return;

        if (other.gameObject == currentCloged.gameObject)
        {
            currentCloged = null;
            holdTimer = 0f;
            ResetProgressUI();
        }
    }

    private void SetProgressUI(float normalized01)
    {
        if (progressSlider == null)
            return;

        if (!progressSlider.gameObject.activeSelf)
            progressSlider.gameObject.SetActive(true);

        progressSlider.value = normalized01;
    }

    private void ResetProgressUI()
    {
        if (progressSlider == null)
            return;

        progressSlider.value = 0f;
        if (progressSlider.gameObject.activeSelf)
            progressSlider.gameObject.SetActive(false);
    }
}