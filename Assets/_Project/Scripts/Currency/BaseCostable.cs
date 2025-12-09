using UnityEngine;

public abstract class BaseCostable : MonoBehaviour, ICostable
{
    private int cost;
    public int Cost => cost;

    public virtual void Initalize(int cost) => this.cost = cost;
}
