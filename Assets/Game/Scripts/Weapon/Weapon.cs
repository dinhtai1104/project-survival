using Cysharp.Threading.Tasks;
using Game.GameActor;
using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Character character;
    [SerializeField]
    SpriteRenderer []weaponSRs;
    public Transform [] muzzlePlaces,weapons;
    [SerializeField]
    private ParticleSystem[] muzzles;
    private Transform _transform;
    public WeaponBase.OnStack onStackListener;

    Vector3 defaultPosition;
    private Transform hand;
    public Vector3 DefaultPosition
    {
        get { return defaultPosition; }

        set { defaultPosition = value; transform.localPosition = value; }
    }
    public virtual void SetUp(Character character,WeaponBase weapon)
    {
        _transform = transform;
        this.character = character;
      
        muzzlePlaces = new Transform[transform.childCount];
        weapons = new Transform[transform.childCount];
        weaponSRs = new SpriteRenderer[transform.childCount];
        muzzles = new ParticleSystem[transform.childCount];

        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = transform.GetChild(i);

            weaponSRs[i] = weapons[i].GetComponent<SpriteRenderer>();

            muzzlePlaces[i] = weapons[i].GetChild(0);
            muzzles[i] = muzzlePlaces[i].GetComponentInChildren<ParticleSystem>();
        }
        hand = character.WeaponHandler.transform;
        SetActive(true);
        gameObject.SetActive(true);
    }
    public void Rotate(Vector2 direction)
    {
        direction.x = Mathf.Abs(direction.x);
        foreach(Transform weapon in weapons)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            weapon.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    Vector3 left = new Vector3(-1, 1, 1);
    Vector3 right = new Vector3(1, 1, 1);
    public void Flip(float direction)
    {
        if (direction == 0) return;
        _transform.localScale = direction > 0 ? right : left;
    }
    int index = 0;
    public Transform GetShootPoint()
    {
        return muzzlePlaces[(index) % muzzlePlaces.Length];
    }
    public void SetNextShootPoint()
    {
        index++;
    }
    public void SetActive(bool active)
    {
        foreach (SpriteRenderer weaponSR in weaponSRs)
        {
            weaponSR.gameObject.SetActive(active);
        }
    }

    public virtual void Trigger()
    {
        if (weaponSRs[index % muzzlePlaces.Length].isVisible)
        {
            muzzles[index % muzzlePlaces.Length].Play();
        }
    }
    private void Update()
    {
        Vector2 pos = hand.position+DefaultPosition;
        _transform.localPosition = pos;
    }

    public void PlayTriggerImpact(float knockBackTime, float knockBack)
    {
        transform.Shake(knockBackTime, 0.5f, knockBack).ContinueWith(() => 
        {
            _transform.localPosition = DefaultPosition;
        }).Forget();
    }
}
