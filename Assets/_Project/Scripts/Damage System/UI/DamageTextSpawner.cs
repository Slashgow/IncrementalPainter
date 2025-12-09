public class DamageTextSpawner : TextSpawner
{
    private void OnEnable() => BaseDamageor.OnAnyDamageorAttack += SpawnDamageText;
    private void OnDisable() => BaseDamageor.OnAnyDamageorAttack -= SpawnDamageText;
   
}