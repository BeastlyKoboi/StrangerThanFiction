using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoonShop : MonoBehaviour
{
    [SerializeField] private BoonDictionary boonDictionary;

    [SerializeField] private List<BoonInfo> collectibleBoons;

    [SerializeField] private List<BoonInfo> commonBoons;
    [SerializeField] private List<BoonInfo> uncommonBoons;
    [SerializeField] private List<BoonInfo> rareBoons;
    [SerializeField] private List<BoonInfo> epicBoons;

    [SerializeField] private int baseCommonBoonPrice;
    [SerializeField] private int baseUncommonBoonPrice;
    [SerializeField] private int baseRareBoonPrice;
    [SerializeField] private int baseEpicBoonPrice;

    private void Awake()
    {
        collectibleBoons = new List<BoonInfo>();
        for (int i = 0; i < boonDictionary.GetEntries().Count; i++)
        {
            collectibleBoons.Add(boonDictionary.GetEntries()[i].Value);
        }

        for (int i = 0; i < boonDictionary.GetEntries().Count; i++)
        {
            BoonInfo boonInfo = boonDictionary.GetEntries()[i].Value;
            
            switch (boonInfo.BoonRarity)
            {
                case Rarity.Common:
                    commonBoons.Add(boonInfo);
                    break;
                case Rarity.Uncommon:
                    uncommonBoons.Add(boonInfo);
                    break;
                case Rarity.Rare:
                    rareBoons.Add(boonInfo);
                    break;
                case Rarity.Epic:
                    epicBoons.Add(boonInfo);
                    break;
            }

        }
    }

    public BoonInfo GetNextPurchaseableBoon()
    {
        float random = Random.Range(0f, 1f);

        // Temporary until we have more boons
        return boonDictionary.GetEntries().ElementAt(Random.Range(0, boonDictionary.GetEntries().Count)).Value;

        if (random < 0.5f)
        {
            return commonBoons[Random.Range(0, commonBoons.Count)];
        }
        else if (random < 0.8f)
        {
            return uncommonBoons[Random.Range(0, uncommonBoons.Count)];
        }
        else if (random < 0.95f)
        {
            return rareBoons[Random.Range(0, rareBoons.Count)];
        }
        else
        {
            return epicBoons[Random.Range(0, epicBoons.Count)];
        }
    }

    public List<BoonInfo> GetUniqueRandomBoons(int count)
    {
        List<BoonInfo> result = new List<BoonInfo>();
        HashSet<BoonInfo> used = new HashSet<BoonInfo>();

        int maxAvailable = commonBoons.Count + uncommonBoons.Count + rareBoons.Count + epicBoons.Count;
        int takeCount = Mathf.Min(count, maxAvailable);

        int attempts = 0;
        while (result.Count < takeCount && attempts < 100)
        {
            BoonInfo boon = GetNextPurchaseableBoon();
            if (boon != null && !used.Contains(boon))
            {
                result.Add(boon);
                used.Add(boon);
            }
            attempts++;
        }
        return result;
    }
}
