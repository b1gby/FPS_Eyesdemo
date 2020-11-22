using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBloodUI : MonoBehaviour
{
    public Slider[] EyeHealthSlider;

    public Text[] EyeHealthTxt;

    public GameObject[] EyeHealth;

    // Start is called before the first frame update
    void Start()
    {
        EyeHealth = GameObject.FindGameObjectsWithTag("Eye");
    }

    // Update is called once per frame
    void Update()
    {
        for(int i =0;i<4;i++)
        {
            if(EyeHealth[i]!=null)
            {
                Health h;
                if (EyeHealth[i].TryGetComponent<Health>(out h))
                {
                    float h_num = h.health;
                    EyeHealthSlider[i].value = h_num;
                    EyeHealthTxt[i].text = h_num.ToString();
                }
            }
            
        }
    }
}
