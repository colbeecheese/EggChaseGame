using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using TMPro;
//using InControl;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private int count;

    public float speed;
    public Text countText;
    public TMP_Text winText;
    public Text timerText;

    public float currentTimer;
    public bool isLevelComplete;

    public int countToComplete;
    int currentLevel;
    public Sprite sprite1; // up sprite
    public Sprite sprite2; // down sprite
    public Sprite sprite3; //right sprite
    public Sprite sprite4; //left sprite
    private SpriteRenderer spriteRenderer;

    void Start()
    {

        //InitializeFirebaseSDK();
        Debug.Log("System time = " + System.DateTime.Now.Month + System.DateTime.Now.Day + System.DateTime.Now.Year);
        Debug.Log("System time = " + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second);
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ufogame-bcdca.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp app = FirebaseApp.DefaultInstance;

        //Debug.Log("PlayerStats = " + PlayerStats.test);
        
        //FirebaseDatabase.DefaultInstance.GetReference("Level1").SetValueAsync("New time");




        currentLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        Debug.Log("PlayerPrefs CurrentLevel = " + PlayerPrefs.GetInt("CurrentLevel"));
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Gameobject
        if (spriteRenderer.sprite == null) // if the sprite on spriteRenderer is null then
            spriteRenderer.sprite = sprite1; // set the sprite to sprite1
        count = 0;
        StartCoroutine(SetCountText());
        winText.SetText ("");
        timerText.text = "Timer: ";
        isLevelComplete = false;


        FirebaseDatabase.DefaultInstance.GetReference("Jim").GetValueAsync().ContinueWith(task => 
        {
          if (task.IsFaulted)
            {
                    // Handle the error...
                    Debug.Log("isFaulted");
            }
          else if (task.IsCompleted)
            {
              DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    Debug.Log("snapshot string = " + snapshot.ToString());
           
            }
        });

    }


    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
       
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb2d.AddForce(movement * speed);

        //Swap the gameobject sprite based on input direction
        var angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        //Debug.Log("Angle= " + angle);
        if (angle > 45f && angle <= 135f) // If the player is moving up
        {
            spriteRenderer.sprite = sprite1; // call method to change sprite
        }
        if (angle > -135f && angle <= -45f)
        {
            spriteRenderer.sprite = sprite2;
        }
        if (angle > -45f && angle <= 45f)
        {
            spriteRenderer.sprite = sprite3;
        }
        if (angle > 135f && angle <= 225f)
        {
            spriteRenderer.sprite = sprite4;
        }

        StartCoroutine(SetCountText());

        if (!isLevelComplete)
        {
            currentTimer = currentTimer + Time.deltaTime;
            timerText.text = "Timer: " + currentTimer.ToString("00.00");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
        }

    }
    void InitializeFirebaseSDK()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    IEnumerator SetCountText ()
    {
 
        countText.text = "Count = " + count.ToString();
        if (count >= countToComplete)
        {
            UpdateFirebaseData(currentLevel);
            isLevelComplete = true;
            winText.SetText ("You Win!");

            yield return new WaitForSeconds(2); //time before the next scene is loaded

            if (currentLevel < 6) //build index starts at 0
            {
                SceneManager.LoadScene(currentLevel + 1);
            }
            else
            {
               
                Application.Quit();
            }

            PlayerStats.test = PlayerStats.test + count; //code to just test persistent data
            
        }
    }
    void UpdateFirebaseData(int level)
    {
        Debug.Log("UpdateFirebaseData()");
        string currentSystemTime = System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString();
        FirebaseDatabase.DefaultInstance.GetReference("Level" + currentLevel).Child(currentSystemTime).SetValueAsync(timerText.text);
        Debug.Log("Firebase Reference" + FirebaseDatabase.DefaultInstance.GetReference("Level" + currentLevel));
    }

    void UpdateFirebaseDataHorses()
    {
//        FirebaseDatabase.DefaultInstance.GetReference("Level" + currentLevel).Child(currentSystemTime).SetValueAsync(timerText.text);

    }

    void ReadFirebaseDataHorses()
    {

    }

}