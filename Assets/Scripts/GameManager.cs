using UnityEngine;

public class GameManager : SingletonBase<GameManager> 
{
    [SerializeField] private MatchState _matchState;
    public MatchState MatchState => _matchState;


}