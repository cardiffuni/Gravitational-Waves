using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskListArrow : UIUtils
{
    public float RotateOnZBy = 90f;
    public float RotationDuration = 0.2f;

    private float _nextRotation;

    private new void Awake()
    {
        base.Awake();

        _nextRotation = RotateOnZBy;
    }

    /// <summary>
    /// Toggles the rotation of the Task List Arrow.
    /// </summary>
    public void ToggleRotation()
    {
        /*if (!gameObject.LeanIsTweening())
        {
            RotateZBy(_nextRotation, RotationDuration);
            _nextRotation *= -1;
        }*/
    }
}
