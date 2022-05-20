0. Tengo una acción genérica

```csharp

// Habria que implementar una clase a parte, ya que las interfaces no permiten definir el cuerpo de los metodos
interface IExecutableAction
{
    cancellationToken: CancellationTokenSource;

    priorExecutableAction: IExecutableAction ;
    
    // Esto no es el tiempo que tarda en realizarse la acción, es el tiempo que está en el buffer
    // antes de ser descartada.
    DurationInBuffer: float;
    PostRecheckTime: float;
    TimeLeft: float;
    IsExecuting: bool;

    func Execute(); -> void

    // Usually here we see if the last action finished executing
    func CheckCanExecute() => !priorExecutableAction.IsExecuting; -> bool

    event Action ActionExecuted;
    event Action ActionCancelled;

    //Buffer related stuff
    Buffer<T> _buffer;

    func OnActionInserted(Buffer<T> buffer)
    {
        _buffer = buffer;
        TimeLeft = DurationInBuffer;
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        ActionInBufferDuration(cancellationToken.Token).Forget();
        RecheckLoop(cancellationToken.Token).Forget();
    }

    func OnActionRemoved() -> void
    {
        cancellationToken.Cancel();
    }

    ~IExecutableAction()
    {
        cancellationToken.Dispose();
    }

    async func ActionInBufferDuration(CancellationToken cancellation) -> asyncFunc
    {
        while (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
        _buffer.Remove(this);
    }

    async func RecheckLoop(CancellationToken cancellation) -> asyncFunc
    {
        while (true)
        {
            if (TryExecute())
            {
                Execute();
                IsExecuting = true;
                _buffer.Remove(this);
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
    }

    func CancelExecution() -> void
    {
        if(IsExecuting)
        {
            ActionCancelled?.Invoke();
            cancellationToken.Cancel();
            IsExecuting = false;
        }
    }
}

```

1. Tengo tipos de acciones

```csharp

    class MeleeAction extends IExecutableAction {}
    class RangedAction extends IExecutableAction {}
    class  MovementAction extends IExecutableAction {}
    class SignatureAction extends IExecutableAction {}

```

2. La unidad tiene un contenedor de datos que tiene varias de estas acciones agrupadas por tipo que funcionan como combos y estas acciones son intercambiables
   Veremos este tipo más adelante
    - SkillSet<MeleeAction>
    - SkillSet<RangedAction>
    - SkillSet<MovementAction>
    - SkillSet<SignatureAction>

3. Existe un buffer de acciones comparible con todas las acciones
    Buffer<IExecutableAction>

4. Internamente el buffer es una lista de acciones inversa. La acción a ejecutar es la última que se haya añadido, 
porque es la acción más reciente que ha solicitado el jugador. Hay un limite de acciones en el buffer, digamos 3. Las acciones salen
automaticamente del buffer si no se han ejecutado en x tiempo. Si el buffer está lleno se eliminan las acciones más antiguas.

```csharp

class Buffer<T>
{
    buffer: List<T>;
    maxSize: int = 3;

    public Buffer(int maxSize = 3 )
    {
        this.maxSize = maxSize;
        buffer = new List<T>();
    }

    func Add(T action) -> void
    {
        if(buffer.Count == maxSize)
            buffer.RemoveAt(0);
        
        buffer.Add(action);
        action.OnInserted(this);
    }

    func Remove(T action) -> void
    {
        buffer.Remove(action);
        action.OnRemoved(this);
    }

    func GetLastAction() -> T
    {
        return buffer.Last();
    }

    func Clear() -> void
    {
        buffer.Clear();
    }
}

```

5. La unidad (player o enemigo) delegan el saber que accion toca a un tipo externo que contiene el buffer y las acciones
    
    ```csharp

    public class SkillSet<T> where T extends IExecutableAction
    {
        currentSkill: int;

        skills: List<IExecutableAction>;

        public SkillSet(List<IExecutableAction> skills)
        {
            this.skills = skills;

            foreach(var skill in skills)
            {
                skill.ActionExecuted += () => GetNextSkill();
                skill.ActionCancleled += () => { currentSkill = 0; };
            }

            // Assign the action prior to other action 
            for(int i = 1; i < skills.Count - 1; i++)
                skills[i].priorExecutableAction = skills[i - 1];
        }

        public ~SkillSet()
        {
            foreach(var skill in skills)
            {
                skill.ActionExecuted.Dispose();
                skill.ActionCancleled.Dispose();
            }
        }

        func Add(IExecutableAction skill) -> void
        {
            skills.Add(skill);
        }

        func Remove(IExecutableAction skill) -> void
        {
            skills.Remove(skill);
        }

        func GetCurrentSkill() -> T
        {
            return skills[currentSkill];
        }

        func GetNextSkill() -> T
        {
            currentSkill++;
            if(currentSkill >= skills.Count)
                currentSkill = 0;

            return skills[currentSkill];
        }

        func CancelCurrentSkill() -> void
        {
            int skillBefore = currentSkill - 1;
            if(skillBefore < 0)
                skillBefore = skills.Count - 1;

            // We cancel the action before the current action
            // Because the current action is the one pending to be executed
            // But the one that may be executed is the one before the current action
            skills[skillBefore].CancelExecution();
        }
    }

    public class UnitSkills extends ScriptableObject
    {
        Buffer<IExecutableAction> buffer;

        SkillSet<MeleeAction> meleeActions;
        SkillSet<RangedAction> rangedActions;
        SkillSet<MovementAction> movementActions;
        SkillSet<SignatureAction> signatureActions;

        func CancelCurrentAction() -> void
        {
            // We dont't know which type of action is the current one
            // So we cancel all of them
            meleeActions.CancelCurrentSkill();
            rangedActions.CancelCurrentSkill();
            movementActions.CancelCurrentSkill();
            signatureActions.CancelCurrentSkill();

            buffer.Clear();
        }

        func ExecuteMeleeAction() -> void
        {
            buffer.Add(meleeActions.GetCurrentSkill());
        }

        func ExecuteRangedAction() -> void
        {
            buffer.Add(rangedActions.GetCurrentSkill());
        }

        func ExecuteMovementAction() -> void
        {
            buffer.Add(movementActions.GetCurrentSkill());
        }

        func ExecuteSignatureAction() -> void
        {
            buffer.Add(signatureActions.GetCurrentSkill());
        }

        func GetNewInstance() => ScriptableObject.CreateInstance<UnitSkills>(); -> void
    }

        public class Unit extends Monobehaviour
        {
            _inputReader: InputReader;
            _skills: UnitSkills;

            func Awake() -> void
            {
                _skills = _skills.GetNewInstance();
                _inputReader.OnMeleeAction += _skills.ExecuteMeleeAction;
                _inputReader.OnRangedAction += _skills.ExecuteRangedAction;
                _inputReader.OnMovementAction += _skills.ExecuteMovementAction;
                _inputReader.OnSignatureAction += _skills.ExecuteSignatureAction;
            }

            func OnDestroy() -> void
            {
                _inputReader.OnMeleeAction -= _skills.ExecuteMeleeAction;
                _inputReader.OnRangedAction -= _skills.ExecuteRangedAction;
                _inputReader.OnMovementAction -= _skills.ExecuteMovementAction;
                _inputReader.OnSignatureAction -= _skills.ExecuteSignatureAction;
            }

            func OnCollisionEnter(Collision collision) -> void
            {
                if(collision.gameObject.CompareTag("Enemy"))
                {
                    // Cancel currentAction, empty buffer
                    // Other script will handle damage and knockback
                    _skills.CancelCurrentAction();

                }
            }
        }

    ```