using UnityEngine;
using UnityEngine.Events;

public class EaterManager : MonoBehaviour
{
    [SerializeField] private UnityEvent<Vector3> onEatBlob;

    private void OnEnable() => EaterEnemy.OnEatAnyBlob += HandleEatAnyBlob;
    private void OnDisable() => EaterEnemy.OnEatAnyBlob -= HandleEatAnyBlob;
    private void HandleEatAnyBlob(Vector3 vector) => onEatBlob.Invoke(vector);
}
