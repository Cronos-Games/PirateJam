using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextRandomizer : MonoBehaviour
{
    [SerializeField] private List<String> texts;
    private TMP_Text _text;

    private void OnEnable()
    {
        _text = GetComponent<TMP_Text>();
        _text.text = texts[Random.Range(0, texts.Count)];
    }
}
