using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails", menuName = "Player/Stats")]
public class PlayerSO : ScriptableObject
{
    public int Level;
    public string PlayerName;
    public bool IsDead;
}
