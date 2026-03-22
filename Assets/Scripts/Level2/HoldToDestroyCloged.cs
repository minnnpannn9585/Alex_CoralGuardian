using UnityEngine;
using UnityEngine.UI;

public class HoldToDestroyCloged : MonoBehaviour
{
    [Header("Hold Settings")]
    [SerializeField] private KeyCode holdKey = KeyCode.E;
    [SerializeField] private float holdSeconds = 5f;

    private float holdTimer;
    private Cloged currentCloged;

    private void Update()
    {
        if (currentCloged == null)
        {
            return;
        }

        if (Input.GetKey(holdKey))
        {
            holdTimer += Time.deltaTime;

            float t = Mathf.Clamp01(holdTimer / holdSeconds);
            SetProgressUI(t);

            if (holdTimer >= holdSeconds)
            {
                // Cache slider reference before destroying object
                Slider slider = currentCloged.progressSlider;
                GameObject objectToActivate = currentCloged.objectToActivate;

                // Activate the object if assigned
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                }

                Destroy(currentCloged.gameObject);

                // Hide slider explicitly if it still exists
                if (slider != null)
                {
                    slider.value = 0f;
                    slider.gameObject.SetActive(false);
                }

                currentCloged = null;
                holdTimer = 0f;
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
            ResetProgressUI();
            currentCloged = null;
            holdTimer = 0f;
        }
    }

    private void SetProgressUI(float normalized01)
    {
        if (currentCloged == null || currentCloged.progressSlider == null)
            return;

        Slider slider = currentCloged.progressSlider;
        if (!slider.gameObject.activeSelf)
            slider.gameObject.SetActive(true);

        slider.value = normalized01;
    }

    private void ResetProgressUI()
    {
        if (currentCloged == null || currentCloged.progressSlider == null)
            return;

        Slider slider = currentCloged.progressSlider;
        slider.value = 0f;
        if (slider.gameObject.activeSelf)
            slider.gameObject.SetActive(false);
    }
}