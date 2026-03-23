using inkolorgames;
using UnityEngine;

public class DemoManager : PersistentMonoSingleton<DemoManager>
{
    [SerializeField] private bool isDemo;
    public bool IsDemo => isDemo;

}
