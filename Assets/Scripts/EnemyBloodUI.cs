using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBloodUI : MonoBehaviour
{
    public Slider[] EyeHealthSlider;

    public Text[] EyeHealthTxt;

    public Health[] EyeHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i =0;i<4;i++)
        {
            Health h;
            if(EyeHealth[i].TryGetComponent<Health>(out h))
            {
                float h_num = h.health;
                EyeHealthSlider[i].value = h_num;
                EyeHealthTxt[i].text = h_num.ToString();
            }
        }
    }
}
