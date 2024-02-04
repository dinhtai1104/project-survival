using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM && DNP_NewInputSystem
using UnityEngine.InputSystem;
#endif

namespace DamageNumbersPro.Demo
{
    public class DNP_2DDemo : MonoBehaviour
    {
        float nextShotTime;

        void Start()
        {
            nextShotTime = 0;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void Update()
        {
            HandleShooting();
        }
        private void OnGUI()
        {
            if (GUILayout.Button("SHOOT"))
            {
                Shoot();
            }
        }
        void HandleShooting()
        {
           
        }

        void Shoot()
        {
            Vector2 mousePosition = Vector2.zero;

#if ENABLE_INPUT_SYSTEM && DNP_NewInputSystem
            if (Mouse.current != null) {
                mousePosition = Mouse.current.position.ReadValue();
            }
#else
            mousePosition = Input.mousePosition;
#endif

            Vector3 worldPosition = Vector3.zero;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.down, 0.2f);

            //Select Damage Number:
            DNP_PrefabSettings settings = DNP_DemoManager.instance.GetSettings();
            DamageNumber prefab = DNP_DemoManager.instance.GetCurrent();

            //Number:
            float number = 1 + Mathf.Pow(Random.value, 2.2f) * settings.numberRange;
            if (prefab.digitSettings.decimals == 0)
            {
                number = Mathf.Floor(number);
            }

            //Create Damage Number:
            DamageNumber newDamageNumber = prefab.Spawn(worldPosition, number);

            if (hit.collider != null)
            {
                newDamageNumber.SetFollowedTarget(hit.collider.transform);
            }

            //Apply Demo Settings:
            settings.Apply(newDamageNumber);
        }
    }
}
