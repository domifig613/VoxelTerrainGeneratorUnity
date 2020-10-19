using System.Collections;
using UnityEngine;

public class DisableIfNotVisible : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisableIfNotVisibleCoroutine());
    }

    private IEnumerator DisableIfNotVisibleCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (!GetComponent<Renderer>().isVisible)
        {
            gameObject.SetActive(false);
        }
    }
}
