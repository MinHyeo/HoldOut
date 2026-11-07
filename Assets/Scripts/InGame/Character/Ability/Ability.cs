public abstract class Ability 
{
    protected readonly CharacterCombat Owner;
    private readonly AbilityData _data;
    private float _timer;

    protected Ability(CharacterCombat owner, AbilityData data)
    {
        Owner = owner; 
        _data = data;
    }

    public void Tick(float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer > 0f)
            return;

        if (TryActivate())
        {
            _timer = _data.Cooldown;
        }
    }

    protected abstract bool TryActivate();
}