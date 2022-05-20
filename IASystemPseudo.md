```csharp
public class CharacterBase extends MonoBehaviour
{
    skills: Skills;
    health: Health;
    characterInfo: CharacterInfo;

    func AttackMelee() => skills.ExecuteMeleeAction(); -> void
    func AttackRanged() => skills.ExecuteRangedAction(); -> void
    func MoveSkill() => skills.ExecuteMovementAction(); -> void
    func AttackSignature() => skills.ExecuteSignatureAction(); -> void
    func MoveCharacter(Vector2 direction) => characterInfo.movementInput = direction; -> void
    func StopCharacterMovement() => characterInfo.movementInput = Vector2.zero; -> void

    func IsExecuting => skills.IsAnySkillExecuting(); -> bool

    virtual func Awake() -> void
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

class AICharacterController extends CharacterBase
{
    modules: Modules;
    aiBehaviour: AIBehaviour;
    aiSensor: AISensor;
    timeSinceLastAction: float = 0;
    thinkTime: float = 0.5f;

    func Start() => ThinkCorroutine();

   async func Act() -> void
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

   func GetNextAction(attackStat, attackRange, movementStat, signatureStat, aiBehaviour) -> ActionType
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
            modules = modules.OrderByDescendingt(x => x.priority).ToList();
        else
            modules = modules.OrderByDescending(x => x.successRate).ToList();

        return modules[0].moduleType;
   }

   ModuleStats GetModuleStats(ModuleType moduleType, AIBehaviour aiBehaviour)
   {
      // En funcion del behaviour modifica o no los stats base que devuelven los modulos
   }

   func DoAction(ModuleType moduleType) -> void
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
    moduleType: ModuleType;
    sensor: AISensor;

    public Module(ModuleType moduleType, AISensor sensor)
    {
        this.moduleType = moduleType;
        this.sensor = sensor;
    }

    func GetStats() -> ModuleStats;

    func GetSuccessRate() -> float;

    func GetPriority() -> float;
}

class Modules : ScriptableObject
{
    meleeModule: Module;
    rangedModule: Module;
    movementModule: Module;
    signatureModule: Module;
}

```