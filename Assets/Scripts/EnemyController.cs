using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EnemyController : MonoBehaviour
{
    //行走速度和旋转速度
    public float MoveSpeed;
    public float RotateSpeed;
    //控制走路方式
    public int key = 0;
    //当前是否行走
    public bool temp = true;
    
    // Start is called before the first frame update
    void Start()
    {
        key = 1;
        StartCoroutine("Wait");
    }

    // Update is called once per frame
    void Update()
    {
        if (temp == false)
        {
            //行走为否直接跳过，不执行后面的走路代码
            return;
        }
        //开始行走，这里可以修改为四种行走方式
        switch (key)
        {
            case 1:
                //向前走
                transform.Translate(0, 0, 1 * MoveSpeed * Time.deltaTime, Space.Self);
                //旋转1度
                transform.Rotate(0, 1 * RotateSpeed * Time.deltaTime, 0, Space.Self);
                break;
            case 2:
                transform.Translate(1 * MoveSpeed * Time.deltaTime, 0, 0, Space.Self);
                transform.Rotate(0, 1 * RotateSpeed * Time.deltaTime, 0, Space.Self);
                break;
            case 3:
                transform.Translate(0, 0, 1 * MoveSpeed * Time.deltaTime, Space.Self);
                transform.Rotate(0, 1 * RotateSpeed * Time.deltaTime, 0, Space.Self);
                break;
            case 4:
                transform.Translate(1 * MoveSpeed * Time.deltaTime, 0, 0, Space.Self);
                transform.Rotate(0, 1 * RotateSpeed * Time.deltaTime, 0, Space.Self);
                break;

        }
    }

    IEnumerator Wait()
    {
        while (true)
        {
            //两秒运行一次Timer函数
            yield return new WaitForSeconds(2);
            Timer();
        }
    }

    void Timer()
    {
        //生成随机数1-3
        int i = Random.Range(0, 4);
        //走路的概率为3/2
        if (i > 1)
        {
            temp = true;
            
            //自身旋转，原地向后转
            transform.Rotate(0, 180, 0, Space.Self);
            return;
        }
        else
        {
            temp = false;
            
        }
        //换一种走路方式，这里是按顺序，你也可以改成随机
        key++;
        //走路方式控制在1-4开区间内，别问我开区间是啥
        if (key == 5)
        {
            key = 1;
        }
    }
}
