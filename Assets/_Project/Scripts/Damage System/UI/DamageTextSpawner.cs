public class DamageTextSpawner : TextSpawner
{
    private void OnEnable()
    {
        BaseDamageor.OnAnyDamageorAttack += SpawnDamageText;
        BrushSwipe.OnAnyBrushSwipeAttack += SpawnDamageText;
        PoisonZone.OnAnyPoisonAttack += SpawnDamageText;
    }

    private void OnDisable()
    {
        BaseDamageor.OnAnyDamageorAttack -= SpawnDamageText;
        BrushSwipe.OnAnyBrushSwipeAttack -= SpawnDamageText;
        PoisonZone.OnAnyPoisonAttack -= SpawnDamageText;
    }
}