using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class SkillSet<T>  where T : ExecutableAction
{
    CancellationTokenSource tokenSource;
    int currentSkill = 0;
    int lastExecutedSkill = 0;

    [HideInInspector]public List<T> _skills;
    public List<ActionHolder> skills; 

    public SkillSet(List<T> _skills)
    {
        // Copy elements from _skills to _skills
        this._skills = new List<T>();

        for (int i = 0; i < _skills.Count; i++)
        {
            this._skills.Add((T)_skills[i].Clone());
        }
    }

    async UniTaskVoid CooldownToResetSkill(CancellationToken token)
    {
        var currentSkill = _skills[this.lastExecutedSkill];

        // Wait to skill to finish
        while (currentSkill.IsExecuting)
            await UniTask.Yield(PlayerLoopTiming.Update, token);

        // Extra time
        int milliseconds = (int)(1000 * currentSkill.PostRecheckTime);
        await UniTask.Delay(milliseconds, false, PlayerLoopTiming.Update, token);

        // Reset skill if token is not cancelled
        if (!token.IsCancellationRequested)
            this.currentSkill = 0;
    }

    public void Init(GameObject self)
    {
        tokenSource = new CancellationTokenSource();
        currentSkill = 0;
        lastExecutedSkill = 0;
        if(_skills == null)
            _skills = new List<T>();
        
        if (skills != null && skills.Count != 0)
        {
            //Copy holder to skill
            for (int i = 0; i < skills.Count; i++)
            {
                var action = skills[i].GetAction();
                action.Init(self);
                action.IsExecuting = false;
                _skills.Add((T)(IExecutableAction)action);
            }
        }

        if (_skills.Count == 0)
        {
            Debug.LogError("No skills in skill set");
            return;
        }

        foreach (var skill in _skills)
        {
            skill.ActionExecuted += ActionExecuted;
            skill.ActionCancelled += ActionCancelled;
        }

        // Assign the action prior to other action 
        _skills[0].priorExecutableAction = _skills[_skills.Count - 1];
        for (int i = 1; i < _skills.Count; i++)
            _skills[i].priorExecutableAction = _skills[i - 1];
    }

    ~SkillSet()
    {
        foreach (var skill in _skills)
        {
            skill.ActionExecuted -= ActionExecuted;
            skill.ActionCancelled -= ActionCancelled;
        }

        tokenSource.Dispose();
    }

    public void Add(T skill)
    {
        _skills.Add(skill);
    }

    public void Remove(T skill)
    {
        if (!skill.IsExecuting)
            currentSkill = 0;
        _skills.Remove(skill);
    }

    public T GetCurrentSkill()
    {
        return _skills[currentSkill];
    }

    public void ActionExecuted()
    {
        GenericExtensions.CancelAndGenerateNew(ref tokenSource);
        lastExecutedSkill = currentSkill;
        CooldownToResetSkill(tokenSource.Token).Forget();
        GetNextSkill();
    }
    
    public void ActionCancelled()
    {
        currentSkill = 0;
    }

    public T GetNextSkill()
    {
        currentSkill++;
        if (currentSkill >= _skills.Count)
            currentSkill = 0;

        return _skills[currentSkill];
    }

    public void CancelCurrentSkill()
    {
        _skills[lastExecutedSkill].CancelExecution();
        currentSkill = 0;
    }

    public SkillSet<T> GetNewInstance()
    {
        SkillSet<T> skillset = new SkillSet<T>(this._skills);
        return this;
    }
}