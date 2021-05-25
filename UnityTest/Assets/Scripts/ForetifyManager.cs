using UnityEngine;
using UnityEngine.UI;
using ForetifyLinker;
using TMPro;
using Consolation;
using Morai.Protobuf.Foretify;
using System.Collections.Generic;

public class Actor : MonoBehaviour
{
    public string id { get; set; }

    public bool isCreated = false;

    public Vector3 Position;
}

public class ForetifyManager : MonoBehaviour
{
    [HideInInspector]
    public static ForetifyManager Instance;

    public int Port = 7788;

    public Button StartServerBtn;

    public Button StopServerBtn;

    private IServer Server;

    public Console Console;

    public GameObject ActorPrefab;

    public GameObject CloneActor { get; set; }

    LockQueue<Actor> poolActors = new LockQueue<Actor>();

    List<GameObject> listActor = new List<GameObject>();

    private Camera MainCamera { get; set; }

    private void Awake()
    {
        Instance = this;

        // server start
        Server = new Server();
        Server.AddReceiver(new Receiver());
        Server.StatusEvent += ServerStatus;
        Server.Start("127.0.0.1", Port);

        MainCamera = Camera.main;
    }

    private void OnApplicationQuit()
    {
        if (Server != null)
        {
            Server.Stop();
        }            
    }

    private void Start()
    {
        // button event
        StartServerBtn.onClick.AddListener(() =>
        {
            Server.Start("127.0.0.1", Port);
        });

        StopServerBtn.onClick.AddListener(() => {
            Server.Stop();
        });

        coord_6dof pos = Converter.ToCoord6dof(1, 2, 3, 4, 5, 6);            
        Vector3 position = new Vector3((float)pos.X.Value, (float)pos.Y.Value, (float)pos.Z.Value);
        Vector3 rotation = new Vector3((float)pos.Roll.Value, (float)pos.Pitch.Value, (float)pos.Yaw.Value);
    }

    public void ServerStatus(string msg)
    {
        Debug.Log(msg);
    }

    private void Update()
    {
        ///Console.
        ///
        while(poolActors.Count > 0)
        {
            Actor actor = poolActors.Dequeue();
            CloneActor = Instantiate(ActorPrefab);
            CloneActor.AddComponent<Actor>();
            CloneActor.transform.position = actor.Position;
            MainCamera.transform.position = CloneActor.transform.position;
            listActor.Add(CloneActor);
        }
    }

    public void CreateActor(string id, double x, double y, double z)
    {
        //CloneActor = Instantiate(ActorPrefab);
        Actor actor = new Actor();
        actor.id = id;
        actor.Position = new Vector3((float)x, (float)y, (float)z);        
        poolActors.Enqueue(actor);
    }
}
