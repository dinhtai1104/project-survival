using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class TestShowPanel : MonoBehaviour
{
    public TMP_Dropdown buffDropdown;

    [SerializeField,SerializeReference]
    public List<Type> types = new List<Type>();

    private void Start()
    {
        Init();
        List<string> data = new List<string>();
        buffDropdown.ClearOptions();
        foreach (var item in types)
        {
            data.Add(item.Name);
        }
        buffDropdown.AddOptions(data);
    }
    private void Init()
    {
        this.types.Clear();
        Type parentType = typeof(UI.Panel);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();

        IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
        foreach (Type type in subclasses)
        {
            this.types.Add(type);
        }
    }

    public async void Cast()
    {
        await UI.PanelManager.CreateAsync("Panel/" + types[buffDropdown.value] + ".prefab");
    }
}