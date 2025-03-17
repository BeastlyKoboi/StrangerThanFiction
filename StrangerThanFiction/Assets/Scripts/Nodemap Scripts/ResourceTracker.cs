using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    public GameEventAsync ResourceEvent;
    public TextMeshProUGUI resourceText;
    public string prefix;
    public string suffix;

    private UniTask UpdateResource(EventState eventState)
    {
        if (eventState is not EventStateInt eventStateInt) return UniTask.CompletedTask;

        resourceText.text = prefix + eventStateInt.value.ToString() + suffix;
        return UniTask.CompletedTask;
    }

    private void OnEnable() { ResourceEvent.AddListener(UpdateResource); }

    private void OnDisable() { ResourceEvent.RemoveListener(UpdateResource); }
}
