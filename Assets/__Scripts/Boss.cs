using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    
    public float rotationsPerSecond = 0.1f;
    public float bossHealth = 10f;
    public bool notifiedOfDestruction = false;
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public float showDamageDuration = 0.1f;

    public Main checkEnermyIsCleared;
    public Vector3 bossPos;
    public float speed;


    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    // Start is called before the first frame update
    void Start(){
        bossPos = this.transform.position;
    }
    void Awake()
    {
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i=0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        
        if(bossHealth > 10)
        {
            //rotation
            float rZ = -(rotationsPerSecond * 10 * Time.time * 360) % 360f;
            transform.rotation = Quaternion.Euler(0, 0, rZ);
        }
        else{
            float rZ2 = -(rotationsPerSecond * Random.Range(0,10) * Time.time * 360) % 360f;
            transform.rotation = Quaternion.Euler(0, 0, rZ2);
        }

        if(fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void FixedUpdate()
    {
        //when player defeat all enermy, boss enermy will move into the camera
        if(checkEnermyIsCleared.clearEnermy == true)
        {
            if(bossPos.y > 20f)
            {
                Vector3 tempPos = bossPos;
                tempPos.y -= speed * Time.deltaTime;
                bossPos = tempPos;
                this.transform.position = bossPos;
            }
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;

        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                ShowDamage();
                // Get the damage amount from the Main WEAP_DICT
                bossHealth -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(bossHealth <= 0)
                {
                    // Tell the Main singleton that this ship was destroyed
                    
                    notifiedOfDestruction = true;
                    // Destroy this enemy
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
            default:
                    print("Enemy hit by non-ProjectileHero: " + other.name);
                    break;
        }
    }

    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for (int i=0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
