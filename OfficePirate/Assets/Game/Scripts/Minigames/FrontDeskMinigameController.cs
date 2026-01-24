using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FrontDeskMinigameController : MiniGameController
{
    [Header("Task Buttons")]
    [SerializeField] private Button[] taskButtons;

    [Header("Confirm Dialog")]
    [SerializeField] private GameObject confirmDialogPanel;
    [SerializeField] private TMP_Text confirmDialogText;

    private Button _pendingButton;
    private HashSet<Button> _remainingButtons = new HashSet<Button>();

    private void OnEnable()
    {
        ResetState();
    }

    private void ResetState()
    {
        _pendingButton = null;
        _remainingButtons.Clear();

        confirmDialogPanel.SetActive(false);

        foreach (var btn in taskButtons)
        {
            if (btn != null)
                _remainingButtons.Add(btn);
        }
    }


    public void PressTaskButton(GameObject buttonObject)
    {
        var btn = buttonObject.GetComponent<Button>();
        if (btn == null)
            return;

        if (!_remainingButtons.Contains(btn))
            return;

        _pendingButton = btn;

        if (confirmDialogText != null)
            confirmDialogText.text = "Confirm sabotage?";

        confirmDialogPanel.SetActive(true);
    }

    public void PressConfirmYes()
    {
        if (_pendingButton == null)
        {
            confirmDialogPanel.SetActive(false);
            return;
        }

        _remainingButtons.Remove(_pendingButton);
        Destroy(_pendingButton.gameObject);

        _pendingButton = null;
        confirmDialogPanel.SetActive(false);

        if (_remainingButtons.Count == 0)
        {
            CompleteSuccess();
        }
    }

    public void PressConfirmNo()
    {
        _pendingButton = null;
        confirmDialogPanel.SetActive(false);
    }
}