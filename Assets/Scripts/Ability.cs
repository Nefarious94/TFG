using UnityEngine;


[CreateAssetMenu(fileName = "NuevaHabilidad", menuName = "Sistema/Habilidad")]
public class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea] public string description;
}