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

    public ModuleSystem GetModuleSystem(AISensor sensor) => new ModuleSystem(attackModule.GetModule2(sensor), attackRangedModule.GetModule2(sensor), movemenetModule.GetModule2(sensor), signatureModule.GetModule2(sensor));
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
