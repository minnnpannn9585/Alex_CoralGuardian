using UnityEngine;
using UnityEngine.UI;

public class Cloged : MonoBehaviour
{
    [Tooltip("Assign the unique slider for this clogged object here")]
    public Slider progressSlider;

    [Tooltip("Object to activate when this clogged object is destroyed")]
    public GameObject objectToActivate;

    // Start is called before the first frame update
    void Start()
    {
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0;
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }
}
