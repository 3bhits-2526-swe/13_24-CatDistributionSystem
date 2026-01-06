using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltipRect;

    [Header("Settings")]
    public Vector2 offset = new Vector2(10, 10);
    public float followSpeed = 10f;

    private Canvas canvas;
    private bool isVisible = false;

    private void Awake()
    {
        Instance = this;
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    private void Update()
    {
        if (isVisible)
            FollowMouse();
    }

    public void Show(string text)
    {
        if (tooltipText != null)
            tooltipText.text = text;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);

        isVisible = true;
        FollowMouse();
    }

    public void Hide()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        isVisible = false;
    }

    private void FollowMouse()
    {
        if (tooltipRect == null || canvas == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint
        );

        tooltipRect.localPosition = localPoint + offset;
    }
}