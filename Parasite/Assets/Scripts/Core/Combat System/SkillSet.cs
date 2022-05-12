using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class SkillSet<T> where T : ExecutableAction
{
    public SkillSet(List<T> newSkills)
    {
        tokenSource = new CancellationTokenSource();
        currentSkill = 0;
        lastExecutedSkill = 0;

        this._skills = new List<T>();

        InitListFromHolders(newSkills);

        foreach (var skill in _skills)
        {
            skill.ActionExecuted += ActionExecuted;
            skill.ActionCancelled += ActionCancelled;
        }

        if (_skills.Count == 0)
        {
            //Debug.LogError("No skills in skill set");
            return;
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

    CancellationTokenSource tokenSource;
    int currentSkill = 0;
    int lastExecutedSkill = 0;

    [HideInInspector] public List<T> _skills;
    public List<T> skills => _skills;

    async UniTaskVoid CooldownToResetSkill(CancellationToken token)
    {
        var currentSkill = _skills[this.lastExecutedSkill];

        // Wait to skill to finish
        while (currentSkill.IsExecuting)
            await UniTask.Yield(PlayerLoopTiming.Update, token);

        // Extra time
        int milliseconds = (int)(1000 * currentSkill.PostRecheckTime) + (int)(1000 * currentSkill.delayToNextAction);
        await UniTask.Delay(milliseconds, false, PlayerLoopTiming.Update, token);

        // Reset skill if token is not cancelled
        if (!token.IsCancellationRequested)
            this.currentSkill = 0;
    }

    public void Init(GameObject self)
    {
        InitActions(self);
    }

    public bool IsExecuting()
    {
        if(_skills.Count == 0)
            return false;
        return _skills[lastExecutedSkill].IsExecuting;
    }

    void InitListFromHolders(List<T> holders)
    {
        if (_skills == null)
            _skills = new List<T>();

        if (holders != null && holders.Count != 0)
        {
            //Copy holder to skill
            for (int i = 0; i < holders.Count; i++)
            {
                var action = holders[i].Clone();
                action.IsExecuting = false;
                _skills.Add((T)action);
            }
        }
    }

    void InitActions(GameObject self)
    {
        foreach (var skill in _skills)
            skill.Init(self);
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
        if (_skills.Count == 0)
            return;

        _skills[lastExecutedSkill].CancelExecution();
        currentSkill = 0;
    }

    public SkillSet<T> GetNewInstance()
    {
        SkillSet<T> skillset = new SkillSet<T>(_skills);
        return skillset;
    }
}