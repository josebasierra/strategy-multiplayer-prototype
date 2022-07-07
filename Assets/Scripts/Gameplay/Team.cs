using UnityEngine;

public enum TeamId
{
    Team0,  
    Team1,
    Neutral,
};

public class Team : MonoBehaviour
{
    [SerializeField] TeamId _id;

    public TeamId Id => _id;
}
