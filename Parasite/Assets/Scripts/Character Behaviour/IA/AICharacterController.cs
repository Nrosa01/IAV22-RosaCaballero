using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AIBehaviour
{
    Proximidad,
    Cautela,
    Equilibrado,
    Agresividad,
    Defensivo
}

[RequireComponent(typeof(AISensor))]
public class AICharacterController : CharacterBase
{
    [SerializeField] ModulesSO _modules;
    ModuleSystem internalModules;
    AISensor sensor;
    public float timeSinceLastAction = 0;
    public float thinkTime = 0.5f;
    bool isAlive = true;
    public AIBehaviour aIBehaviour;
    ModuleType currentModule;

    protected override void Awake()
    {
        base.Awake();
        sensor = GetComponent<AISensor>();
        internalModules = _modules.GetModuleSystem(sensor);
        Act().Forget();
    }

    async UniTaskVoid Act()
    {
        while (isAlive)
        {
            ModuleStats attackStat = GetModuleStats(ModuleType.Melee, aIBehaviour);
            ModuleStats rangedStat = GetModuleStats(ModuleType.Ranged, aIBehaviour);
            ModuleStats movementStat = GetModuleStats(ModuleType.Movement, aIBehaviour);
            ModuleStats signatureStat = GetModuleStats(ModuleType.Signature, aIBehaviour);

            ModuleType nextAction = GetNextActoni(attackStat, rangedStat, movementStat, signatureStat);
            ExecuteAction(nextAction, GetModuleStats(nextAction, aIBehaviour));

            await UniTask.Delay((int)(thinkTime * 1000), false, default, default);
        }
    }

    ModuleStats GetModuleStats(ModuleType moduleType, AIBehaviour aiBehaviour)
    {
        // En funcion del behaviour modifica o no los stats base que devuelven los modulos
        switch (moduleType)
        {
            case ModuleType.Melee:
                return internalModules.attackModule.GetStats();
            case ModuleType.Ranged:
                return internalModules.attackRangedModule.GetStats();
            case ModuleType.Movement:
                return internalModules.movemenetModule.GetStats();
            case ModuleType.Signature:
                return internalModules.signatureModule.GetStats();
            default:
                return internalModules.attackModule.GetStats(); ;
        }
    }

    Vector3 GetModulePos(ModuleType moduleType, AIBehaviour aiBehaviour)
    {
        // En funcion del behaviour modifica o no los stats base que devuelven los modulos
        switch (moduleType)
        {
            case ModuleType.Melee:
                return internalModules.attackModule.GetOptimalPosition();
            case ModuleType.Ranged:
                return internalModules.attackRangedModule.GetOptimalPosition();
            case ModuleType.Movement:
                return internalModules.movemenetModule.GetOptimalPosition();
            case ModuleType.Signature:
                return internalModules.signatureModule.GetOptimalPosition();
            default:
                return internalModules.attackModule.GetOptimalPosition(); ;
        }
    }

    ModuleType GetNextActoni(ModuleStats attackMeleeStats, ModuleStats attackRangedStats, ModuleStats movementStats, ModuleStats signatureStats)
    {
        List<ModuleStats> stats = new List<ModuleStats>();
        stats.Add(attackMeleeStats);
        stats.Add(attackRangedStats);
        stats.Add(movementStats);
        stats.Add(signatureStats);

        List<ModuleStats> filteredBySuccessRate = stats.FindAll(x => x.successRate > 0.5f);

        if (filteredBySuccessRate.Count == 0)
        {
            List<ModuleStats> fileterdByPriority = stats.OrderByDescending(x => x.priority).ToList();
            return fileterdByPriority[0].type;
        }
        else
        {
            List<ModuleStats> fileterdByPriority = filteredBySuccessRate.OrderByDescending(x => x.priority).ToList();
            return fileterdByPriority[0].type;
        }
    }

    private void Update()
    {
        UpdateDestAndLook();
    }

    void UpdateDestAndLook()
    {
        Vector3 dest = (GetModulePos(currentModule, aIBehaviour) - transform.position).normalized;
        Vector2 destWithoutY = new Vector2(dest.x, dest.z);
        characterInfo.movementInput = destWithoutY;
        characterInfo.lookAtInput = destWithoutY;
    }

    private void OnDrawGizmos()
    {
        if (this.sensor == null) return;
        Vector3 dest = (GetModulePos(currentModule, aIBehaviour) - transform.position).normalized;
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + dest * 5);
    }

    void ExecuteAction(ModuleType type, ModuleStats stats)
    {
        currentModule = type;
        UpdateDestAndLook();

        switch (type)
        {
            case ModuleType.Melee:
                Debug.Log("Attack Melee");
                AttackMelee();
                break;
            case ModuleType.Ranged:
                Debug.Log("Attack Ranged");
                AttackRanged();
                break;
            case ModuleType.Movement:
                //Debug.Log("Movement");
                MoveSkill();
                break;
            case ModuleType.Signature:
                Debug.Log("Signature");
                AttackSignature();
                break;
            default:
                break;
        }
    }
}
