using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/ModuleSystem", fileName = "AIModuleSystem")]
public class ModulesSO : ScriptableObject
{
    [SerializeField] ModuleSO attackModule;
    [SerializeField] ModuleSO attackRangedModule;
    [SerializeField] ModuleSO movemenetModule;
    [SerializeField] ModuleSO signatureModule;

    public ModuleSystem GetModuleSystem(AISensor sensor) => new ModuleSystem(attackModule.GetModule(sensor), attackRangedModule.GetModule(sensor), movemenetModule.GetModule(sensor), signatureModule.GetModule(sensor));
}

public class ModuleSystem
{
    public Module attackModule;
    public Module attackRangedModule;
    public Module movemenetModule;
    public Module signatureModule;

    public ModuleSystem(Module attackModule, Module attackRangedModule, Module movemenetModule, Module signatureModule)
    {
        this.attackModule = attackModule;
        this.attackRangedModule = attackRangedModule;
        this.movemenetModule = movemenetModule;
        this.signatureModule = signatureModule;
    }
}
