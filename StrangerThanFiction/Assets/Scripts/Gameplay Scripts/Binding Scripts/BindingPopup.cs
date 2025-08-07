using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BindingPopup : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    [HeaderAttribute("Binding")]
    [SerializeField] private TextMeshProUGUI bindingLabel;
    [SerializeField] private TextMeshProUGUI bindingWindowTitle;
    [SerializeField] private TextMeshProUGUI chapterLabel;
    [SerializeField] private TextMeshProUGUI noBoonsLabel;
    [SerializeField] private GameObject boonBox;

    [SerializeField] private GameObject boonPrefab;

    private void Start()
    {
        combatManager.OnRoundStart.AddListener(UpdateChapterLabel);
    }

    public UniTask UpdateChapterLabel(RoundStartState roundStartState)
    {
        chapterLabel.text = $"Chapter {roundStartState.currentRoundNumber} of {roundStartState.maxRounds}";
        return UniTask.CompletedTask;
    }

    public void UpdateBinding(BindingState bindingState)
    {
        bindingLabel.text = $"{bindingState.currTotalBindingDamage}/{bindingState.bindingPower}";
    }

    public void UpdateBindingWindow(BindingState bindingState)
    {
        bindingWindowTitle.text = bindingState.BattleNodeData.Title;

        if (bindingState.Boons.boons.Count > 0)
        {
            noBoonsLabel.gameObject.SetActive(false);
            boonBox.SetActive(true);
            foreach (Boon boon in bindingState.Boons.boons)
            {
                GameObject boonTile = Instantiate(boonPrefab, boonBox.transform);
                boonTile.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = boon.Name;
                boonTile.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = boon.Description;
            }
        }
        else
        {
            Debug.Log("No boons available for this binding.");
            noBoonsLabel.gameObject.SetActive(true);
            boonBox.SetActive(false);
        }


    }


}
