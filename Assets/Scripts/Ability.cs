using UnityEngine;


[CreateAssetMenu(fileName = "NuevaHabilidad", menuName = "Sistema/Habilidad")]
public class Ability : ScriptableObject
{
    public enum AbilityType { Melee, Ranged, Healing, Buff }

    public string abilityName;
    [TextArea] public string description;
    public AbilityType type;
    public int power;
    public int range;  
    public GameObject projectilePrefab;
}