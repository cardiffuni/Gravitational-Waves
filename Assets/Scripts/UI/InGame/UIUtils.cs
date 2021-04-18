using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{

    private bool _visible = true;

    protected void Awake()
    {
    }

    /// <summary>
    /// Toggles whether a UI component is visible or not.
    /// </summary>
    public void ToggleVisibility()
    {
        _visible = !_visible;

        gameObject.SetActive(_visible);
    }

    public void RotateZBy(float degrees, float duration)
    {
        // GameObject.Rotate(gameObject, new Vector3(0, 0, gameObject.transform.rotation.eulerAngles.z + degrees), duration);
    }
}
