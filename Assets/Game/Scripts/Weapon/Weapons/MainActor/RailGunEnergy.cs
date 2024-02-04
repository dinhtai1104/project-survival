using UnityEngine;

public class RailGunEnergy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer energy;
    private int maxStack = 5;
    private int currentStack = 0;
    public void SetMaxStack(int maxStack)
    {
        this.maxStack = maxStack;
        currentStack = 0;

        Color a = Color.white;
        a.a = currentStack * 1.0f / maxStack;
        energy.color = a;
    }
    public void Stack()
    {
        currentStack++;
        if (currentStack >= maxStack)
        {
            currentStack = 0;
        }
        Color a = Color.white;
        a.a = currentStack * 1.0f / maxStack;
        energy.color = a;
    }
}