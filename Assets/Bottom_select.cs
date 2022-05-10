using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using Valve.VR;
using System.Text;

public class Bottom_select : MonoBehaviour
{
    public MainController main;
    public GameObject finish_obj = null;
    public GameObject yes;
    public GameObject no;
    public GameObject yes_select;
    public GameObject no_select;
    public GameObject RigthHand;
    public GameObject record_text;
    public Callibration callibration_status;
    public bool is_yes = false;
    public bool is_no = false;
    public SteamVR_Action_Vector2 touch_axis;
    private Vector2 axis;

    void Start()
    {
        SteamVR_Actions.default_Teleport.AddOnStateDownListener(On_pressed, SteamVR_Input_Sources.RightHand);
        // yes = GameObject.Find("yes");
        // no = GameObject.Find("no");
        yes_select.SetActive(false);
        no_select.SetActive(false);
        yes.SetActive(true);
        no.SetActive(true);        

    }

    void On_pressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        if (!main.finish_record) return;
        if (axis.y > 0)
        {

            yes.SetActive(true);
            no.SetActive(true);

            if (is_yes == false)
            {
               // Debug.Log("YES?");
                yes.SetActive(false);
                no.SetActive(true);
                yes_select.SetActive(true);
                no_select.SetActive(false);
              
                is_yes = true;
                is_no = false;

            }
            else {
                Debug.Log("Yes");
                main.is_saving = false;
                main.finish_record = false;
                main.can_retry = false;
                main.waiting = true;
                is_yes = false;
                is_no = false;
                yes.SetActive(false);
                no.SetActive(false);
                yes_select.SetActive(false);
                no_select.SetActive(false);
                finish_obj.gameObject.SetActive(true);
                Text finish_text = finish_obj.GetComponent<Text>();
                finish_text.text = "Wait for instructions...";
                record_text.GetComponent<Text>().text = "Wait for instructions...";
                // callibration_status.is_callibrated=false;
                callibration_status.hide();         
                
                var sb = new StringBuilder();
                sb.Append("[");
                for (int i = 0; i < main.record_pos.Count-1; i++)
                {
                    sb.Append($"[{main.record_pos[i].x},{main.record_pos[i].y},{main.record_pos[i].z}],");
                }                

                sb.Append($"[{main.record_pos[main.record_pos.Count-1].x},{main.record_pos[main.record_pos.Count - 1].y},{main.record_pos[main.record_pos.Count - 1].z}]]");
                main.SendMessage(Encoding.ASCII.GetBytes(sb.ToString()).Length.ToString());
                main.SendMessage(sb.ToString());

                //moikai!
                yes_select.SetActive(false);
                no_select.SetActive(false);
                yes.SetActive(true);
                no.SetActive(true);
               
                is_no = false;
                // is_yes = true;
                is_yes = false;                
                main.reset_exp();
            }

        }
        else
        {
            Debug.Log("NO");

           if (is_no == false)
            {
                //タッチパッド下をクリックした場合の処理
               // Debug.Log("NO?");
             
                is_yes = false;
                is_no = true;
                yes.SetActive(true);
                no.SetActive(false);
                yes_select.SetActive(false);
                 no_select.SetActive(true);

            }
            else
            {
                //タッチパッド下をクリックした場合の処理
                
                is_yes = false;
                is_no = false;
                Debug.Log("no");
                main.Record();
                yes.SetActive(true);
                no.SetActive(true);
                yes_select.SetActive(false);
                no_select.SetActive(false);

            }

        }
        }

    void Update()
    {
        axis = touch_axis.GetAxis(SteamVR_Input_Sources.RightHand);        
      
    }

}

