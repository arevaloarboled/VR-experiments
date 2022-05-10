using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using Valve.VR;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class MainController : MonoBehaviour
{    

    // Start is called before the first frame update    
    public GameObject callibration;

    public GameObject RigthHand;

    // [SerializeField]
    public GameObject SaveText;
    public GameObject SaveSelect;

    //[SerializeField]
    public GameObject RecordText;
    public GameObject RecordHead;
    public GameObject recorded_prefab;

    public GameObject pos_text;
    public GameObject cam;
    public GameObject follower;        

    public GameObject light;
    public GameObject measurement_grid;
    public Material shine;

    private List<GameObject> recorded_points;

    private Callibration callibration_status;

    public bool is_recording=false;
    public bool is_saving = false;
    [Tooltip("Delta time for recording samples.")]
    public float delta_record_time = 0.01f;
    private float temp_time = 0f;    
    private float temp_time_prefab = 0f;
    [Tooltip("Delta time for illumination path.")]
    public float delta_shiny_time = 0.001f;
    public List<Vector3> record_pos;
    public bool finish_record = false;
    [Tooltip("Is user holding trigger?")]
    private bool is_pressed = false;
    private Thread clientReceiveThread;
    private TcpClient socketConnection;
    [Tooltip("Is system illustrating spherical coordinates?.")]
    private bool spherical = true;
    public bool can_retry = false;
    [Tooltip("Is waiting for instructions from the server?")]
    public bool waiting = true;

    public AudioSource sound_source;
    private string msg_from_r = "";    

    void Start()
    {
        RecordText.gameObject.SetActive(false);
        RecordHead.gameObject.SetActive(false);
        SaveText.gameObject.SetActive(false);
        SaveSelect.gameObject.SetActive(false);
        callibration_status = callibration.GetComponent<Callibration>();
        recorded_points = new List<GameObject>();
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(On_pressed, SteamVR_Input_Sources.RightHand);
        SteamVR_Actions.default_GrabPinch.AddOnStateUpListener(Un_pressed, SteamVR_Input_Sources.RightHand);
        SteamVR_Actions.default_GrabGrip.AddOnStateUpListener(Change_coor, SteamVR_Input_Sources.RightHand);
        SteamVR_Actions.default_Menu.AddOnStateUpListener(retry, SteamVR_Input_Sources.RightHand);
        SaveText.GetComponent<Text>().text = "Wait for instructions...";
        SaveText.gameObject.SetActive(true);
        callibration_status.hide();
        ConnectToTcpServer();

    }
    //retry sound again
    private void retry(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (!can_retry) return;
        sound_source.time = 0;
        RecordText.gameObject.SetActive(false);
        RecordHead.gameObject.SetActive(false);
        SaveText.gameObject.SetActive(false);
        SaveSelect.gameObject.SetActive(false);        
        callibration.GetComponent<Callibration>().Recallibrate();
        foreach (var i in recorded_points)
        {
            Destroy(i);
        }
        Debug.Log(record_pos);
        recorded_points.Clear();
        temp_time = 0;
        is_recording = false;
        finish_record = false;
        SaveText.gameObject.SetActive(false);
        SaveSelect.gameObject.SetActive(false);
    }

    private void Change_coor(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        spherical = !spherical;
    }

    private void On_pressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        Debug.Log("PRESSED");
        is_pressed = true;
    }
    private void Un_pressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        Debug.Log("UNPRESSED");
        is_pressed = false;
        if (is_recording) { Save(); finish_record = true; }
    }

    // Update is called once per frame
    void Update()
    {   
        //display coordinates in spherical or eclidian system     
        if (spherical)
        {
            float elevation = 0;
            float azimuth = 0;
            float distance = Mathf.Abs(Vector3.Distance(RigthHand.transform.position, RecordHead.transform.position)) / 0.25f;
            //Calculate diretion vector between RecordHead and sound source	
            Vector3 dir = (RigthHand.transform.position - RecordHead.transform.position).normalized;
            //Calculate angle of elevation between RecordHead and sound source        
            if (Vector3.Cross(RecordHead.transform.right, Vector3.ProjectOnPlane(dir, RecordHead.transform.up)) == Vector3.zero)
            {
                Vector3 dirE = Vector3.ProjectOnPlane(dir, RecordHead.transform.forward);
                elevation = Vector3.SignedAngle(RecordHead.transform.right, dirE, RecordHead.transform.forward);
            }
            else
            {
                Vector3 dirE = Vector3.ProjectOnPlane(dir, RecordHead.transform.right);
                elevation = -Vector3.SignedAngle(RecordHead.transform.forward, dirE, RecordHead.transform.right);
            }
            elevation = elevation % 180 == 0 ? 0 : elevation;
            if (elevation < -90f)
            {
                elevation = -90 - (elevation % 90);
            }
            if (elevation > 90f)
            {
                elevation = 90 - (elevation % 90);
            }
            //Calculate angle of azimuth between RecordHead and sound source
            Vector3 dirA = Vector3.ProjectOnPlane(dir, RecordHead.transform.up);
            azimuth = Vector3.SignedAngle(RecordHead.transform.forward, dirA, RecordHead.transform.up);
            if (azimuth < 0f)
            {
                azimuth = 360f + azimuth;
            }
            pos_text.GetComponent<TextMesh>().text = "D=" + Math.Round((double)distance, 1).ToString() + "cm\nE=" + Math.Round((double)elevation, 1).ToString() + "º\nA=" + Math.Round((double)azimuth, 1).ToString() + "º";
        }
        else
        {
            Vector3 tmp = RigthHand.transform.position - RecordHead.transform.position;
            tmp = tmp / 0.25f;
            pos_text.GetComponent<TextMesh>().text = "X=" + Math.Round((double)tmp.x, 1).ToString() + "cm\nY=" + Math.Round((double)tmp.y, 1).ToString() + "cm\nZ=" + Math.Round((double)tmp.z, 1).ToString() + "cm";
        }
        if (!is_recording) pos_text.GetComponent<TextMesh>().text = "";
        pos_text.transform.LookAt(pos_text.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        follower.transform.position = new Vector3(RigthHand.transform.position.x, follower.transform.position.y, RigthHand.transform.position.z);
        //pos_text.transform.LookAt(cam.transform);        

        // Debug.Log((RigthHand.transform.position - RecordHead.transform.position)/0.25f);
        // Debug.DrawLine(RecordHead.transform.position,RigthHand.transform.position);

        //receive a new song
        if (msg_from_r != "")
        {
            // if(msg_from_r == "{/wait/}"){
            //     //server is telling to wait to the subject, action from experimenter maybe required
            //     SaveText.GetComponent<Text>().text = "Wait for instructions...";
            //     RecordText.GetComponent<Text>().text = "Wait for instructions...";
            // }else{
            //restart everything
            RecordText.gameObject.SetActive(false);
            RecordHead.gameObject.SetActive(false);
            SaveText.gameObject.SetActive(false);
            SaveSelect.gameObject.SetActive(false);
            sound_source.clip = Resources.Load<AudioClip>(msg_from_r);
            callibration.GetComponent<Callibration>().Recallibrate();
            waiting = false;
            msg_from_r = "";

            foreach (var i in recorded_points)
            {
                Destroy(i);
            }
            Debug.Log(record_pos);
            recorded_points.Clear();
            temp_time = 0;
            is_recording = false;
            finish_record = false;
            SaveText.gameObject.SetActive(false);
            SaveSelect.gameObject.SetActive(false);
            can_retry = true;                     
        }
        if(waiting) return;
        if (is_recording && is_pressed && !finish_record) //instantaite and record points
        {
            temp_time = temp_time + Time.deltaTime;
            if (temp_time >= delta_record_time)
            {                
                record_pos.Add((RigthHand.transform.position - RecordHead.transform.position) / 0.25f);
                GameObject temp = (GameObject)Instantiate(recorded_prefab, RigthHand.transform.position, Quaternion.identity);
                recorded_points.Add(temp);
                temp_time = 0f;
            }
        }
        //ready to start recording
        if (!is_recording && !finish_record && callibration_status.is_callibrated && !sound_source.isPlaying) Record();
        //illuminate path
        if (is_recording || finish_record)
        {
            temp_time_prefab = temp_time_prefab + Time.deltaTime;
            if (temp_time_prefab >= delta_shiny_time) { shine_prefabs(); temp_time_prefab = 0; }
        }
    }

    public void shine_prefabs()
    {
        bool next = false;
        foreach (var i in recorded_points)
        {
            Behaviour h = (Behaviour)i.GetComponent("Halo");
            if (next)
            {
                h.enabled = true;
                return;
            }
            if (h.enabled)
            {
                h.enabled = false;
                next = true;
            }
        }
        if (recorded_points.Count != 0)
        {
            Behaviour h = (Behaviour)recorded_points[0].GetComponent("Halo");
            h.enabled = true;
        }
    }

    public void reset_exp(){
        record_pos = new List<Vector3>();
        foreach (var i in recorded_points)
        {
            Destroy(i);
        }
        recorded_points.Clear();
        record_pos = new List<Vector3>();
        RecordText.gameObject.SetActive(false);
        RecordHead.gameObject.SetActive(false);        
        SaveSelect.gameObject.SetActive(false);
        SaveText.GetComponent<Text>().text = "Wait for instructions...";
        SaveText.gameObject.SetActive(true);
        callibration_status.hide();       
    }

    public void Record()
    {
        finish_record = false;
        SaveText.gameObject.SetActive(false);
        SaveSelect.gameObject.SetActive(false);
        RecordText.gameObject.SetActive(true);
        RecordHead.gameObject.SetActive(true);
        light.gameObject.SetActive(true);
        measurement_grid.gameObject.SetActive(true);
        RecordText.GetComponent<Text>().text = "Please record the answer.";
        RenderSettings.skybox = shine;
        is_recording = true;
        record_pos = new List<Vector3>();
        foreach (var i in recorded_points)
        {
            Destroy(i);
        }
        // Debug.Log(record_pos);
        recorded_points.Clear();
        temp_time = 0;

    }

    public void Save()
    {
        is_recording = false;

        RecordText.gameObject.SetActive(false);
        //RecordHead.gameObject.SetActive(false);
        SaveText.GetComponent<Text>().text = "Do you want to save and continue？";
        SaveText.gameObject.SetActive(true);
        SaveSelect.gameObject.SetActive(true);

    }



    ///sockets functions
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);            
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    /// 
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 6000);            
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        //int number;

                        //if (!int.TryParse(serverMessage, out number)) msg_from_r = serverMessage;
                        msg_from_r = serverMessage;
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
            // socketConnection.Close();            
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void SendMessage(string msg)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = msg;
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

     void OnApplicationQuit(){
         socketConnection.Close();
     }

}






