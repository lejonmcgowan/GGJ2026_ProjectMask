using System;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioSource AudioSource; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (FindObjectsByType<BGM>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
