using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Callibration : MonoBehaviour
{
    public GameObject environment;
    public GameObject obj_indicator;
    public GameObject obj_marker;
    Image indicator;
    Image marker;
    RaycastHit hitInfo;    
    public Vector3 head_position;
    public Vector3 head_rotation;
    public bool is_callibrated=false;
    public GameObject comment_obj = null;

    public Material dark;
    public Light light;
    public Material shine;

    public GameObject Camera;
    public MainController main;
    public GameObject helper;
    public GameObject callibration_text;
    public GameObject measurement_grid;
    public AudioSource sound_source;
    public double soundtime;
    void Start()
    {
        comment_obj.gameObject.SetActive(false);
        indicator =obj_indicator.GetComponent<Image>();
        marker = obj_marker.GetComponent<Image>();
        indicator.color = Color.red;
        marker.color = Color.red;
        head_position = new Vector3();
        head_rotation = new Vector3();
        is_callibrated = false;

        //not callibrated
       
    }

    public void hide() //hide all callibration assets
    {
        indicator.gameObject.SetActive(false);
        marker.gameObject.SetActive(false);
        helper.gameObject.SetActive(false);
        callibration_text.gameObject.SetActive(false);
        comment_obj.gameObject.SetActive(false);
        //head_position = new Vector3(-100, -100, -100);
    }

    void AnimateIndicator(bool isOn)
    {
        if (isOn)
        {
            indicator.fillAmount += 0.5f * Time.deltaTime;
        }
        else
        {
            indicator.fillAmount = 0;
        }
    }

    public void Recallibrate()
    {
        sound_source.Pause();
        indicator.gameObject.SetActive(true);
        marker.gameObject.SetActive(true);
        helper.gameObject.SetActive(true);
        callibration_text.gameObject.SetActive(true);
        comment_obj.gameObject.SetActive(false);
        head_position = new Vector3();
        head_rotation = new Vector3();
        is_callibrated = false;

        light.gameObject.SetActive(true);
        measurement_grid.gameObject.SetActive(true);
        RenderSettings.skybox = shine;
    }

    void FixedUpdate()
    {
        // Debug.Log(Vector3.Angle(Camera.transform.forward, environment.transform.forward));
        if (main.waiting) return;
        if (Vector3.Angle(Camera.transform.forward, environment.transform.forward) > 2f && Vector3.Angle(Camera.transform.up, Vector3.up) > 3f && sound_source.isPlaying && is_callibrated) Recallibrate();
        if (head_position != Vector3.zero) return;        
        //helper.transform.position=new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z+20f); 
        helper.transform.position=Camera.transform.position+(environment.transform.forward*20);
        if (Vector3.Angle(Camera.transform.forward, environment.transform.forward) <= 2f && Vector3.Angle(Camera.transform.up,Vector3.up) <= 3f) //checking where is looking
        {
            
            comment_obj.gameObject.SetActive(false);
            marker.color = Color.green;
            indicator.color = Color.green;
            AnimateIndicator(true);

            if (indicator.fillAmount >= 1) // is sit good!!
            {
                Debug.Log("callibrated!");
                light.gameObject.SetActive(false);
                RenderSettings.skybox = dark;
                //head_position = Camera.transform.position;                  
                head_position = new Vector3(0,25,0);                  
                indicator.gameObject.SetActive(false);
                marker.gameObject.SetActive(false);
                helper.gameObject.SetActive(false);
                callibration_text.gameObject.SetActive(false);
                measurement_grid.gameObject.SetActive(false);
                comment_obj.gameObject.SetActive(false);
                is_callibrated = true;
                sound_source.Play();
              
            }

        }
        else
        {
            is_callibrated = false;
            Recallibrate();
            //not looking at the center....
            indicator.color = Color.red;
            marker.color = Color.red;
            AnimateIndicator(false);

            if (Vector3.Angle(Camera.transform.up, Vector3.up) > 3f)
            {
                  comment_obj.gameObject.SetActive(true);
                 

            }
            else comment_obj.gameObject.SetActive(false);
            
        }
    }
}
