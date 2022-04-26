using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSet<T>  where T : IExecutableAction
{
    int currentSkill = 0;
    int lastExecutedSkill = 0;

    [HideInInspector]public List<T> _skills;
    public List<ActionHolder> skills; 

    public SkillSet()
    {
        Init(null);
    }

    public void Init(GameObject self)
    {
        _skills = new List<T>();
        if (skills == null || skills.Count == 0)
            return;
        
        //Copy holder to skill
        for (int i = 0; i < skills.Count; i++)
        {
            var action = skills[i].GetAction();
            action.Init(self);
            action.IsExecuting = false;
            _skills.Add((T)(IExecutableAction)action);
        }

        foreach (var skill in _skills)
        {
            skill.ActionExecuted += ActionExecuted;
            skill.ActionCancelled += ActionCancelled;
        }

        // Assign the action prior to other action 
        _skills[0].priorExecutableAction = _skills[0];
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
    }

    public void Add(T skill)
    {
        _skills.Add(skill);
    }

    public void Remove(T skill)
    {
        _skills.Remove(skill);
    }

    public T GetCurrentSkill()
    {
        return _skills[currentSkill];
    }

    public void ActionExecuted()
    {
        lastExecutedSkill = currentSkill;
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
    }

    public SkillSet<T> GetNewInstance()
    {
        return this;
    }
}