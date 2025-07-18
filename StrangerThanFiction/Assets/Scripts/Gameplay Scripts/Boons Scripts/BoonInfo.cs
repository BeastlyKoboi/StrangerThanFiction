using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoonInfo", menuName = "DataContainer/BoonInfo")]
public class BoonInfo : ScriptableObject
{
    public string BoonName;
    [TextArea]
    public string Description;
    public Sprite Image;
}
