using System.Collections.Generic;

namespace Game.GameActor
{
    public class ActorPropertyHandler
    {
        public Dictionary<EActorProperty, float> properties = new Dictionary<EActorProperty, float>();

    
        public void AddProperty(EActorProperty key, float value)
        {
            if (!properties.ContainsKey(key))
            {
                properties.Add(key, value);
            }
            else
            {
                properties[key] = value;
            }
        }
        public int GetProperty(EActorProperty key, int defaultValue = 0)
        {
            if (!properties.ContainsKey(key))
            {
                properties.Add(key, defaultValue);
            }
            return (int)properties[key];
        }

        public float GetProperty(EActorProperty key, float defaultValue = 0f)
        {
            if (!properties.ContainsKey(key))
            {
                properties.Add(key, defaultValue);
            }
            return properties[key];
        }
        
        public void ClearAll()
        {
            properties.Clear();
        }
    }
}

