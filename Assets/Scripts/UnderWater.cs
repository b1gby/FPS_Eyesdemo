using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class UnderWater : MonoBehaviour
{
    public GameObject camera;
    public GameObject pc;

    private float blurspread = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = false;//关闭 Fog
        camera.GetComponent<Blur>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //当相机进入水中时开启Fog和Blur组件
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == pc)
        {
            //开启相机上的Blur脚本,设置属性值(可以在可视化面板中设置属性值,这样在代码里就只用控制enable)
            camera.GetComponent<Blur>().enabled = true;
            camera.GetComponent<Blur>().iterations = 3;
            camera.GetComponent<Blur>().blurSpread = blurspread;

            //开启Fog(雾)
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0, 0.4f, 0.7f, 0.6f);//颜色浅蓝
            RenderSettings.fogDensity = 0.04f;

            pc.GetComponent<PlayerController>().isUnderWater = true;
        }
    }

    //离开水下时,关闭Blur和Fog
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == pc)
        {
            camera.GetComponent<Blur>().enabled = false;
            RenderSettings.fog = false;

            pc.GetComponent<PlayerController>().isUnderWater = false;
            pc.GetComponent<Rigidbody>().AddForce(Vector3.down,ForceMode.Impulse);
        }
    }

}