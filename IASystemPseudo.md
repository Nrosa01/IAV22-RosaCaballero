```csharp
public class CharacterBase : MonoBehaviour
{
    public Skills skills;
    protected Health health;
    public CharacterInfo characterInfo;

    protected void AttackMelee() => skills.ExecuteMeleeAction();
    protected void AttackRanged() => skills.ExecuteRangedAction();
    protected void MoveSkill() => skills.ExecuteMovementAction();
    protected void AttackSignature() => skills.ExecuteSignatureAction();
    protected void MoveCharacter(Vector2 direction) => characterInfo.movementInput = direction;
    protected void StopCharacterMovement() => characterInfo.movementInput = Vector2.zero;

    public bool IsExecuting => skills.IsAnySkillExecuting();

    protected virtual void Awake()
    {
        if (skills == null)
            throw new Exception("CharacterBase: Skills not set");

        skills.Initialize(this.gameObject);
    }
}

enum AIBehaviour
{
    Proximidad,
    Cautela,
    Equilibrado,
    Agresividad,
    Defensivo
}

enum ModuleType {
    Melee,
    Ranged,
    Movement,
    Signature
}

class AICharacterController : CharacterBase
{
    Modules modules;
    AIBehaviour aiBehaviour;
    AISensor aiSensor;
    float timeSinceLastAction = 0;
    float thinkTime = 0.5f;

    Start() => ThinkCorroutine();

   async void Act()
   {
     while(alive)
     {
        struct attackStat = GetModuleStats(ModuleType.Attack, aiBehaviour);
        struct attackRange = GetModuleStats(ModuleType.AttackRange, aiBehaviour);
        struct movementStat = GetModuleStats(ModuleType.Movement, aiBehaviour);
        struct signatureStat = GetModuleStats(ModuleType.Signature, aiBehaviour);

        ActionType nextAction = GetNextAction(attackStat,   attackRange, movementStat, signatureStat, aiBehaviour);

        await thinkTime;
     }
   }

   ActionType GetNextAction(attackStat, attackRange, movementStat, signatureStat, aiBehaviour)
   {
      // Cada modulo tiene una tasa de éxito y una prioridad. Se ejecuta el módulo que tenga una tasa de éxito mayor a 0.5 y que tenga mayor prioridad.

      //Para esto metemos los modulos en una lista y la filtramos usando linq. Se puede hacer más eficiente pero por cuestiones de tiempo esta fue la solución elegida

        List<Modulos> modules = new List<Modulos>();
        modules.Add(new Modulos(ModuleType.Melee, attackStat.successRate, attackStat.priority));

        modules.Add(new Modulos(ModuleType.Ranged, attackRange.successRate, attackRange.priority));


        modules.Add(new Modulos(ModuleType.Movement, movementStat.successRate, movementStat.priority));

        modules.Add(new Modulos(ModuleType.Signature, signatureStat.successRate, signatureStat.priority));

        modules = modules.Where(x => x.successRate > 0.5f).ToList();

        // Si no hay ningún módulo que tenga una tasa de éxito mayor a 0.5, se ejecuta el módulo que tenga mayor prioridad.

        if(modules.Count == 0)
            modules = modules.OrderByDescending(x => x.priority).ToList();
        else
            modules = modules.OrderByDescending(x => x.successRate).ToList();

        return modules[0].moduleType;
   }

   ModuleStats GetModuleStats(ModuleType moduleType, AIBehaviour aiBehaviour)
   {
      // En funcion del behaviour modifica o no los stats base que devuelven los modulos
   }

   void DoAction(ModuleType moduleType)
   {
       switch (moduleType)
       {
           case ModuleType.Melee:
               AttackMelee();
               break;
           case ModuleType.Ranged:
               AttackRanged();
               break;
           case ModuleType.Movement:
               MoveSkill();
               break;
           case ModuleType.Signature:
               AttackSignature();
               break;
       }
   }
}

abstract class Module
{
    public ModuleType moduleType;
    AISensor sensor;

    public Module(ModuleType moduleType, AISensor sensor)
    {
        this.moduleType = moduleType;
        this.sensor = sensor;
    }

    public abstract ModuleStats GetStats();

    protected abstract GetSuccessRate();

    protected abstract GetPriority();
}

class Modules : ScriptableObject
{
    Module meleeModule;
    Module rangedModule;
    Module movementModule;
    Module signatureModule;
}

```