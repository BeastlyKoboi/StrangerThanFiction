using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Stolen From ChristinaCreatesGames
/// https://www.youtube.com/watch?v=LNwYgN47qqk
/// https://github.com/Maraakis/ChristinaCreatesGames/blob/main/Detect%20hovering%20on%20tagged%20text%20elements/LinkHandlerForTMPTextHover.cs
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class LinkHandlerForTMPTextHover : MonoBehaviour
{
    private TMP_Text _tmpTextBox;
    private Canvas _canvasToCheck;
    private Camera _cameraToUse;
    private RectTransform _textBoxRectTransform;

    private int _currentlyActiveLinkedElement;

    public delegate void HoverOnLinkEvent(string keyword, Vector3 mousePos);
    public static event HoverOnLinkEvent OnHoverOnLinkEvent;

    public delegate void CloseTooltipEvent();
    public static event CloseTooltipEvent OnCloseTooltipEvent;

    private void Start()
    {
        _tmpTextBox = GetComponent<TMP_Text>();
        _canvasToCheck = GetComponentInParent<Canvas>();
        _textBoxRectTransform = GetComponent<RectTransform>();

        if (_canvasToCheck.renderMode == RenderMode.ScreenSpaceOverlay)
            _cameraToUse = null;
        else
            _cameraToUse = _canvasToCheck.worldCamera;
    }

    private void Update()
    {
        CheckForLinkAtMousePosition();
    }

    private void CheckForLinkAtMousePosition()
    {
        // For new input system
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // For old input system use this, rest stays the same:
        // Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        bool isIntersectingRectTransform = TMP_TextUtilities.IsIntersectingRectTransform(_textBoxRectTransform, mousePosition, null);

        if (!isIntersectingRectTransform)
            return;

        int intersectingLink = TMP_TextUtilities.FindIntersectingLink(_tmpTextBox, mousePosition, null);

        if (_currentlyActiveLinkedElement != intersectingLink)
            OnCloseTooltipEvent?.Invoke();

        if (intersectingLink == -1)
            return;


        Vector3 bottomLeft = Vector3.zero;
        Vector3 topRight = Vector3.zero;

        float maxAscender = -Mathf.Infinity;
        float minDescender = Mathf.Infinity;

        TMP_TextInfo textInfo = _tmpTextBox.textInfo;
        TMP_LinkInfo linkInfo = _tmpTextBox.textInfo.linkInfo[intersectingLink];
        TMP_CharacterInfo currentCharInfo = textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex];
        TMP_CharacterInfo lastCharInfo = textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1];

        maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender, lastCharInfo.ascender);
        minDescender = Mathf.Min(minDescender, currentCharInfo.descender, lastCharInfo.descender);

        bottomLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);

        bottomLeft = _textBoxRectTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
        topRight = _textBoxRectTransform.TransformPoint(new Vector3(lastCharInfo.topRight.x, maxAscender, 0));

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        Vector2 centerPosition = bottomLeft;
        centerPosition.x += width / 2;
        centerPosition.y += height / 2;

        OnHoverOnLinkEvent?.Invoke(linkInfo.GetLinkID(), centerPosition);
        _currentlyActiveLinkedElement = intersectingLink;
    }
}