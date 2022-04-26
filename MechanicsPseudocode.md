0. Tengo una acción genérica

```csharp

// Habria que implementar una clase a parte, ya que las interfaces no permiten definir el cuerpo de los metodos
public interface IExecutableAction
{
    public CancellationTokenSource cancellationToken = new CancellationTokenSource();

    public IExecutableAction priorExecutableAction { get; set; }
    
    // Esto no es el tiempo que tarda en realizarse la acción, es el tiempo que está en el buffer
    // antes de ser descartada.
    public float DurationInBuffer { get; set; } 
    public float PostRecheckTime { get; set; }
    public float TimeLeft { get; set; }
    public bool IsExecuting { get; set; }

    void Execute();

    // Usually here we see if the last action finished executing
    virtual bool CheckCanExecute() => !priorExecutableAction.IsExecuting;

    public event Action ActionExecuted;
    public event Action ActionCancelled;

    //Buffer related stuff
    Buffer<T> _buffer;

    public void OnActionInserted(Buffer<T> buffer)
    {
        _buffer = buffer;
        TimeLeft = DurationInBuffer;
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        ActionInBufferDuration(cancellationToken.Token).Forget();
        RecheckLoop(cancellationToken.Token).Forget();
    }

    public void OnActionRemoved()
    {
        cancellationToken.Cancel();
    }

    ~IExecutableAction()
    {
        cancellationToken.Dispose();
    }

    async UniTaskVoid ActionInBufferDuration(CancellationToken cancellation)
    {
        while (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
        _buffer.Remove(this);
    }

    async UniTaskVoid RecheckLoop(CancellationToken cancellation)
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

    void CancelExecution()
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

    public class MeleeAction : IExecutableAction {}
    public class RangedAction : IExecutableAction {}
    public class  MovementAction : IExecutableAction {}
    public class SignatureAction : IExecutableAction {}

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
    public List<T> buffer;
    public int maxSize = 3;

    public Buffer(int maxSize = 3 )
    {
        this.maxSize = maxSize;
        buffer = new List<T>();
    }

    public void Add(T action)
    {
        if(buffer.Count == maxSize)
            buffer.RemoveAt(0);
        
        buffer.Add(action);
        action.OnInserted(this);
    }

    public void Remove(T action)
    {
        buffer.Remove(action);
        action.OnRemoved(this);
    }

    public T GetLastAction()
    {
        return buffer.Last();
    }

    public void Clear()
    {
        buffer.Clear();
    }
}

```

5. La unidad (player o enemigo) delegan el saber que accion toca a un tipo externo que contiene el buffer y las acciones
    
    ```csharp

    public class Unit
    {
        public Buffer<IExecutableAction> buffer;
        public IExecutableAction currentAction;

        public Unit(Buffer<IExecutableAction> buffer)
        {
            this.buffer = buffer;
        }

        public void ExecuteAction(IExecutableAction action)
        {
            currentAction = action;
            action.Execute();
        }

        public void CancelAction()
        {
            currentAction.Cancel();
            currentAction = null;
        }
    }

    public class SkillSet<T> : where T : IExecutableAction
    {
        int currentSkill = 0;

        public List<IExecutableAction> skills;

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

        public void Add(IExecutableAction skill)
        {
            skills.Add(skill);
        }

        public void Remove(IExecutableAction skill)
        {
            skills.Remove(skill);
        }

        public T GetCurrentSkill()
        {
            return skills[currentSkill];
        }

        public T GetNextSkill()
        {
            currentSkill++;
            if(currentSkill >= skills.Count)
                currentSkill = 0;

            return skills[currentSkill];
        }

        void CancelCurrentSkill()
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

    public class UnitSkills : ScriptableObject
    {
        Buffer<IExecutableAction> buffer;

        SkillSet<MeleeAction> meleeActions;
        SkillSet<RangedAction> rangedActions;
        SkillSet<MovementAction> movementActions;
        SkillSet<SignatureAction> signatureActions;

        public void CancelCurrentAction()
        {
            // We dont't know which type of action is the current one
            // So we cancel all of them
            meleeActions.CancelCurrentSkill();
            rangedActions.CancelCurrentSkill();
            movementActions.CancelCurrentSkill();
            signatureActions.CancelCurrentSkill();

            buffer.Clear();
        }

        public void ExecuteMeleeAction()
        {
            buffer.Add(meleeActions.GetCurrentSkill());
        }

        public void ExecuteRangedAction()
        {
            buffer.Add(rangedActions.GetCurrentSkill());
        }

        public void ExecuteMovementAction()
        {
            buffer.Add(movementActions.GetCurrentSkill());
        }

        public void ExecuteSignatureAction()
        {
            buffer.Add(signatureActions.GetCurrentSkill());
        }

        public GetNewInstance() => ScriptableObject.CreateInstance<UnitSkills>();
    }

        public class Unit : Monobehaviour
        {
            [SerializeField] public InputReader _inputReader = default;
            [SerializeField] UnitSkills _skills;

            private void Awake()
            {
                _skills = _skills.GetNewInstance();
                _inputReader.OnMeleeAction += _skills.ExecuteMeleeAction;
                _inputReader.OnRangedAction += _skills.ExecuteRangedAction;
                _inputReader.OnMovementAction += _skills.ExecuteMovementAction;
                _inputReader.OnSignatureAction += _skills.ExecuteSignatureAction;
            }

            private void OnDestroy()
            {
                _inputReader.OnMeleeAction -= _skills.ExecuteMeleeAction;
                _inputReader.OnRangedAction -= _skills.ExecuteRangedAction;
                _inputReader.OnMovementAction -= _skills.ExecuteMovementAction;
                _inputReader.OnSignatureAction -= _skills.ExecuteSignatureAction;
            }

            private void OnCollisionEnter(Collision collision)
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